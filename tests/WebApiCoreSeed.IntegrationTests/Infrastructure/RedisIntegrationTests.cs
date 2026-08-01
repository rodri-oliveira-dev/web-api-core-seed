using StackExchange.Redis;
using WebApiCoreSeed.IntegrationTests.Infrastructure;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

[Collection(ApiIntegrationFixtureDefinition.Name)]
[Trait("Category", IntegrationCategories.Integration)]
[Trait("Category", IntegrationCategories.Container)]
public sealed class RedisIntegrationTests
{
    private readonly ApiFactory _factory;

    public RedisIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Redis escreve e le chave")]
    public async Task RedisQuandoChaveEscritaDeveRetornarValor()
    {
        await _factory.ResetStateAsync();
        var key = $"redis:read-write:{Guid.NewGuid():N}";

        var value = await _factory.WithRedisAsync(async database =>
        {
            await database.StringSetAsync(key, "valor-integracao");
            return await database.StringGetAsync(key);
        });

        Assert.Equal("valor-integracao", value.ToString());
    }

    [Fact(DisplayName = "Redis retorna nulo quando chave nao existe")]
    public async Task RedisQuandoChaveNaoExisteDeveRetornarNulo()
    {
        await _factory.ResetStateAsync();

        var value = await _factory.WithRedisAsync(database =>
            database.StringGetAsync($"redis:missing:{Guid.NewGuid():N}"));

        Assert.True(value.IsNull);
    }

    [Fact(DisplayName = "Redis respeita expiracao configurada para chave")]
    public async Task RedisQuandoChaveExpiraDeveRemoverValor()
    {
        await _factory.ResetStateAsync();
        var key = $"redis:expiration:{Guid.NewGuid():N}";

        await _factory.WithRedisAsync(database =>
            database.StringSetAsync(key, "expira", TimeSpan.FromSeconds(1)));

        var expired = await WaitUntilExpiredAsync(key);

        Assert.True(expired);
    }

    private async Task<bool> WaitUntilExpiredAsync(string key)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        while (!timeout.IsCancellationRequested)
        {
            var value = await _factory.WithRedisAsync(database => database.StringGetAsync(key));
            if (value == RedisValue.Null)
            {
                return true;
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), timeout.Token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        return false;
    }
}
