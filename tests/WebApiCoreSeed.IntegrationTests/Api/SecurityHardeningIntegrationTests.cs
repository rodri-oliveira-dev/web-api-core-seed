using System.Net;
using System.Net.Http.Headers;
using WebApiCoreSeed.IntegrationTests.Infrastructure;

namespace WebApiCoreSeed.IntegrationTests.Api;

[Collection(ApiIntegrationFixtureDefinition.Name)]
[Trait("Category", IntegrationCategories.Integration)]
[Trait("Category", IntegrationCategories.Container)]
public sealed class SecurityHardeningIntegrationTests
{
    private const string AllowedOrigin = "https://app.example.test";
    private const string DeniedOrigin = "https://evil.example.test";

    private readonly ApiFactory _factory;

    public SecurityHardeningIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "CORS preflight permite somente origin configurada")]
    public async Task PreflightQuandoOriginConfiguradaDeveRetornarCorsHeaders()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        using var request = CreatePreflightRequest(AllowedOrigin);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(AllowedOrigin, response.Headers.GetValues("Access-Control-Allow-Origin").Single());
        Assert.False(response.Headers.Contains("Access-Control-Allow-Credentials"));
    }

    [Fact(DisplayName = "CORS preflight rejeita origin nao configurada")]
    public async Task PreflightQuandoOriginNaoConfiguradaNaoDeveRetornarCorsHeaders()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        using var request = CreatePreflightRequest(DeniedOrigin);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.False(response.Headers.Contains("Access-Control-Allow-Credentials"));
    }

    [Fact(DisplayName = "Headers de seguranca modernos sao emitidos sem headers obsoletos")]
    public async Task EndpointPublicoQuandoSucessoDeveEmitirHeadersDeSeguranca()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"security-headers-{Guid.NewGuid():N}");

        var response = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").Single());
        Assert.Contains("camera=()", response.Headers.GetValues("Permissions-Policy").Single());
        Assert.Contains("frame-ancestors 'none'", response.Headers.GetValues("Content-Security-Policy").Single());
        Assert.False(response.Headers.Contains("X-XSS-Protection"));
        Assert.False(response.Headers.Contains("X-Xss-Protection"));
        Assert.False(response.Headers.Contains("Feature-Policy"));
    }

    [Fact(DisplayName = "Health publico nao expoe detalhes internos")]
    public async Task HealthPublicoQuandoHealthyDeveRetornarEstadoMinimo()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();

        var response = await client.GetAsync("/health/live");
        var json = await JsonAssertions.ReadJsonAsync(response, HttpStatusCode.OK);

        Assert.Equal("Healthy", json.GetProperty("status").GetString());
        Assert.False(json.TryGetProperty("entries", out _));
        Assert.False(json.TryGetProperty("results", out _));
    }

    [Fact(DisplayName = "Health legado /hc nao expoe detalhes internos")]
    public async Task HealthLegadoQuandoHealthyDeveRetornarEstadoMinimo()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();

        var response = await client.GetAsync("/hc");
        var json = await JsonAssertions.ReadJsonAsync(response, HttpStatusCode.OK);

        Assert.Equal("Healthy", json.GetProperty("status").GetString());
        Assert.False(json.TryGetProperty("entries", out _));
        Assert.False(json.TryGetProperty("results", out _));
    }

    [Fact(DisplayName = "Readiness reflete dependencias de infraestrutura")]
    public async Task ReadinessQuandoDependenciasProntasDeveExporDetalhesEmTesting()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();

        var response = await client.GetAsync("/health/ready");
        var json = await JsonAssertions.ReadJsonAsync(response, HttpStatusCode.OK);
        var entries = json.GetProperty("entries");

        Assert.Equal("Healthy", json.GetProperty("status").GetString());
        Assert.Equal("Healthy", entries.GetProperty("Banco de Dados").GetProperty("status").GetString());
        Assert.Equal("Healthy", entries.GetProperty("Cache Redis").GetProperty("status").GetString());
    }

    [Fact(DisplayName = "Authorization e query sensivel nao aparecem nos logs capturados")]
    public async Task LogsQuandoRequestComSegredosNaoDevemConterAuthorizationOuQuerySensivel()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        var token = $"secret-token-{Guid.NewGuid():N}";
        var password = $"secret-password-{Guid.NewGuid():N}";
        var initialLength = GetLogLength();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-ClientId", $"security-log-{Guid.NewGuid():N}");

        var response = await client.GetAsync($"/api/v1/Pratos?pageNumber=1&pageSize=10&access_token={token}&password={password}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var logDelta = await ReadLogDeltaAsync(initialLength);

        Assert.DoesNotContain(token, logDelta, StringComparison.Ordinal);
        Assert.DoesNotContain(password, logDelta, StringComparison.Ordinal);
        Assert.DoesNotContain("Authorization", logDelta, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("access_token", logDelta, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("password", logDelta, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Resposta de autenticacao sensivel usa no-store")]
    public async Task EntrarQuandoPayloadInvalidoDeveUsarNoStore()
    {
        await _factory.ResetStateAsync();
        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"security-auth-cache-{Guid.NewGuid():N}");

        var response = await client.PostAsync("/api/v1/entrar", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
        Assert.Contains("no-cache", response.Headers.Pragma.Select(value => value.Name));
    }

    private static HttpRequestMessage CreatePreflightRequest(string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/Pratos");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "Authorization,Content-Type,X-ClientId");

        return request;
    }

    private long GetLogLength()
    {
        return File.Exists(_factory.SerilogFilePath)
            ? new FileInfo(_factory.SerilogFilePath).Length
            : 0;
    }

    private async Task<string> ReadLogDeltaAsync(long initialLength)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(5);

        while (DateTimeOffset.UtcNow < deadline)
        {
            if (File.Exists(_factory.SerilogFilePath) && new FileInfo(_factory.SerilogFilePath).Length > initialLength)
            {
                await using var stream = new FileStream(_factory.SerilogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stream.Seek(initialLength, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);

                return await reader.ReadToEndAsync();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        return string.Empty;
    }
}
