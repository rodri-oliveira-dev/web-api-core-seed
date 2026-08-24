using Microsoft.Extensions.DependencyInjection;
using WebApiCoreSeed.IntegrationTests.Infrastructure;
using WebApiCoreSeed.SampleRestaurant.Interfaces;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Service;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

[Collection(ApiIntegrationFixtureDefinition.Name)]
[Trait("Category", IntegrationCategories.Integration)]
public sealed class CompositionRootIntegrationTests
{
    private readonly ApiFactory _factory;

    public CompositionRootIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Composition root resolve dependencias do SampleRestaurant em um escopo")]
    public void CompositionRootQuandoAplicacaoInicializaDeveResolverDependenciasScoped()
    {
        using var scope = _factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        Assert.NotNull(provider.GetRequiredService<SampleRestaurantDbContext>());
        Assert.NotNull(provider.GetRequiredService<IAtendenteRepository>());
        Assert.NotNull(provider.GetRequiredService<ILogEntryRepository>());
        Assert.NotNull(provider.GetRequiredService<IMesaRepository>());
        Assert.NotNull(provider.GetRequiredService<IPedidoPratoRepository>());
        Assert.NotNull(provider.GetRequiredService<IPedidoRepository>());
        Assert.NotNull(provider.GetRequiredService<IPratoRepository>());
        Assert.NotNull(provider.GetRequiredService<ISampleRestaurantUnitOfWork>());
        Assert.NotNull(provider.GetRequiredService<IAtendenteService>());
        Assert.NotNull(provider.GetRequiredService<ILogEntryService>());
        Assert.NotNull(provider.GetRequiredService<IMesaService>());
        Assert.NotNull(provider.GetRequiredService<IPedidoPratoService>());
        Assert.NotNull(provider.GetRequiredService<IPedidoService>());
        Assert.NotNull(provider.GetRequiredService<IPratoService>());
        Assert.NotNull(provider.GetRequiredService<INotificador>());
    }
}
