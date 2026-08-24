using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApiCoreSeed.IntegrationTests.Infrastructure;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

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

    [Fact(DisplayName = "LogEntry preserva tabela legada Loggin no SQL Server")]
    public async Task LogEntryQuandoPersistidoDeveUsarTabelaLegadaLoggin()
    {
        await _factory.ResetStateAsync();
        var logEntry = new LogEntry
        {
            Id = Guid.NewGuid(),
            EventId = 42,
            Escopo = "Compatibility",
            LogLevel = ELogLevel.Information,
            Message = "Evento de compatibilidade persistido",
            CreatedTime = DateTime.UtcNow
        };

        await _factory.WithDomainContextAsync(async context =>
        {
            context.LogEntries.Add(logEntry);
            await context.SaveChangesAsync();
        });

        await using var connection = new SqlConnection(_factory.SqlServerConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT_BIG(*) FROM [dbo].[Loggin] WHERE [Id] = @Id AND [Message] = @Message;";
        command.Parameters.AddWithValue("@Id", logEntry.Id);
        command.Parameters.AddWithValue("@Message", logEntry.Message);

        var rows = Convert.ToInt64(await command.ExecuteScalarAsync(), System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(1, rows);

        var persisted = await _factory.WithDomainContextAsync(context =>
            context.LogEntries.AsNoTracking().SingleAsync(item => item.Id == logEntry.Id));

        Assert.Equal(logEntry.Message, persisted.Message);
    }

    [Fact(DisplayName = "Unit of Work confirma criacao registrada por repositorio")]
    public async Task UnitOfWorkQuandoRepositorioRegistraCriacaoDevePersistir()
    {
        await _factory.ResetStateAsync();
        var prato = TestData.CreatePrato("UoW criacao");

        using (var scope = _factory.Services.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IPratoRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<ISampleRestaurantUnitOfWork>();

            await repository.Adicionar(prato);
            var changes = await unitOfWork.CommitAsync();

            Assert.True(changes > 0);
        }

        var persisted = await _factory.WithDomainContextAsync(context =>
            context.Pratos.AsNoTracking().SingleAsync(item => item.Id == prato.Id));

        Assert.Equal("UoW criacao", persisted.Titulo);
    }

    [Fact(DisplayName = "Unit of Work confirma atualizacao registrada por repositorio")]
    public async Task UnitOfWorkQuandoRepositorioRegistraAtualizacaoDevePersistir()
    {
        await _factory.ResetStateAsync();
        var prato = TestData.CreatePrato("UoW antes");

        await _factory.WithDomainContextAsync(async context =>
        {
            context.Pratos.Add(prato);
            await context.SaveChangesAsync();
        });

        using (var scope = _factory.Services.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IPratoRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<ISampleRestaurantUnitOfWork>();
            var persisted = await repository.ObterPorId(prato.Id);
            Assert.NotNull(persisted);
            persisted.Titulo = "UoW depois";

            await repository.Atualizar(persisted);
            await unitOfWork.CommitAsync();
        }

        var updated = await _factory.WithDomainContextAsync(context =>
            context.Pratos.AsNoTracking().SingleAsync(item => item.Id == prato.Id));

        Assert.Equal("UoW depois", updated.Titulo);
    }

    [Fact(DisplayName = "Repositorio sem commit nao persiste alteracao")]
    public async Task RepositorioQuandoEscopoTerminaSemCommitNaoDevePersistir()
    {
        await _factory.ResetStateAsync();
        var prato = TestData.CreatePrato("Sem commit");

        using (var scope = _factory.Services.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IPratoRepository>();

            await repository.Adicionar(prato);
        }

        var exists = await _factory.WithDomainContextAsync(context =>
            context.Pratos.AnyAsync(item => item.Id == prato.Id));

        Assert.False(exists);
    }

    [Fact(DisplayName = "Unit of Work reverte alteracoes atomicas quando commit falha")]
    public async Task UnitOfWorkQuandoCommitFalhaNaoDevePersistirParcialmente()
    {
        await _factory.ResetStateAsync();
        var mesa = TestData.CreateMesa("UOW-TX");
        var pedido = new Pedido
        {
            AtendenteId = Guid.NewGuid(),
            MesaId = Guid.NewGuid(),
            Numero = "PED-UOW-FAIL",
            DataHoraCadastro = DateTime.UtcNow
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var mesaRepository = scope.ServiceProvider.GetRequiredService<IMesaRepository>();
            var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<ISampleRestaurantUnitOfWork>();

            await mesaRepository.Adicionar(mesa);
            await pedidoRepository.Adicionar(pedido);

            await Assert.ThrowsAsync<DbUpdateException>(() => unitOfWork.CommitAsync());
        }

        var mesaExists = await _factory.WithDomainContextAsync(context =>
            context.Mesas.AnyAsync(item => item.Id == mesa.Id));
        var pedidoExists = await _factory.WithDomainContextAsync(context =>
            context.Pedidos.AnyAsync(item => item.Id == pedido.Id));

        Assert.False(mesaExists);
        Assert.False(pedidoExists);
    }

    [Fact(DisplayName = "Unit of Work com commit cancelado nao persiste alteracao")]
    public async Task UnitOfWorkQuandoCommitCanceladoNaoDevePersistir()
    {
        await _factory.ResetStateAsync();
        var mesa = TestData.CreateMesa("UOW-CANCEL");

        using (var scope = _factory.Services.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IMesaRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<ISampleRestaurantUnitOfWork>();
            using var cancellationTokenSource = new CancellationTokenSource();

            await repository.Adicionar(mesa);
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                unitOfWork.CommitAsync(cancellationTokenSource.Token));
        }

        var exists = await _factory.WithDomainContextAsync(context =>
            context.Mesas.AnyAsync(item => item.Id == mesa.Id));

        Assert.False(exists);
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
                UserName = "duplicado@example.local",
                NormalizedUserName = "DUPLICADO@EXAMPLE.LOCAL",
                Email = "duplicado-1@example.local",
                NormalizedEmail = "DUPLICADO-1@RESTAURANTE.LOCAL",
                EmailConfirmed = true
            });
            context.Users.Add(new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "duplicado@example.local",
                NormalizedUserName = "DUPLICADO@EXAMPLE.LOCAL",
                Email = "duplicado-2@example.local",
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
