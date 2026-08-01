using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Restaurante.IO.Business.Models;
using WebApiCoreSeed.IntegrationTests.Infrastructure;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

[Collection(ApiIntegrationFixtureDefinition.Name)]
[Trait("Category", IntegrationCategories.Integration)]
[Trait("Category", IntegrationCategories.Container)]
public sealed class SqlServerIntegrationTests
{
    private readonly ApiFactory _factory;

    public SqlServerIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Migrations podem ser aplicadas em banco SQL Server vazio")]
    public async Task MigrationsQuandoBancoVazioDeveCriarSchema()
    {
        await _factory.ResetStateAsync();

        await _factory.WithIdentityContextAsync(async context =>
        {
            var migrations = await context.Database.GetAppliedMigrationsAsync();
            Assert.Contains("20200817223121_InitialCreate", migrations);
            Assert.True(await context.Users.AnyAsync() == false);
        });

        await _factory.WithDomainContextAsync(async context =>
        {
            var migrations = await context.Database.GetAppliedMigrationsAsync();
            Assert.Contains("20200817223231_InitialCreate", migrations);
            Assert.True(await context.Pratos.AnyAsync() == false);
        });
    }

    [Fact(DisplayName = "Entidade de dominio persiste e pode ser consultada no SQL Server")]
    public async Task PratoQuandoPersistidoDeveSerConsultado()
    {
        await _factory.ResetStateAsync();
        var prato = TestData.CreatePrato("Moqueca de teste");

        await _factory.WithDomainContextAsync(async context =>
        {
            context.Pratos.Add(prato);
            await context.SaveChangesAsync();
        });

        var persisted = await _factory.WithDomainContextAsync(context =>
            context.Pratos.AsNoTracking().SingleAsync(item => item.Id == prato.Id));

        Assert.Equal("Moqueca de teste", persisted.Titulo);
        Assert.True(persisted.Ativo);
    }

    [Fact(DisplayName = "Constraint de chave estrangeira e aplicada pelo SQL Server")]
    public async Task PedidoQuandoReferenciasNaoExistemDeveFalharPorConstraint()
    {
        await _factory.ResetStateAsync();

        await _factory.WithDomainContextAsync(async context =>
        {
            context.Pedidos.Add(new Pedido
            {
                AtendenteId = Guid.NewGuid(),
                MesaId = Guid.NewGuid(),
                Numero = "PED-001",
                DataHoraCadastro = DateTime.UtcNow
            });

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        });
    }

    [Fact(DisplayName = "Indice unico do Identity e aplicado no SQL Server")]
    public async Task UsuarioQuandoNomeNormalizadoDuplicadoDeveFalharPorIndiceUnico()
    {
        await _factory.ResetStateAsync();

        await _factory.WithIdentityContextAsync(async context =>
        {
            context.Users.Add(new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "duplicado@restaurante.local",
                NormalizedUserName = "DUPLICADO@RESTAURANTE.LOCAL",
                Email = "duplicado-1@restaurante.local",
                NormalizedEmail = "DUPLICADO-1@RESTAURANTE.LOCAL",
                EmailConfirmed = true
            });
            context.Users.Add(new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "duplicado@restaurante.local",
                NormalizedUserName = "DUPLICADO@RESTAURANTE.LOCAL",
                Email = "duplicado-2@restaurante.local",
                NormalizedEmail = "DUPLICADO-2@RESTAURANTE.LOCAL",
                EmailConfirmed = true
            });

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        });
    }

    [Fact(DisplayName = "Transacao representativa pode ser revertida no SQL Server")]
    public async Task MesaQuandoTransacaoRevertidaNaoDevePersistir()
    {
        await _factory.ResetStateAsync();
        var mesa = TestData.CreateMesa("TX-01");

        await _factory.WithDomainContextAsync(async context =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            context.Mesas.Add(mesa);
            await context.SaveChangesAsync();
            await transaction.RollbackAsync();
        });

        var exists = await _factory.WithDomainContextAsync(context =>
            context.Mesas.AnyAsync(item => item.Id == mesa.Id));

        Assert.False(exists);
    }

    [Fact(DisplayName = "Funcao nativa confirma comportamento real do SQL Server")]
    public async Task SqlServerQuandoExecutaFuncaoNativaDeveRetornarResultado()
    {
        await _factory.ResetStateAsync();

        await using var connection = new SqlConnection(_factory.SqlServerConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT DATEDIFF(second, SYSUTCDATETIME(), DATEADD(second, 5, SYSUTCDATETIME()))";

        var result = await command.ExecuteScalarAsync();

        Assert.Equal(5, Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
    }
}
