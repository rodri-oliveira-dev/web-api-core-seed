using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Restaurante.IO.Api;
using Restaurante.IO.Api.DataContext;
using Restaurante.IO.Api.Services.Interfaces;
using Restaurante.IO.Api.Settings;
using Restaurante.IO.Business.Interfaces.Pagination;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;
using Xunit;

namespace Pedidos.Test.Integracao
{
    public class ProblemDetailsContractTests
    {
        [Fact]
        public async Task PayloadInvalidoDeveRetornarValidationProblemDetails()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient();

            var response = await client.PostAsJsonAsync("/api/v1/entrar", new { });
            var problem = await ReadProblemAsync(response, HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:validation", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
            Assert.True(problem.TryGetProperty("errors", out var errors));
            Assert.True(errors.TryGetProperty("Email", out _));
            Assert.True(errors.TryGetProperty("Password", out _));
        }

        [Fact]
        public async Task RecursoInexistenteDeveRetornarNotFoundProblemDetails()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient(("Pratos", "ObterPorId"));

            var response = await client.GetAsync($"/api/v1/Pratos/{Guid.NewGuid()}");
            var problem = await ReadProblemAsync(response, HttpStatusCode.NotFound);

            Assert.Equal("urn:problem:not-found", problem.GetProperty("type").GetString());
            Assert.Equal("/api/v1/Pratos/" + response.RequestMessage.RequestUri.Segments[^1], problem.GetProperty("instance").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task RegraDeDominioDeveRetornarBadRequestProblemDetails()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient(("Mesas", "Adicionar"));

            var mesa = new
            {
                id = Guid.NewGuid(),
                numero = "A1",
                lugares = 41,
                ativo = true,
                localizacaoMesa = 1
            };

            var response = await client.PostAsJsonAsync("/api/v1/Mesas", mesa);
            var problem = await ReadProblemAsync(response, HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:domain-rule", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
            Assert.True(problem.TryGetProperty("errors", out var errors));
            Assert.True(errors.TryGetProperty("notifications", out _));
        }

        [Fact]
        public async Task ExcecaoInesperadaDeveRetornarProblemDetailsSemDadosSensiveis()
        {
            using var factory = new RestauranteApiFactory(services =>
            {
                services.RemoveAll<IPratoRepository>();
                services.AddScoped<IPratoRepository, ThrowingPratoRepository>();
            });
            using var client = factory.CreateApiClient(("Pratos", "ObterPorId"));

            var response = await client.GetAsync($"/api/v1/Pratos/{Guid.NewGuid()}");
            var problem = await ReadProblemAsync(response, HttpStatusCode.InternalServerError);
            var body = problem.GetRawText();

            Assert.Equal("urn:problem:unexpected-error", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
            Assert.DoesNotContain("sensitive-sql", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("connection string", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task EndpointProtegidoSemTokenDeveRetornarUnauthorizedProblemDetails()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient();

            var response = await client.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
            var problem = await ReadProblemAsync(response, HttpStatusCode.Unauthorized);

            Assert.Equal("urn:problem:authentication", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task EndpointProtegidoComTokenSemPermissaoDeveRetornarForbiddenProblemDetails()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", RestauranteApiFactory.CreateTokenWithoutPermission());

            var response = await client.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
            var problem = await ReadProblemAsync(response, HttpStatusCode.Forbidden);

            Assert.Equal("urn:problem:authorization", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task OpenApiScalarEHealthCheckDeveResponderNoHostDeTeste()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient();

            var openApiV1 = await client.GetAsync("/openapi/v1.json");
            var openApiV2 = await client.GetAsync("/openapi/v2.json");
            var scalar = await client.GetAsync("/scalar/");
            var health = await client.GetAsync("/hc");

            Assert.Equal(HttpStatusCode.OK, openApiV1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, openApiV2.StatusCode);
            Assert.Equal(HttpStatusCode.OK, scalar.StatusCode);
            Assert.Equal(HttpStatusCode.OK, health.StatusCode);
        }

        [Fact]
        public async Task OpenApiDeveDocumentarVersoesJwtProblemDetailsERateLimit()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient();

            var v1 = await ReadJsonAsync(await client.GetAsync("/openapi/v1.json"), HttpStatusCode.OK);
            var v2 = await ReadJsonAsync(await client.GetAsync("/openapi/v2.json"), HttpStatusCode.OK);

            Assert.Equal("3.0.4", v1.GetProperty("openapi").GetString());
            Assert.Equal("v1", v1.GetProperty("info").GetProperty("version").GetString());
            Assert.Equal("v2", v2.GetProperty("info").GetProperty("version").GetString());

            Assert.True(v1.GetProperty("paths").TryGetProperty("/api/v1/Pratos", out var pratosPath));
            Assert.True(v1.GetProperty("paths").TryGetProperty("/api/v1/Mesas/{id}", out var mesaPath));
            Assert.True(v2.GetProperty("paths").TryGetProperty("/api/v2/entrar", out _));

            var bearer = v1.GetProperty("components").GetProperty("securitySchemes").GetProperty("Bearer");
            Assert.Equal("http", bearer.GetProperty("type").GetString());
            Assert.Equal("bearer", bearer.GetProperty("scheme").GetString());
            Assert.Equal("JWT", bearer.GetProperty("bearerFormat").GetString());

            var publicGet = pratosPath.GetProperty("get");
            Assert.False(publicGet.TryGetProperty("security", out _));
            AssertProblemResponse(publicGet, "429");

            var protectedGet = mesaPath.GetProperty("get");
            Assert.True(protectedGet.TryGetProperty("security", out var security));
            Assert.Contains("Bearer", security.GetRawText(), StringComparison.Ordinal);
            AssertProblemResponse(protectedGet, "401");
            AssertProblemResponse(protectedGet, "403");
            AssertProblemResponse(protectedGet, "429");
        }

        [Fact]
        public async Task RequisicoesPublicasAbaixoDoLimiteDevemSerPermitidas()
        {
            using var factory = new RestauranteApiFactory(configureRateLimits: CreateRateLimitConfiguration(publicPermitLimit: 2));
            using var client = factory.CreateApiClient();
            client.DefaultRequestHeaders.Add("X-ClientId", "public-allowed");

            var first = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");
            var second = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, first.StatusCode);
            Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        }

        [Fact]
        public async Task RequisicaoPublicaAcimaDoLimiteDeveRetornarProblemDetails()
        {
            using var factory = new RestauranteApiFactory(configureRateLimits: CreateRateLimitConfiguration(publicPermitLimit: 2));
            using var client = factory.CreateApiClient();
            client.DefaultRequestHeaders.Add("X-ClientId", "public-blocked");

            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10")).StatusCode);

            var response = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");
            var problem = await ReadProblemAsync(response, HttpStatusCode.TooManyRequests);

            Assert.Equal("urn:problem:rate-limit", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
            Assert.NotNull(response.Headers.RetryAfter);
        }

        [Fact]
        public async Task LoginAcimaDoLimiteDeveUsarParticoesAnonimasIndependentes()
        {
            using var factory = new RestauranteApiFactory(configureRateLimits: CreateRateLimitConfiguration(authenticationSensitivePermitLimit: 1));
            using var firstPartition = factory.CreateApiClient();
            using var secondPartition = factory.CreateApiClient();
            firstPartition.DefaultRequestHeaders.Add("X-ClientId", "login-partition-1");
            secondPartition.DefaultRequestHeaders.Add("X-ClientId", "login-partition-2");

            Assert.Equal(HttpStatusCode.BadRequest, (await firstPartition.PostAsJsonAsync("/api/v1/entrar", new { })).StatusCode);

            var blocked = await firstPartition.PostAsJsonAsync("/api/v1/entrar", new { });
            var problem = await ReadProblemAsync(blocked, HttpStatusCode.TooManyRequests);
            var recovered = await secondPartition.PostAsJsonAsync("/api/v1/entrar", new { });

            Assert.Equal("urn:problem:rate-limit", problem.GetProperty("type").GetString());
            Assert.NotNull(blocked.Headers.RetryAfter);
            Assert.Equal(HttpStatusCode.BadRequest, recovered.StatusCode);
        }

        [Fact]
        public async Task LimiteAutenticadoDeveSerIndependentePorUsuario()
        {
            using var factory = new RestauranteApiFactory(configureRateLimits: CreateRateLimitConfiguration(authenticatedPermitLimit: 1));
            using var firstUser = factory.CreateApiClient(("Mesas", "ObterPorId"));
            using var secondUser = factory.CreateApiClient(("Mesas", "ObterPorId"));

            Assert.Equal(HttpStatusCode.NotFound, (await firstUser.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}")).StatusCode);

            var blocked = await firstUser.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
            var secondUserResponse = await secondUser.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");

            await ReadProblemAsync(blocked, HttpStatusCode.TooManyRequests);
            Assert.Equal(HttpStatusCode.NotFound, secondUserResponse.StatusCode);
        }

        [Fact]
        public async Task HealthCheckDeveFicarIsentoDeRateLimit()
        {
            using var factory = new RestauranteApiFactory(configureRateLimits: CreateRateLimitConfiguration(
                publicPermitLimit: 1,
                authenticatedPermitLimit: 1,
                authenticationSensitivePermitLimit: 1));
            using var client = factory.CreateApiClient();

            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/hc")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/hc")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/hc")).StatusCode);
        }

        private static async Task<JsonElement> ReadProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            return document.RootElement.Clone();
        }

        private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            return document.RootElement.Clone();
        }

        private static void AssertProblemResponse(JsonElement operation, string statusCode)
        {
            var response = operation.GetProperty("responses").GetProperty(statusCode);
            Assert.True(response.GetProperty("content").TryGetProperty("application/problem+json", out _));
        }

        private static Action<NativeRateLimitingSettings> CreateRateLimitConfiguration(
            int publicPermitLimit = 3,
            int authenticatedPermitLimit = 3,
            int authenticationSensitivePermitLimit = 2,
            int windowSeconds = 30)
        {
            return settings =>
            {
                settings.Public.PermitLimit = publicPermitLimit;
                settings.Public.WindowSeconds = windowSeconds;
                settings.Public.QueueLimit = 0;
                settings.Authenticated.PermitLimit = authenticatedPermitLimit;
                settings.Authenticated.WindowSeconds = windowSeconds;
                settings.Authenticated.QueueLimit = 0;
                settings.AuthenticationSensitive.PermitLimit = authenticationSensitivePermitLimit;
                settings.AuthenticationSensitive.WindowSeconds = windowSeconds;
                settings.AuthenticationSensitive.QueueLimit = 0;
            };
        }

        private sealed class RestauranteApiFactory : WebApplicationFactory<Program>
        {
            private const string TestSecret = "X-BURGUER@COCA-2-PROBLEM-DETAILS-TEST-SECRET-2026";
            private readonly Action<IServiceCollection> _configureServices;
            private readonly Action<NativeRateLimitingSettings> _configureRateLimits;
            private readonly string _databaseName = Guid.NewGuid().ToString();

            public RestauranteApiFactory(
                Action<IServiceCollection> configureServices = null,
                Action<NativeRateLimitingSettings> configureRateLimits = null)
            {
                _configureServices = configureServices;
                _configureRateLimits = configureRateLimits;
            }

            public HttpClient CreateApiClient(params (string Type, string Value)[] claims)
            {
                var client = CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                    BaseAddress = new Uri("https://localhost")
                });

                if (claims.Length > 0)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(claims));
                }

                return client;
            }

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["ConnectionStrings:DefaultConnection"] = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={_databaseName};Integrated Security=True;",
                        ["AppSettings:Secret"] = TestSecret,
                        ["RedisCacheSettings:Enabled"] = "false",
                        ["SeqSettings:Enabled"] = "false",
                        ["SeqSettings:Url"] = "http://localhost",
                        ["SeqSettings:FilePath"] = "test-problem-details.log",
                        ["OpenTelemetry:Enabled"] = "false"
                    });

                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<MeuDbContext>();
                    services.RemoveAll<ApplicationDbContext>();
                    services.RemoveAll<DbContextOptions<MeuDbContext>>();
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

                    services.AddDbContext<MeuDbContext>(options => options.UseInMemoryDatabase(_databaseName));
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(_databaseName + "-identity"));
                    services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TestSecret));
                    });
                    services.PostConfigure<HealthCheckServiceOptions>(options => options.Registrations.Clear());
                    services.RemoveAll<RedisCacheSettings>();
                    services.RemoveAll<IResponseCacheService>();
                    services.AddSingleton(new RedisCacheSettings { Enabled = false });
                    if (_configureRateLimits != null)
                    {
                        services.Configure(_configureRateLimits);
                    }

                    services.RemoveAll<IPratoRepository>();
                    services.RemoveAll<IMesaRepository>();
                    services.AddScoped<IPratoRepository, FakePratoRepository>();
                    services.AddScoped<IMesaRepository, FakeMesaRepository>();

                    _configureServices?.Invoke(services);
                });
            }

            public static string CreateTokenWithoutPermission()
            {
                return CreateToken(Array.Empty<(string Type, string Value)>());
            }

            private static string CreateToken(IEnumerable<(string Type, string Value)> claims)
            {
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TestSecret));
                var tokenClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Email, "teste@restaurante.local")
                };
                tokenClaims.AddRange(claims.Select(claim => new Claim(claim.Type, claim.Value)));

                var token = new JwtSecurityToken(
                    issuer: "Restaurante",
                    audience: "https://localhost",
                    claims: tokenClaims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature));

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }

        private abstract class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
        {
            public Task<int> Adicionar(TEntity entity) => Task.FromResult(1);

            public Task<TEntity> ObterPorId(Guid id) => Task.FromResult<TEntity>(null);

            public Task<List<TEntity>> ObterTodos() => Task.FromResult(new List<TEntity>());

            public Task<int> TotalRegistros() => Task.FromResult(0);

            public Task<IEnumerable<TEntity>> Paginacao(PaginationParameter paginationParameter) => Task.FromResult<IEnumerable<TEntity>>(Array.Empty<TEntity>());

            public Task<int> Atualizar(TEntity entity) => Task.FromResult(1);

            public Task<int> Remover(Guid id) => Task.FromResult(1);

            public Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate) => Task.FromResult<IEnumerable<TEntity>>(Array.Empty<TEntity>());

            public Task<int> SaveChanges() => Task.FromResult(1);

            public void Dispose()
            {
            }
        }

        private sealed class FakePratoRepository : FakeRepository<Prato>, IPratoRepository
        {
        }

        private sealed class FakeMesaRepository : FakeRepository<Mesa>, IMesaRepository
        {
        }

        private sealed class ThrowingPratoRepository : IPratoRepository
        {
            public Task<int> Adicionar(Prato entity) => Task.FromException<int>(CreateException());

            public Task<Prato> ObterPorId(Guid id) => Task.FromException<Prato>(CreateException());

            public Task<List<Prato>> ObterTodos() => Task.FromException<List<Prato>>(CreateException());

            public Task<int> TotalRegistros() => Task.FromException<int>(CreateException());

            public Task<IEnumerable<Prato>> Paginacao(PaginationParameter paginationParameter) => Task.FromException<IEnumerable<Prato>>(CreateException());

            public Task<int> Atualizar(Prato entity) => Task.FromException<int>(CreateException());

            public Task<int> Remover(Guid id) => Task.FromException<int>(CreateException());

            public Task<IEnumerable<Prato>> Buscar(Expression<Func<Prato, bool>> predicate) => Task.FromException<IEnumerable<Prato>>(CreateException());

            public Task<int> SaveChanges() => Task.FromException<int>(CreateException());

            public void Dispose()
            {
            }

            private static InvalidOperationException CreateException()
            {
                return new InvalidOperationException("sensitive-sql connection string stack token");
            }
        }
    }
}
