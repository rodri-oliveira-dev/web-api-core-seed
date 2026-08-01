using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.IntegrationTests.Infrastructure;
using WebApiCoreSeed.SampleRestaurant.Models;

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

        Assert.Equal(1, json.GetProperty("totalItems").GetInt32());
        Assert.Equal(1, json.GetProperty("page").GetInt32());
        Assert.Equal(10, json.GetProperty("pageSize").GetInt32());
        Assert.Equal(1, json.GetProperty("totalPages").GetInt32());
        Assert.False(json.GetProperty("hasNextPage").GetBoolean());
        Assert.False(json.GetProperty("hasPreviousPage").GetBoolean());
        Assert.Equal("Risoto de integracao", json.GetProperty("items")[0].GetProperty("titulo").GetString());
    }

    [Fact(DisplayName = "Paginacao de pratos retorna primeira, intermediaria, ultima e pagina apos final")]
    public async Task ObterPratosQuandoPaginasSolicitadasDeveRetornarMetadataConsistente()
    {
        await _factory.ResetStateAsync();
        await SeedPratosAsync("A", "B", "C", "D", "E");

        var first = await GetPratosAsync(pageNumber: 1, pageSize: 2);
        var middle = await GetPratosAsync(pageNumber: 2, pageSize: 2);
        var last = await GetPratosAsync(pageNumber: 3, pageSize: 2);
        var afterEnd = await GetPratosAsync(pageNumber: 4, pageSize: 2);

        AssertPage(first, page: 1, pageSize: 2, totalItems: 5, totalPages: 3, count: 2, hasNext: true, hasPrevious: false);
        Assert.Collection(ReadTitles(first),
            title => Assert.Equal("A", title),
            title => Assert.Equal("B", title));

        AssertPage(middle, page: 2, pageSize: 2, totalItems: 5, totalPages: 3, count: 2, hasNext: true, hasPrevious: true);
        Assert.Collection(ReadTitles(middle),
            title => Assert.Equal("C", title),
            title => Assert.Equal("D", title));

        AssertPage(last, page: 3, pageSize: 2, totalItems: 5, totalPages: 3, count: 1, hasNext: false, hasPrevious: true);
        Assert.Collection(ReadTitles(last),
            title => Assert.Equal("E", title));

        AssertPage(afterEnd, page: 4, pageSize: 2, totalItems: 5, totalPages: 3, count: 0, hasNext: false, hasPrevious: true);
    }

    [Fact(DisplayName = "Paginacao de pratos em colecao vazia retorna metadata zerada")]
    public async Task ObterPratosQuandoColecaoVaziaDeveRetornarPaginaVazia()
    {
        await _factory.ResetStateAsync();

        var json = await GetPratosAsync(pageNumber: 1, pageSize: 10);

        AssertPage(json, page: 1, pageSize: 10, totalItems: 0, totalPages: 0, count: 0, hasNext: false, hasPrevious: false);
    }

    [Fact(DisplayName = "Paginacao de pratos respeita page size minimo e maximo")]
    public async Task ObterPratosQuandoPageSizeNoLimiteDeveRetornarItensLimitados()
    {
        await _factory.ResetStateAsync();
        await SeedPratosAsync(Enumerable.Range(1, 51).Select(index => $"Prato {index:00}").ToArray());

        var min = await GetPratosAsync(pageNumber: 1, pageSize: 1);
        var max = await GetPratosAsync(pageNumber: 1, pageSize: 50);

        AssertPage(min, page: 1, pageSize: 1, totalItems: 51, totalPages: 51, count: 1, hasNext: true, hasPrevious: false);
        AssertPage(max, page: 1, pageSize: 50, totalItems: 51, totalPages: 2, count: 50, hasNext: true, hasPrevious: false);
    }

    [Theory(DisplayName = "Paginacao de pratos invalida retorna Validation Problem Details")]
    [InlineData("pageNumber=0&pageSize=10", "PageNumber")]
    [InlineData("pageNumber=-1&pageSize=10", "PageNumber")]
    [InlineData("pageNumber=1&pageSize=0", "PageSize")]
    [InlineData("pageNumber=1&pageSize=-1", "PageSize")]
    [InlineData("pageNumber=1&pageSize=51", "PageSize")]
    public async Task ObterPratosQuandoPaginacaoInvalidaDeveRetornarProblemDetails(string queryString, string field)
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"api-pagination-invalid-{Guid.NewGuid():N}");

        var response = await client.GetAsync($"/api/v1/Pratos?{queryString}");
        var problem = await JsonAssertions.ReadProblemAsync(response, HttpStatusCode.BadRequest);

        Assert.Equal("urn:problem:validation", problem.GetProperty("type").GetString());
        Assert.True(problem.GetProperty("errors").TryGetProperty(field, out _));
        Assert.True(problem.TryGetProperty("traceId", out _));
    }

    [Fact(DisplayName = "Paginacao de pratos usa ordenacao estavel por titulo e id")]
    public async Task ObterPratosQuandoTitulosRepetidosDeveOrdenarPorTituloEId()
    {
        await _factory.ResetStateAsync();
        await _factory.WithDomainContextAsync(async context =>
        {
            context.Pratos.Add(CreatePrato("Mesmo titulo", "00000000-0000-0000-0000-000000000002"));
            context.Pratos.Add(CreatePrato("Outro titulo", "00000000-0000-0000-0000-000000000003"));
            context.Pratos.Add(CreatePrato("Mesmo titulo", "00000000-0000-0000-0000-000000000001"));
            await context.SaveChangesAsync();
        });

        var json = await GetPratosAsync(pageNumber: 1, pageSize: 10);
        var items = json.GetProperty("items").EnumerateArray().ToArray();

        Assert.Equal("Mesmo titulo", items[0].GetProperty("titulo").GetString());
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), items[0].GetProperty("id").GetGuid());
        Assert.Equal("Mesmo titulo", items[1].GetProperty("titulo").GetString());
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000002"), items[1].GetProperty("id").GetGuid());
        Assert.Equal("Outro titulo", items[2].GetProperty("titulo").GetString());
    }

    [Fact(DisplayName = "Paginacao offset reflete insercoes entre consultas sem quebrar metadata")]
    public async Task ObterPratosQuandoInseridoEntrePaginasDeveAtualizarTotais()
    {
        await _factory.ResetStateAsync();
        await SeedPratosAsync("B", "C", "D");

        var first = await GetPratosAsync(pageNumber: 1, pageSize: 2);
        AssertPage(first, page: 1, pageSize: 2, totalItems: 3, totalPages: 2, count: 2, hasNext: true, hasPrevious: false);

        await SeedPratosAsync("A");

        var second = await GetPratosAsync(pageNumber: 2, pageSize: 2);
        AssertPage(second, page: 2, pageSize: 2, totalItems: 4, totalPages: 2, count: 2, hasNext: false, hasPrevious: true);
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

    [Fact(DisplayName = "Endpoint de escrita persiste Mesa usando Unit of Work")]
    public async Task AdicionarMesaQuandoPayloadValidoDevePersistirComUnitOfWork()
    {
        await _factory.ResetStateAsync();
        var mesaId = Guid.NewGuid();
        using var client = _factory.CreateApiClient(("Mesas", "Adicionar"));

        var response = await client.PostAsJsonAsync("/api/v1/Mesas", new
        {
            id = mesaId,
            numero = "HTTP-UOW",
            lugares = 4,
            ativo = true,
            localizacaoMesa = 1
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var exists = await _factory.WithDomainContextAsync(context =>
            context.Mesas.AnyAsync(item => item.Id == mesaId && item.Numero == "HTTP-UOW"));

        Assert.True(exists);
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

        var response = await client.GetAsync("/health/ready");
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

    private async Task SeedPratosAsync(params string[] titulos)
    {
        await _factory.WithDomainContextAsync(async context =>
        {
            foreach (var titulo in titulos)
            {
                context.Pratos.Add(TestData.CreatePrato(titulo));
            }

            await context.SaveChangesAsync();
        });
    }

    private async Task<JsonElement> GetPratosAsync(int pageNumber, int pageSize)
    {
        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"api-pagination-{Guid.NewGuid():N}");

        var response = await client.GetAsync($"/api/v1/Pratos?pageNumber={pageNumber}&pageSize={pageSize}");

        return await JsonAssertions.ReadJsonAsync(response, HttpStatusCode.OK);
    }

    private static Prato CreatePrato(string titulo, string id)
    {
        var prato = TestData.CreatePrato(titulo);
        prato.Id = Guid.Parse(id);

        return prato;
    }

    private static void AssertPage(
        JsonElement json,
        int page,
        int pageSize,
        int totalItems,
        int totalPages,
        int count,
        bool hasNext,
        bool hasPrevious)
    {
        Assert.Equal(page, json.GetProperty("page").GetInt32());
        Assert.Equal(pageSize, json.GetProperty("pageSize").GetInt32());
        Assert.Equal(totalItems, json.GetProperty("totalItems").GetInt32());
        Assert.Equal(totalPages, json.GetProperty("totalPages").GetInt32());
        Assert.Equal(count, json.GetProperty("items").GetArrayLength());
        Assert.Equal(hasNext, json.GetProperty("hasNextPage").GetBoolean());
        Assert.Equal(hasPrevious, json.GetProperty("hasPreviousPage").GetBoolean());
    }

    private static string[] ReadTitles(JsonElement json)
    {
        return json.GetProperty("items")
            .EnumerateArray()
            .Select(item => item.GetProperty("titulo").GetString()!)
            .ToArray();
    }
}
