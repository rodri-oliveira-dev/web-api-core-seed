using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.IntegrationTests.Infrastructure;

namespace WebApiCoreSeed.IntegrationTests.Api;

[Collection(ApiIntegrationFixtureDefinition.Name)]
[Trait("Category", IntegrationCategories.Integration)]
[Trait("Category", IntegrationCategories.Container)]
public sealed class ApiContractIntegrationTests
{
    private readonly ApiFactory _factory;

    public ApiContractIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Endpoint publico valido retorna sucesso usando SQL Server e Redis reais")]
    public async Task ObterPratosQuandoEndpointValidoDeveRetornarSucesso()
    {
        await _factory.ResetStateAsync();
        await _factory.WithDomainContextAsync(async context =>
        {
            context.Pratos.Add(TestData.CreatePrato("Risoto de integracao"));
            await context.SaveChangesAsync();
        });

        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"api-valid-{Guid.NewGuid():N}");

        var response = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");
        var json = await JsonAssertions.ReadJsonAsync(response, HttpStatusCode.OK);

        Assert.Equal(1, json.GetProperty("totalItens").GetInt32());
        Assert.Equal("Risoto de integracao", json.GetProperty("data")[0].GetProperty("titulo").GetString());
    }

    [Fact(DisplayName = "Payload invalido retorna Validation Problem Details")]
    public async Task EntrarQuandoPayloadInvalidoDeveRetornarProblemDetails()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"api-invalid-{Guid.NewGuid():N}");

        var response = await client.PostAsJsonAsync("/api/v1/entrar", new { });
        var problem = await JsonAssertions.ReadProblemAsync(response, HttpStatusCode.BadRequest);

        Assert.Equal("urn:problem:validation", problem.GetProperty("type").GetString());
        Assert.True(problem.TryGetProperty("traceId", out _));
        Assert.True(problem.GetProperty("errors").TryGetProperty("Email", out _));
        Assert.True(problem.GetProperty("errors").TryGetProperty("Password", out _));
    }

    [Fact(DisplayName = "Recurso inexistente retorna Not Found Problem Details")]
    public async Task ObterMesaQuandoRecursoNaoExisteDeveRetornarProblemDetails()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient(("Mesas", "ObterPorId"));

        var response = await client.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
        var problem = await JsonAssertions.ReadProblemAsync(response, HttpStatusCode.NotFound);

        Assert.Equal("urn:problem:not-found", problem.GetProperty("type").GetString());
        Assert.True(problem.TryGetProperty("traceId", out _));
    }

    [Fact(DisplayName = "Endpoint protegido sem credencial retorna Unauthorized Problem Details")]
    public async Task ObterMesaQuandoSemCredencialDeveRetornarUnauthorized()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();

        var response = await client.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
        var problem = await JsonAssertions.ReadProblemAsync(response, HttpStatusCode.Unauthorized);

        Assert.Equal("urn:problem:authentication", problem.GetProperty("type").GetString());
        Assert.True(problem.TryGetProperty("traceId", out _));
    }

    [Fact(DisplayName = "Endpoint protegido com token sem permissao retorna Forbidden Problem Details")]
    public async Task ObterMesaQuandoSemPermissaoDeveRetornarForbidden()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AuthenticationHelper.CreateToken());

        var response = await client.GetAsync($"/api/v1/Mesas/{Guid.NewGuid()}");
        var problem = await JsonAssertions.ReadProblemAsync(response, HttpStatusCode.Forbidden);

        Assert.Equal("urn:problem:authorization", problem.GetProperty("type").GetString());
        Assert.True(problem.TryGetProperty("traceId", out _));
    }

    [Fact(DisplayName = "Regra de dominio invalida retorna Domain Rule Problem Details")]
    public async Task AdicionarMesaQuandoRegraDeDominioInvalidaDeveRetornarProblemDetails()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient(("Mesas", "Adicionar"));

        var response = await client.PostAsJsonAsync("/api/v1/Mesas", new
        {
            id = Guid.NewGuid(),
            numero = "A1",
            lugares = 41,
            ativo = true,
            localizacaoMesa = 1
        });
        var problem = await JsonAssertions.ReadProblemAsync(response, HttpStatusCode.BadRequest);

        Assert.Equal("urn:problem:domain-rule", problem.GetProperty("type").GetString());
        Assert.True(problem.GetProperty("errors").TryGetProperty("notifications", out _));
    }

    [Fact(DisplayName = "Rate limit publico retorna Too Many Requests Problem Details")]
    public async Task ObterPratosQuandoAcimaDoLimiteDeveRetornarTooManyRequests()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"api-rate-{Guid.NewGuid():N}");

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10")).StatusCode);

        var response = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");
        var problem = await JsonAssertions.ReadProblemAsync(response, HttpStatusCode.TooManyRequests);

        Assert.Equal("urn:problem:rate-limit", problem.GetProperty("type").GetString());
        Assert.NotNull(response.Headers.RetryAfter);
    }

    [Fact(DisplayName = "Health check responde com SQL Server e Redis saudaveis")]
    public async Task HealthCheckQuandoDependenciasProntasDeveRetornarHealthy()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();

        var response = await client.GetAsync("/hc");
        var json = await JsonAssertions.ReadJsonAsync(response, HttpStatusCode.OK);
        var results = json.TryGetProperty("results", out var healthResults)
            ? healthResults
            : json.GetProperty("entries");

        Assert.Equal("Healthy", json.GetProperty("status").GetString());
        Assert.Equal("Healthy", results.GetProperty("Banco de Dados").GetProperty("status").GetString());
        Assert.Equal("Healthy", results.GetProperty("Cache Redis").GetProperty("status").GetString());
    }

    [Fact(DisplayName = "OpenAPI responde no host de teste")]
    public async Task OpenApiQuandoHostDeTesteDeveResponder()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();

        var openApiV1 = await client.GetAsync("/openapi/v1.json");
        var openApiV2 = await client.GetAsync("/openapi/v2.json");

        Assert.Equal(HttpStatusCode.OK, openApiV1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, openApiV2.StatusCode);
    }
}
