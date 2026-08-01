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
        public async Task SwaggerEHealthCheckDeveResponderNoHostDeTeste()
        {
            using var factory = new RestauranteApiFactory();
            using var client = factory.CreateApiClient();

            var swagger = await client.GetAsync("/swagger/v1/swagger.json");
            var health = await client.GetAsync("/hc");

            Assert.Equal(HttpStatusCode.OK, swagger.StatusCode);
            Assert.Equal(HttpStatusCode.OK, health.StatusCode);
        }

        private static async Task<JsonElement> ReadProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            return document.RootElement.Clone();
        }

        private sealed class RestauranteApiFactory : WebApplicationFactory<Program>
        {
            private const string TestSecret = "X-BURGUER@COCA-2-PROBLEM-DETAILS-TEST-SECRET-2026";
            private readonly Action<IServiceCollection> _configureServices;
            private readonly string _databaseName = Guid.NewGuid().ToString();

            public RestauranteApiFactory(Action<IServiceCollection> configureServices = null)
            {
                _configureServices = configureServices;
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
                        ["DatasulSeqSettings:Enabled"] = "false",
                        ["DatasulSeqSettings:Url"] = "http://localhost",
                        ["DatasulSeqSettings:FilePath"] = "test-problem-details.log"
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
                    services.RemoveAll<IPratoRepository>();
                    services.RemoveAll<IMesaRepository>();
                    services.AddScoped<IPratoRepository, FakePratoRepository>();
                    services.AddScoped<IMesaRepository, FakeMesaRepository>();

                    _configureServices?.Invoke(services);
                });
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
