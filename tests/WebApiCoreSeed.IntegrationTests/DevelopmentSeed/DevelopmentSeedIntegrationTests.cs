using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApiCoreSeed.Api.DevelopmentSeed;
using WebApiCoreSeed.Api.ViewModels.User;
using WebApiCoreSeed.IntegrationTests.Infrastructure;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.IntegrationTests.DevelopmentSeed;

[Collection(ApiIntegrationFixtureDefinition.Name)]
[Trait("Category", IntegrationCategories.Integration)]
[Trait("Category", IntegrationCategories.Container)]
public sealed class DevelopmentSeedIntegrationTests
{
    private const string DevelopmentUserEmail = "developer@example.local";
    private const string DevelopmentUserPassword = "NotASecret_ForTests_2026!";
    private readonly ApiFactory _factory;

    public DevelopmentSeedIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Seed cria dados iniciais em banco vazio")]
    public async Task SeedQuandoBancoVazioDeveCriarDadosIniciais()
    {
        await _factory.ResetStateAsync();

        var result = await RunSeedAsync();

        Assert.True(result.IdentityChanges > 0);
        Assert.True(result.SampleRestaurantChanges > 0);

        await AssertSeedCountsAsync();
        await _factory.WithIdentityContextAsync(async context =>
        {
            var migrations = await context.Database.GetAppliedMigrationsAsync();
            var user = await context.Users.SingleAsync(user => user.Id == DevelopmentSeedDefinition.UserId);

            Assert.Contains("20200817223121_InitialCreate", migrations);
            Assert.Equal(DevelopmentUserEmail, user.Email);
            Assert.True(user.EmailConfirmed);
        });

        await _factory.WithDomainContextAsync(async context =>
        {
            var migrations = await context.Database.GetAppliedMigrationsAsync();

            Assert.Contains("20200817223231_InitialCreate", migrations);
            Assert.Contains("20260801191447_AddPratosPaginationOrderingIndex", migrations);
        });
    }

    [Fact(DisplayName = "Seed pode executar duas vezes sem duplicar dados")]
    public async Task SeedQuandoExecutadoDuasVezesDeveSerIdempotente()
    {
        await _factory.ResetStateAsync();

        var first = await RunSeedAsync();
        var second = await RunSeedAsync();

        Assert.True(first.IdentityChanges > 0);
        Assert.True(first.SampleRestaurantChanges > 0);
        Assert.Equal(0, second.IdentityChanges);
        Assert.Equal(0, second.SampleRestaurantChanges);
        await AssertSeedCountsAsync();
    }

    [Fact(DisplayName = "Seed completa dados parcialmente existentes")]
    public async Task SeedQuandoDadosParciaisExistemDeveCompletarSemDuplicar()
    {
        await _factory.ResetStateAsync();
        var existingMesa = DevelopmentSeedDefinition.Mesas[0];

        await _factory.WithDomainContextAsync(async context =>
        {
            context.Mesas.Add(new Mesa
            {
                Id = existingMesa.Id,
                Numero = existingMesa.Numero,
                Lugares = existingMesa.Lugares,
                Ativo = existingMesa.Ativo,
                LocalizacaoMesa = existingMesa.LocalizacaoMesa
            });
            await context.SaveChangesAsync();
        });

        await RunSeedAsync();

        await AssertSeedCountsAsync();
    }

    [Fact(DisplayName = "Seed atualiza dado conhecido com seguranca")]
    public async Task SeedQuandoDadoConhecidoFoiAlteradoDeveRestaurarDefinicao()
    {
        await _factory.ResetStateAsync();
        var seedPrato = DevelopmentSeedDefinition.Pratos[0];

        await _factory.WithDomainContextAsync(async context =>
        {
            context.Pratos.Add(new Prato
            {
                Id = seedPrato.Id,
                Titulo = "Titulo local alterado",
                Descricao = "Descricao local alterada",
                Foto = "local.png",
                Preco = 1,
                Ativo = false,
                TipoPrato = ETipoPrato.Bebida
            });
            await context.SaveChangesAsync();
        });

        var result = await RunSeedAsync();

        Assert.True(result.SampleRestaurantChanges > 0);

        var persisted = await _factory.WithDomainContextAsync(context =>
            context.Pratos.AsNoTracking().SingleAsync(prato => prato.Id == seedPrato.Id));

        Assert.Equal(seedPrato.Titulo, persisted.Titulo);
        Assert.Equal(seedPrato.Descricao, persisted.Descricao);
        Assert.Equal(seedPrato.Foto, persisted.Foto);
        Assert.Equal(seedPrato.Preco, persisted.Preco);
        Assert.Equal(seedPrato.Ativo, persisted.Ativo);
        Assert.Equal(seedPrato.TipoPrato, persisted.TipoPrato);
    }

    [Fact(DisplayName = "Seed preserva dados criados pelo usuario")]
    public async Task SeedQuandoExistemDadosDoUsuarioDevePreservaLos()
    {
        await _factory.ResetStateAsync();
        var userPratoId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");

        await _factory.WithDomainContextAsync(async context =>
        {
            context.Pratos.Add(new Prato
            {
                Id = userPratoId,
                Titulo = "Prato criado pelo usuario",
                Descricao = "Dado fora do inventario do seed.",
                Foto = "usuario.png",
                Preco = 99,
                Ativo = true,
                TipoPrato = ETipoPrato.Comida
            });
            await context.SaveChangesAsync();
        });

        await RunSeedAsync();

        var exists = await _factory.WithDomainContextAsync(context =>
            context.Pratos.AnyAsync(prato => prato.Id == userPratoId && prato.Titulo == "Prato criado pelo usuario"));

        Assert.True(exists);
    }

    [Fact(DisplayName = "Seed respeita cancelamento antes de gravar dados")]
    public async Task SeedQuandoCanceladoDeveFalharSemGravarDados()
    {
        await _factory.ResetStateAsync();

        using var scope = _factory.Services.CreateScope();
        using var cancellationTokenSource = new CancellationTokenSource();
        var runner = scope.ServiceProvider.GetRequiredService<DevelopmentSeedRunner>();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            runner.RunAsync(cancellationTokenSource.Token));

        await _factory.WithIdentityContextAsync(async context =>
            Assert.False(await context.Users.AnyAsync(user => user.Id == DevelopmentSeedDefinition.UserId)));
        await _factory.WithDomainContextAsync(async context =>
            Assert.False(await context.Pratos.AnyAsync(prato =>
                DevelopmentSeedDefinition.Pratos.Select(seed => seed.Id).Contains(prato.Id))));
    }

    [Fact(DisplayName = "Usuario de desenvolvimento faz login e acessa endpoint protegido")]
    public async Task SeedQuandoExecutadoDevePermitirLoginEAcessoAutenticado()
    {
        await _factory.ResetStateAsync();
        await RunSeedAsync();

        using var client = _factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("X-ClientId", $"seed-login-{Guid.NewGuid():N}");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/entrar", new LoginUserViewModel
        {
            Email = DevelopmentUserEmail,
            Password = DevelopmentUserPassword
        });
        using var loginDocument = await JsonDocument.ParseAsync(await loginResponse.Content.ReadAsStreamAsync());

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.True(loginDocument.RootElement.GetProperty("success").GetBoolean());
        var accessToken = loginDocument.RootElement
            .GetProperty("data")
            .GetProperty("accessToken")
            .GetString();
        Assert.False(string.IsNullOrWhiteSpace(accessToken));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var mesaResponse = await client.GetAsync($"/api/v1/Mesas/{DevelopmentSeedDefinition.Mesas[0].Id}");

        Assert.Equal(HttpStatusCode.OK, mesaResponse.StatusCode);
    }

    private async Task<DevelopmentSeedResult> RunSeedAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<DevelopmentSeedRunner>();

        return await runner.RunAsync();
    }

    private async Task AssertSeedCountsAsync()
    {
        await _factory.WithIdentityContextAsync(async context =>
        {
            Assert.Equal(1, await context.Users.CountAsync(user => user.Id == DevelopmentSeedDefinition.UserId));
            Assert.Equal(
                DevelopmentSeedDefinition.UserClaims.Count,
                await context.UserClaims.CountAsync(claim => claim.UserId == DevelopmentSeedDefinition.UserId));
        });

        await _factory.WithDomainContextAsync(async context =>
        {
            Assert.Equal(DevelopmentSeedDefinition.Pratos.Count, await CountSeedPratosAsync(context));
            Assert.Equal(DevelopmentSeedDefinition.Mesas.Count, await CountSeedMesasAsync(context));
            Assert.Equal(1, await context.Atendentes.CountAsync(item => item.Id == DevelopmentSeedDefinition.Atendente.Id));
            Assert.Equal(1, await context.Pedidos.CountAsync(item => item.Id == DevelopmentSeedDefinition.Pedido.Id));
            Assert.Equal(
                DevelopmentSeedDefinition.PedidoPratos.Count,
                await context.PedidoPrato.CountAsync(item =>
                    DevelopmentSeedDefinition.PedidoPratos.Select(seed => seed.Id).Contains(item.Id)));
        });
    }

    private static Task<int> CountSeedPratosAsync(WebApiCoreSeed.SampleRestaurant.Infrastructure.Context.SampleRestaurantDbContext context)
    {
        var ids = DevelopmentSeedDefinition.Pratos.Select(prato => prato.Id).ToArray();

        return context.Pratos.CountAsync(prato => ids.Contains(prato.Id));
    }

    private static Task<int> CountSeedMesasAsync(WebApiCoreSeed.SampleRestaurant.Infrastructure.Context.SampleRestaurantDbContext context)
    {
        var ids = DevelopmentSeedDefinition.Mesas.Select(mesa => mesa.Id).ToArray();

        return context.Mesas.CountAsync(mesa => ids.Contains(mesa.Id));
    }
}
