using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using WebApiCoreSeed.Api;
using WebApiCoreSeed.Api.ViewModels;
using WebApiCoreSeed.Identity.Infrastructure.Context;
using WebApiCoreSeed.Api.Services.Interfaces;
using WebApiCoreSeed.Api.Settings;
using WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Integracao
{
    public class ProblemDetailsContractTests
    {
        [Fact]
        public async Task PayloadInvalidoDeveRetornarValidationProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient();

            var response = await client.PostAsJsonAsync("/api/v1/entrar", new { });
            var problem = await ReadProblemAsync(response, HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:validation", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
            Assert.True(problem.TryGetProperty("errors", out var errors));
            Assert.True(errors.TryGetProperty("Email", out _));
            Assert.True(errors.TryGetProperty("Password", out _));
        }

        [Theory]
        [InlineData("/api/v1/entrar", SecurityAlgorithms.HmacSha384)]
        [InlineData("/api/v2/entrar", SecurityAlgorithms.HmacSha256)]
        public async Task LoginComCredenciaisValidasDeveRetornarTokenDaVersao(string path, string expectedAlgorithm)
        {
            var email = $"login-{Guid.NewGuid():N}@example.local";
            const string password = "Senha@123A";
            using var factory = new WebApiCoreSeedApiFactory();
            await factory.CreateIdentityUserAsync(email, password, ("Pratos", "ObterPorId"));
            using var client = factory.CreateApiClient();

            var response = await client.PostAsJsonAsync(path, new
            {
                email,
                password
            });
            var json = await ReadJsonAsync(response, HttpStatusCode.OK);

            AssertLoginResponse(json, email, expectedAlgorithm);
        }

        [Fact]
        public async Task RegistrarV1ComUsuarioAutenticadoDeveCriarContaERetornarToken()
        {
            var email = $"register-{Guid.NewGuid():N}@example.local";
            const string password = "Senha@123A";
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WebApiCoreSeedApiFactory.CreateTokenWithoutPermission());

            var response = await client.PostAsJsonAsync("/api/v1/nova-conta", new
            {
                email,
                password,
                confirmPassword = password
            });
            var json = await ReadJsonAsync(response, HttpStatusCode.OK);

            AssertLoginResponse(json, email, SecurityAlgorithms.HmacSha384);
        }

        [Fact]
        public async Task LoginComSenhaInvalidaDeveRetornarDomainProblemDetails()
        {
            var email = $"wrong-password-{Guid.NewGuid():N}@example.local";
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(authenticationSensitivePermitLimit: 10));
            await factory.CreateIdentityUserAsync(email, "Senha@123A");
            using var client = factory.CreateApiClient();

            var response = await client.PostAsJsonAsync("/api/v1/entrar", new
            {
                email,
                password = "Senha@456B"
            });
            var problem = await ReadProblemAsync(response, HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:domain-rule", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task LoginComUsuarioBloqueadoDeveRetornarDomainProblemDetails()
        {
            var email = $"locked-{Guid.NewGuid():N}@example.local";
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(authenticationSensitivePermitLimit: 10));
            await factory.CreateIdentityUserAsync(email, "Senha@123A");
            using var client = factory.CreateApiClient();

            for (var attempt = 0; attempt < 3; attempt++)
            {
                var failed = await client.PostAsJsonAsync("/api/v1/entrar", new
                {
                    email,
                    password = "Senha@456B"
                });
                Assert.Equal(HttpStatusCode.BadRequest, failed.StatusCode);
            }

            var response = await client.PostAsJsonAsync("/api/v1/entrar", new
            {
                email,
                password = "Senha@456B"
            });
            var problem = await ReadProblemAsync(response, HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:domain-rule", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task RecursoInexistenteDeveRetornarNotFoundProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient(("Pratos", "ObterPorId"));

            var response = await client.GetAsync($"/api/v1/Pratos/{Guid.NewGuid()}");
            var problem = await ReadProblemAsync(response, HttpStatusCode.NotFound);

            Assert.Equal("urn:problem:not-found", problem.GetProperty("type").GetString());
            var requestUri = response.RequestMessage?.RequestUri
                ?? throw new InvalidOperationException("Expected response request URI.");
            Assert.Equal("/api/v1/Pratos/" + requestUri.Segments[^1], problem.GetProperty("instance").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task ObterPratoExistenteDeveRetornarOk()
        {
            var prato = CreatePrato("Prato existente");
            var repository = new StatefulPratoRepository(prato);
            using var factory = new WebApiCoreSeedApiFactory(services =>
            {
                services.RemoveAll<IPratoRepository>();
                services.AddSingleton<IPratoRepository>(repository);
            });
            using var client = factory.CreateApiClient(("Pratos", "*"));

            var json = await ReadJsonAsync(await client.GetAsync($"/api/v1/Pratos/{prato.Id}"), HttpStatusCode.OK);

            Assert.Equal(prato.Id, json.GetProperty("id").GetGuid());
            Assert.Equal("Prato existente", json.GetProperty("titulo").GetString());
        }

        [Fact]
        public async Task AdicionarPratoValidoDeveRetornarCreatedComFotoGerada()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient(("Pratos", "*"));

            var response = await client.PostAsJsonAsync("/api/v1/Pratos", CreatePratoRequest());
            var json = await ReadJsonAsync(response, HttpStatusCode.Created);
            var data = json.GetProperty("data");

            Assert.True(json.GetProperty("success").GetBoolean());
            Assert.Equal("Prato HTTP", data.GetProperty("titulo").GetString());
            Assert.EndsWith(".png", data.GetProperty("foto").GetString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AdicionarPratoQuandoImagemAusenteDeveRetornarDomainProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient(("Pratos", "*"));
            var payload = CreatePratoRequest(fotoUpload: null);

            var problem = await ReadProblemAsync(await client.PostAsJsonAsync("/api/v1/Pratos", payload), HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:domain-rule", problem.GetProperty("type").GetString());
            Assert.Contains("imagem", problem.GetProperty("errors").GetRawText(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AdicionarPratoQuandoImagemNaoEhBase64DeveRetornarDomainProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient(("Pratos", "*"));
            var payload = CreatePratoRequest(fotoUpload: "nao-e-base64");

            var problem = await ReadProblemAsync(await client.PostAsJsonAsync("/api/v1/Pratos", payload), HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:domain-rule", problem.GetProperty("type").GetString());
            Assert.Contains("Base64", problem.GetProperty("errors").GetRawText(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AtualizarPratoQuandoIdDivergenteDeveRetornarDomainProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient(("Pratos", "*"));
            var payload = CreatePratoRequest();

            var problem = await ReadProblemAsync(
                await client.PutAsJsonAsync($"/api/v1/Pratos/{Guid.NewGuid()}", payload),
                HttpStatusCode.BadRequest);

            Assert.Equal("urn:problem:domain-rule", problem.GetProperty("type").GetString());
            Assert.Contains("ids", problem.GetProperty("errors").GetRawText(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AtualizarPratoExistenteDevePersistirAlteracoes()
        {
            var prato = CreatePrato("Prato anterior");
            var repository = new StatefulPratoRepository(prato);
            using var factory = new WebApiCoreSeedApiFactory(services =>
            {
                services.RemoveAll<IPratoRepository>();
                services.AddSingleton<IPratoRepository>(repository);
            });
            using var client = factory.CreateApiClient(("Pratos", "*"));
            var payload = CreatePratoRequest(prato.Id, titulo: "Prato atualizado", foto: "atualizado.webp");

            var response = await client.PutAsJsonAsync($"/api/v1/Pratos/{prato.Id}", payload);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.NotNull(repository.UpdatedPrato);
            Assert.Equal("Prato atualizado", repository.UpdatedPrato.Titulo);
            Assert.EndsWith(".png", repository.UpdatedPrato.Foto, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ExcluirPratoExistenteDeveRemoverComSucesso()
        {
            var prato = CreatePrato("Prato removido");
            var repository = new StatefulPratoRepository(prato);
            using var factory = new WebApiCoreSeedApiFactory(services =>
            {
                services.RemoveAll<IPratoRepository>();
                services.AddSingleton<IPratoRepository>(repository);
            });
            using var client = factory.CreateApiClient(("Pratos", "*"));

            var response = await client.DeleteAsync($"/api/v1/Pratos/{prato.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(prato.Id, repository.RemovedPratoId);
        }

        [Fact]
        public async Task RegraDeDominioDeveRetornarBadRequestProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
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
            using var factory = new WebApiCoreSeedApiFactory(services =>
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
        public async Task RequisicaoCanceladaNaoDeveRetornarProblemDetails500()
        {
            var probe = new CancellationProbe();
            using var factory = new WebApiCoreSeedApiFactory(services =>
            {
                services.AddSingleton(probe);
                services.RemoveAll<IPratoRepository>();
                services.AddScoped<IPratoRepository, BlockingPratoRepository>();
            });
            using var client = factory.CreateApiClient();
            using var cancellationTokenSource = new CancellationTokenSource();

            var request = client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10", cancellationTokenSource.Token);
            await probe.Started.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.True(probe.TokenCanBeCanceled);

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => request);
        }

        [Fact]
        public async Task EndpointProtegidoSemTokenDeveRetornarUnauthorizedProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient();

            var response = await client.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
            var problem = await ReadProblemAsync(response, HttpStatusCode.Unauthorized);

            Assert.Equal("urn:problem:authentication", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task EndpointProtegidoComTokenSemPermissaoDeveRetornarForbiddenProblemDetails()
        {
            using var factory = new WebApiCoreSeedApiFactory();
            using var client = factory.CreateApiClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WebApiCoreSeedApiFactory.CreateTokenWithoutPermission());

            var response = await client.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
            var problem = await ReadProblemAsync(response, HttpStatusCode.Forbidden);

            Assert.Equal("urn:problem:authorization", problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task OpenApiScalarEHealthCheckDeveResponderNoHostDeTeste()
        {
            using var factory = new WebApiCoreSeedApiFactory();
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
            using var factory = new WebApiCoreSeedApiFactory();
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
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(publicPermitLimit: 2));
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
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(publicPermitLimit: 2));
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
        public async Task LoginAcimaDoLimiteNaoDevePermitirBypassPorClientIdAnonimo()
        {
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(authenticationSensitivePermitLimit: 1));
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
            Assert.Equal(HttpStatusCode.TooManyRequests, recovered.StatusCode);
        }

        [Fact]
        public async Task LoginAcimaDoLimiteNaoDeveConfiarEmForwardedHeaderDeProxyNaoConfiavel()
        {
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(authenticationSensitivePermitLimit: 1));
            using var firstClient = factory.CreateApiClient();
            using var secondClient = factory.CreateApiClient();
            firstClient.DefaultRequestHeaders.Add("X-Forwarded-For", "203.0.113.10");
            secondClient.DefaultRequestHeaders.Add("X-Forwarded-For", "203.0.113.20");

            Assert.Equal(HttpStatusCode.BadRequest, (await firstClient.PostAsJsonAsync("/api/v1/entrar", new { })).StatusCode);

            var blocked = await secondClient.PostAsJsonAsync("/api/v1/entrar", new { });

            await ReadProblemAsync(blocked, HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task LoginAcimaDoLimiteDeveUsarForwardedHeaderSomenteComProxyConfiavel()
        {
            using var factory = new WebApiCoreSeedApiFactory(
                configureRateLimits: CreateRateLimitConfiguration(authenticationSensitivePermitLimit: 1),
                configureConfiguration: configuration =>
                {
                    configuration["ForwardedHeaders:Enabled"] = "true";
                    configuration["ForwardedHeaders:KnownNetworks:0"] = "0.0.0.0/0";
                });
            using var firstAddress = factory.CreateApiClient();
            using var secondAddress = factory.CreateApiClient();
            firstAddress.DefaultRequestHeaders.Add("X-Forwarded-For", "203.0.113.10");
            secondAddress.DefaultRequestHeaders.Add("X-Forwarded-For", "203.0.113.20");

            Assert.Equal(HttpStatusCode.BadRequest, (await firstAddress.PostAsJsonAsync("/api/v1/entrar", new { })).StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, (await secondAddress.PostAsJsonAsync("/api/v1/entrar", new { })).StatusCode);

            var blocked = await firstAddress.PostAsJsonAsync("/api/v1/entrar", new { });

            await ReadProblemAsync(blocked, HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task LimiteAutenticadoDeveSerIndependentePorUsuario()
        {
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(authenticatedPermitLimit: 1));
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
            using var factory = new WebApiCoreSeedApiFactory(configureRateLimits: CreateRateLimitConfiguration(
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

        private static void AssertLoginResponse(JsonElement json, string email, string expectedAlgorithm)
        {
            Assert.True(json.GetProperty("success").GetBoolean());
            var data = json.GetProperty("data");
            var accessToken = data.GetProperty("accessToken").GetString();
            Assert.False(string.IsNullOrWhiteSpace(accessToken));

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            Assert.Equal(expectedAlgorithm, jwt.Header.Alg);
            Assert.Equal(email, data.GetProperty("userToken").GetProperty("email").GetString());
            Assert.True(data.GetProperty("expiresIn").GetDouble() > 0);
        }

        private static PratoRequestViewModel CreatePratoRequest(
            Guid? id = null,
            string titulo = "Prato HTTP",
            string descricao = "Prato criado via teste HTTP.",
            string? fotoUpload = "aW1hZ2VtLWRlLXRlc3Rl",
            string foto = "prato.png")
        {
            return new PratoRequestViewModel
            {
                Id = id ?? Guid.NewGuid(),
                Titulo = titulo,
                Descricao = descricao,
                FotoUpload = fotoUpload,
                Foto = foto,
                Preco = 25.5,
                Ativo = true,
                TipoPrato = ETipoPrato.Comida
            };
        }

        private static Prato CreatePrato(string titulo)
        {
            return new Prato
            {
                Id = Guid.NewGuid(),
                Titulo = titulo,
                Descricao = "Prato existente para teste HTTP.",
                Foto = "existente.png",
                Preco = 42.5,
                Ativo = true,
                TipoPrato = ETipoPrato.Comida
            };
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

        private sealed class WebApiCoreSeedApiFactory : WebApplicationFactory<Program>
        {
            private const string TestSecret = "X-BURGUER@COCA-2-PROBLEM-DETAILS-TEST-SECRET-2026";
            private readonly Action<IServiceCollection>? _configureServices;
            private readonly Action<NativeRateLimitingSettings>? _configureRateLimits;
            private readonly Action<Dictionary<string, string?>>? _configureConfiguration;
            private readonly string _databaseName = Guid.NewGuid().ToString();

            public WebApiCoreSeedApiFactory(
                Action<IServiceCollection>? configureServices = null,
                Action<NativeRateLimitingSettings>? configureRateLimits = null,
                Action<Dictionary<string, string?>>? configureConfiguration = null)
            {
                _configureServices = configureServices;
                _configureRateLimits = configureRateLimits;
                _configureConfiguration = configureConfiguration;
                EnsureBootstrapConfiguration();
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

            public async Task CreateIdentityUserAsync(string email, string password, params (string Type, string Value)[] claims)
            {
                using var scope = Services.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                var result = await userManager.CreateAsync(user, password);
                Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(error => error.Description)));

                if (claims.Length > 0)
                {
                    var claimResult = await userManager.AddClaimsAsync(user, claims.Select(claim => new Claim(claim.Type, claim.Value)));
                    Assert.True(claimResult.Succeeded, string.Join("; ", claimResult.Errors.Select(error => error.Description)));
                }
            }

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={_databaseName};Integrated Security=True;",
                        ["AppSettings:Secret"] = TestSecret,
                        ["RedisCacheSettings:Enabled"] = "false",
                        ["SeqSettings:Enabled"] = "false",
                        ["SeqSettings:Url"] = "http://localhost",
                        ["SeqSettings:FilePath"] = "test-problem-details.log",
                        ["OpenTelemetry:Enabled"] = "false"
                    };
                    _configureConfiguration?.Invoke(values);
                    configuration.AddInMemoryCollection(values);

                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<SampleRestaurantDbContext>();
                    services.RemoveAll<ApplicationDbContext>();
                    services.RemoveAll<DbContextOptions<SampleRestaurantDbContext>>();
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.RemoveAll<IDbContextOptionsConfiguration<SampleRestaurantDbContext>>();
                    services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();

                    services.AddDbContext<SampleRestaurantDbContext>(options => options.UseInMemoryDatabase(_databaseName));
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
                    new Claim(ClaimTypes.Email, "test@example.local")
                };
                tokenClaims.AddRange(claims.Select(claim => new Claim(claim.Type, claim.Value)));

                var token = new JwtSecurityToken(
                    issuer: "WebApiCoreSeed",
                    audience: "https://localhost",
                    claims: tokenClaims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature));

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            private static void EnsureBootstrapConfiguration()
            {
                Environment.SetEnvironmentVariable(
                    "ConnectionStrings__DefaultConnection",
                    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WebApiCoreSeedBootstrapTests;Integrated Security=True;");
                Environment.SetEnvironmentVariable("AppSettings__Secret", TestSecret);
            }
        }

        private sealed class FakePratoRepository : IPratoRepository
        {
            public Task Adicionar(Prato prato, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task Atualizar(Prato prato, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task RemoverPorId(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Prato?>(null);

            public Task<bool> ExisteComId(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(false);

            public Task<IReadOnlyList<PratoListItem>> ListarPagina(PaginationParameter paginationParameter, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<PratoListItem>>(Array.Empty<PratoListItem>());

            public Task<int> Contar(CancellationToken cancellationToken = default) => Task.FromResult(0);

        }

        private sealed class StatefulPratoRepository : IPratoRepository
        {
            private readonly Dictionary<Guid, Prato> _pratos;

            public StatefulPratoRepository(params Prato[] pratos)
            {
                _pratos = pratos.ToDictionary(prato => prato.Id);
            }

            public Prato? UpdatedPrato { get; private set; }

            public Guid? RemovedPratoId { get; private set; }

            public Task Adicionar(Prato prato, CancellationToken cancellationToken = default)
            {
                _pratos[prato.Id] = prato;
                return Task.CompletedTask;
            }

            public Task Atualizar(Prato prato, CancellationToken cancellationToken = default)
            {
                UpdatedPrato = prato;
                _pratos[prato.Id] = prato;
                return Task.CompletedTask;
            }

            public Task RemoverPorId(Guid id, CancellationToken cancellationToken = default)
            {
                RemovedPratoId = id;
                _pratos.Remove(id);
                return Task.CompletedTask;
            }

            public Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
            {
                _pratos.TryGetValue(id, out var prato);
                return Task.FromResult(prato);
            }

            public Task<bool> ExisteComId(Guid id, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_pratos.ContainsKey(id));
            }

            public Task<IReadOnlyList<PratoListItem>> ListarPagina(PaginationParameter paginationParameter, CancellationToken cancellationToken = default)
            {
                var items = _pratos.Values
                    .OrderBy(prato => prato.Titulo)
                    .ThenBy(prato => prato.Id)
                    .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                    .Take(paginationParameter.PageSize)
                    .Select(prato => new PratoListItem
                    {
                        Id = prato.Id,
                        Titulo = prato.Titulo,
                        Descricao = prato.Descricao,
                        Foto = prato.Foto,
                        Preco = prato.Preco,
                        Ativo = prato.Ativo,
                        TipoPrato = prato.TipoPrato
                    })
                    .ToArray();

                return Task.FromResult<IReadOnlyList<PratoListItem>>(items);
            }

            public Task<int> Contar(CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_pratos.Count);
            }
        }

        private sealed class FakeMesaRepository : IMesaRepository
        {
            public Task Adicionar(Mesa mesa, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task Atualizar(Mesa mesa, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task RemoverPorId(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task<Mesa?> ObterPorId(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Mesa?>(null);

        }

        private sealed class ThrowingPratoRepository : IPratoRepository
        {
            public Task Adicionar(Prato prato, CancellationToken cancellationToken = default) => Task.FromException(CreateException());

            public Task Atualizar(Prato prato, CancellationToken cancellationToken = default) => Task.FromException(CreateException());

            public Task RemoverPorId(Guid id, CancellationToken cancellationToken = default) => Task.FromException(CreateException());

            public Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default) => Task.FromException<Prato?>(CreateException());

            public Task<bool> ExisteComId(Guid id, CancellationToken cancellationToken = default) => Task.FromException<bool>(CreateException());

            public Task<IReadOnlyList<PratoListItem>> ListarPagina(PaginationParameter paginationParameter, CancellationToken cancellationToken = default) => Task.FromException<IReadOnlyList<PratoListItem>>(CreateException());

            public Task<int> Contar(CancellationToken cancellationToken = default) => Task.FromException<int>(CreateException());

            private static InvalidOperationException CreateException()
            {
                return new InvalidOperationException("sensitive-sql connection string stack token");
            }
        }

        private sealed class BlockingPratoRepository : IPratoRepository
        {
            private readonly CancellationProbe _probe;

            public BlockingPratoRepository(CancellationProbe probe)
            {
                _probe = probe;
            }

            public Task Adicionar(Prato prato, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task Atualizar(Prato prato, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task RemoverPorId(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Prato?>(null);

            public Task<bool> ExisteComId(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(false);

            public async Task<IReadOnlyList<PratoListItem>> ListarPagina(PaginationParameter paginationParameter, CancellationToken cancellationToken = default)
            {
                _probe.MarkStarted(cancellationToken);
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

                return Array.Empty<PratoListItem>();
            }

            public Task<int> Contar(CancellationToken cancellationToken = default) => Task.FromResult(0);

        }

        private sealed class CancellationProbe
        {
            private readonly TaskCompletionSource<bool> _started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public Task Started => _started.Task;

            public bool TokenCanBeCanceled { get; private set; }

            public void MarkStarted(CancellationToken cancellationToken)
            {
                TokenCanBeCanceled = cancellationToken.CanBeCanceled;
                _started.TrySetResult(true);
            }
        }
    }
}
