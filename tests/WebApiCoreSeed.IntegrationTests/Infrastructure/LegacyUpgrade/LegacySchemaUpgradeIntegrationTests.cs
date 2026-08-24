using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.IntegrationTests.Infrastructure;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure.LegacyUpgrade;

[Collection(LegacySchemaUpgradeFixtureDefinition.Name)]
[Trait("Category", IntegrationCategories.Integration)]
[Trait("Category", IntegrationCategories.Container)]
public sealed class LegacySchemaUpgradeIntegrationTests
{
    private const string IdentityInitialMigration = "20200817223121_InitialCreate";
    private const string SampleRestaurantInitialMigration = "20200817223231_InitialCreate";
    private const string PaginationOrderingMigration = "20260801191447_AddPratosPaginationOrderingIndex";

    private static readonly Guid LegacyAtendenteId = Guid.Parse("8b5d2e57-289c-41a4-9ba4-5d0d2c6a3b10");
    private static readonly Guid LegacyMesaId = Guid.Parse("3f5f3ab5-85ec-4db8-b9d7-3d0ce1a63711");
    private static readonly Guid LegacyPratoId = Guid.Parse("76e5630a-1303-4857-aeb9-b47af22f88b1");
    private static readonly Guid LegacyPedidoId = Guid.Parse("84236b27-2696-44b1-a478-04f2d4a34b21");
    private static readonly Guid LegacyPedidoPratoId = Guid.Parse("2b6f6034-42c1-49bd-b5d1-1966e051a1d3");
    private static readonly Guid LegacyLogId = Guid.Parse("7c872d22-bc3f-4bb2-986d-38192a9aa2ff");
    private static readonly Guid CurrentPratoId = Guid.Parse("f05110f5-bae8-49c6-8f41-cce3edc275e6");

    private readonly LegacySchemaUpgradeFixture _fixture;

    public LegacySchemaUpgradeIntegrationTests(LegacySchemaUpgradeFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Migrations atuais atualizam schema legado preservando dados")]
    public async Task MigrationsQuandoSchemaLegadoExisteDevemAplicarSomenteUpgradeAtual()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        var cancellationToken = cancellationTokenSource.Token;

        await _fixture.ApplyLegacySchemaAsync(cancellationToken);

        var migrationsBeforeUpgrade = await GetMigrationCountsAsync(cancellationToken);
        Assert.Equal(2, migrationsBeforeUpgrade.Count);
        Assert.Single(migrationsBeforeUpgrade, item => item.MigrationId == IdentityInitialMigration && item.Count == 1);
        Assert.Single(migrationsBeforeUpgrade, item => item.MigrationId == SampleRestaurantInitialMigration && item.Count == 1);
        Assert.False(await IndexExistsAsync("Pratos", "IX_Pratos_Titulo_Id", cancellationToken));

        await InsertLegacyDataAsync(cancellationToken);

        await using (var identityContext = _fixture.CreateApplicationContext())
        {
            await identityContext.Database.MigrateAsync(cancellationToken);
        }

        await using (var sampleContext = _fixture.CreateSampleRestaurantContext())
        {
            await sampleContext.Database.MigrateAsync(cancellationToken);
        }

        var migrationsAfterUpgrade = await GetMigrationCountsAsync(cancellationToken);
        Assert.Equal(3, migrationsAfterUpgrade.Count);
        Assert.All(migrationsAfterUpgrade, item => Assert.Equal(1, item.Count));
        Assert.Contains(migrationsAfterUpgrade, item => item.MigrationId == PaginationOrderingMigration);

        await AssertLegacyDataWasPreservedAsync(cancellationToken);
        await AssertCurrentContextsCanUseUpgradedDatabaseAsync(cancellationToken);
        await AssertSchemaObjectsAsync(cancellationToken);

        await using (var identityContext = _fixture.CreateApplicationContext())
        {
            await identityContext.Database.MigrateAsync(cancellationToken);
        }

        await using (var sampleContext = _fixture.CreateSampleRestaurantContext())
        {
            await sampleContext.Database.MigrateAsync(cancellationToken);
        }

        var migrationsAfterSecondUpgrade = await GetMigrationCountsAsync(cancellationToken);
        Assert.Equal(migrationsAfterUpgrade.OrderBy(item => item.MigrationId), migrationsAfterSecondUpgrade.OrderBy(item => item.MigrationId));
        Assert.True(await IndexExistsAsync("Pratos", "IX_Pratos_Titulo_Id", cancellationToken));
    }

    private async Task InsertLegacyDataAsync(CancellationToken cancellationToken)
    {
        await using var connection = await _fixture.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await ExecuteNonQueryAsync(
            connection,
            transaction,
            """
            INSERT INTO [dbo].[AspNetUsers]
                ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed],
                 [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed],
                 [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
            VALUES
                (@UserId, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, 1,
                 NULL, @SecurityStamp, @ConcurrencyStamp, NULL, 0, 0, NULL, 1, 0);
            """,
            cancellationToken,
            ("@UserId", "legacy-user-001"),
            ("@UserName", "legacy.user@example.test"),
            ("@NormalizedUserName", "LEGACY.USER@EXAMPLE.TEST"),
            ("@Email", "legacy.user@example.test"),
            ("@NormalizedEmail", "LEGACY.USER@EXAMPLE.TEST"),
            ("@SecurityStamp", "legacy-security-stamp"),
            ("@ConcurrencyStamp", "legacy-concurrency-stamp"));

        await ExecuteNonQueryAsync(
            connection,
            transaction,
            """
            INSERT INTO [dbo].[Atendentes] ([Id], [Nome], [TipoAtendente])
            VALUES (@AtendenteId, @Nome, 0);

            INSERT INTO [dbo].[Mesas] ([Id], [Numero], [Lugares], [Ativo], [LocalizacaoMesa])
            VALUES (@MesaId, @NumeroMesa, 4, 1, 0);

            INSERT INTO [dbo].[Pratos] ([Id], [Titulo], [Descricao], [Foto], [Preco], [Ativo], [TipoPrato])
            VALUES (@PratoId, @TituloPrato, @DescricaoPrato, @FotoPrato, 42.5, 1, 0);

            INSERT INTO [dbo].[Pedidos] ([Id], [AtendenteId], [MesaId], [Numero], [DataHoraCadastro], [DataHoraEncerrado])
            VALUES (@PedidoId, @AtendenteId, @MesaId, @NumeroPedido, @DataPedido, NULL);

            INSERT INTO [dbo].[PedidoPrato] ([Id], [PedidoId], [PratoId], [StatusProducao], [Observacao])
            VALUES (@PedidoPratoId, @PedidoId, @PratoId, 1, @Observacao);

            INSERT INTO [dbo].[Loggin] ([Id], [EventId], [Escopo], [LogLevel], [Message], [CreatedTime])
            VALUES (@LogId, 18, @Escopo, 2, @Message, @DataPedido);
            """,
            cancellationToken,
            ("@AtendenteId", LegacyAtendenteId),
            ("@Nome", "Atendente legado"),
            ("@MesaId", LegacyMesaId),
            ("@NumeroMesa", "M-LEG-01"),
            ("@PratoId", LegacyPratoId),
            ("@TituloPrato", "Caldo legado"),
            ("@DescricaoPrato", "Dado criado antes do upgrade"),
            ("@FotoPrato", "caldo-legado.jpg"),
            ("@PedidoId", LegacyPedidoId),
            ("@NumeroPedido", "PED-LEG-01"),
            ("@DataPedido", new DateTime(2020, 8, 18, 12, 30, 0)),
            ("@PedidoPratoId", LegacyPedidoPratoId),
            ("@Observacao", "Sem cebola"),
            ("@LogId", LegacyLogId),
            ("@Escopo", "LegacyUpgrade"),
            ("@Message", "Evento legado preservado"));

        await transaction.CommitAsync(cancellationToken);
    }

    private async Task AssertLegacyDataWasPreservedAsync(CancellationToken cancellationToken)
    {
        await using var connection = await _fixture.OpenConnectionAsync(cancellationToken);

        Assert.Equal(1, await CountRowsAsync(connection, "AspNetUsers", cancellationToken));
        Assert.Equal(1, await CountRowsAsync(connection, "Atendentes", cancellationToken));
        Assert.Equal(1, await CountRowsAsync(connection, "Mesas", cancellationToken));
        Assert.Equal(1, await CountRowsAsync(connection, "Pratos", cancellationToken));
        Assert.Equal(1, await CountRowsAsync(connection, "Pedidos", cancellationToken));
        Assert.Equal(1, await CountRowsAsync(connection, "PedidoPrato", cancellationToken));
        Assert.Equal(1, await CountRowsAsync(connection, "Loggin", cancellationToken));
    }

    private async Task AssertCurrentContextsCanUseUpgradedDatabaseAsync(CancellationToken cancellationToken)
    {
        await using (var identityContext = _fixture.CreateApplicationContext())
        {
            var user = await identityContext.Users.AsNoTracking().SingleAsync(item => item.Id == "legacy-user-001", cancellationToken);
            Assert.Equal("legacy.user@example.test", user.Email);
        }

        await using var sampleContext = _fixture.CreateSampleRestaurantContext();
        var prato = await sampleContext.Pratos.AsNoTracking().SingleAsync(item => item.Id == LegacyPratoId, cancellationToken);
        var pedidoPrato = await sampleContext.PedidoPrato
            .AsNoTracking()
            .SingleAsync(item => item.Id == LegacyPedidoPratoId, cancellationToken);

        Assert.Equal("Caldo legado", prato.Titulo);
        Assert.Equal(LegacyPedidoId, pedidoPrato.PedidoId);
        Assert.Equal(LegacyPratoId, pedidoPrato.PratoId);

        sampleContext.Pratos.Add(new Prato
        {
            Id = CurrentPratoId,
            Titulo = "Prato atual apos upgrade",
            Descricao = "Persistido pelo DbContext atual",
            Foto = "prato-atual.jpg",
            Preco = 55.75,
            Ativo = true,
            TipoPrato = ETipoPrato.Comida
        });

        await sampleContext.SaveChangesAsync(cancellationToken);

        var currentPratoExists = await sampleContext.Pratos
            .AsNoTracking()
            .AnyAsync(item => item.Id == CurrentPratoId, cancellationToken);

        Assert.True(currentPratoExists);
    }

    private async Task AssertSchemaObjectsAsync(CancellationToken cancellationToken)
    {
        Assert.True(await TableExistsAsync("__EFMigrationsHistory", cancellationToken));
        Assert.True(await TableExistsAsync("AspNetUsers", cancellationToken));
        Assert.True(await TableExistsAsync("Pratos", cancellationToken));
        Assert.True(await IndexExistsAsync("Pratos", "IX_Pratos_Titulo_Id", cancellationToken));
        Assert.Equal(["Titulo", "Id"], await GetIndexColumnsAsync("Pratos", "IX_Pratos_Titulo_Id", cancellationToken));

        Assert.True(await IndexExistsAsync("PedidoPrato", "IX_PedidoPrato_PedidoId", cancellationToken));
        Assert.True(await IndexExistsAsync("PedidoPrato", "IX_PedidoPrato_PratoId", cancellationToken));
        Assert.True(await IndexExistsAsync("Pedidos", "IX_Pedidos_AtendenteId", cancellationToken));
        Assert.True(await IndexExistsAsync("Pedidos", "IX_Pedidos_MesaId", cancellationToken));
        Assert.True(await IndexExistsAsync("AspNetUsers", "UserNameIndex", cancellationToken));

        Assert.True(await ConstraintExistsAsync("FK_Pedidos_Atendentes", cancellationToken));
        Assert.True(await ConstraintExistsAsync("FK_Pedidos_Mesas", cancellationToken));
        Assert.True(await ConstraintExistsAsync("FK_PedidoPrato_Pedidos", cancellationToken));
        Assert.True(await ConstraintExistsAsync("FK_PedidoPrato_Pratos", cancellationToken));
    }

    private async Task<List<(string MigrationId, int Count)>> GetMigrationCountsAsync(CancellationToken cancellationToken)
    {
        await using var connection = await _fixture.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT [MigrationId], COUNT_BIG(*)
            FROM [dbo].[__EFMigrationsHistory]
            GROUP BY [MigrationId]
            ORDER BY [MigrationId];
            """;

        var migrations = new List<(string MigrationId, int Count)>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            migrations.Add((reader.GetString(0), Convert.ToInt32(reader.GetInt64(1))));
        }

        return migrations;
    }

    private async Task<bool> TableExistsAsync(string tableName, CancellationToken cancellationToken)
    {
        return await ScalarExistsAsync(
            """
            SELECT COUNT_BIG(*)
            FROM sys.tables
            WHERE object_id = OBJECT_ID(@ObjectName);
            """,
            cancellationToken,
            ("@ObjectName", $"dbo.{tableName}"));
    }

    private async Task<bool> IndexExistsAsync(string tableName, string indexName, CancellationToken cancellationToken)
    {
        return await ScalarExistsAsync(
            """
            SELECT COUNT_BIG(*)
            FROM sys.indexes
            WHERE object_id = OBJECT_ID(@ObjectName)
              AND name = @IndexName;
            """,
            cancellationToken,
            ("@ObjectName", $"dbo.{tableName}"),
            ("@IndexName", indexName));
    }

    private async Task<bool> ConstraintExistsAsync(string constraintName, CancellationToken cancellationToken)
    {
        return await ScalarExistsAsync(
            """
            SELECT COUNT_BIG(*)
            FROM sys.foreign_keys
            WHERE name = @ConstraintName;
            """,
            cancellationToken,
            ("@ConstraintName", constraintName));
    }

    private async Task<List<string>> GetIndexColumnsAsync(string tableName, string indexName, CancellationToken cancellationToken)
    {
        await using var connection = await _fixture.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT columns.name
            FROM sys.indexes indexes
            INNER JOIN sys.index_columns index_columns
                ON indexes.object_id = index_columns.object_id
                AND indexes.index_id = index_columns.index_id
            INNER JOIN sys.columns columns
                ON index_columns.object_id = columns.object_id
                AND index_columns.column_id = columns.column_id
            WHERE indexes.object_id = OBJECT_ID(@ObjectName)
              AND indexes.name = @IndexName
            ORDER BY index_columns.key_ordinal;
            """;
        command.Parameters.AddWithValue("@ObjectName", $"dbo.{tableName}");
        command.Parameters.AddWithValue("@IndexName", indexName);

        var columns = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            columns.Add(reader.GetString(0));
        }

        return columns;
    }

    private async Task<bool> ScalarExistsAsync(
        string commandText,
        CancellationToken cancellationToken,
        params (string Name, object Value)[] parameters)
    {
        await using var connection = await _fixture.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        AddParameters(command, parameters);

        var count = Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken), CultureInfo.InvariantCulture);
        return count > 0;
    }

    private static async Task<int> CountRowsAsync(SqlConnection connection, string tableName, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = tableName switch
        {
            "AspNetUsers" => "SELECT COUNT_BIG(*) FROM [dbo].[AspNetUsers];",
            "Atendentes" => "SELECT COUNT_BIG(*) FROM [dbo].[Atendentes];",
            "Mesas" => "SELECT COUNT_BIG(*) FROM [dbo].[Mesas];",
            "Pratos" => "SELECT COUNT_BIG(*) FROM [dbo].[Pratos];",
            "Pedidos" => "SELECT COUNT_BIG(*) FROM [dbo].[Pedidos];",
            "PedidoPrato" => "SELECT COUNT_BIG(*) FROM [dbo].[PedidoPrato];",
            "Loggin" => "SELECT COUNT_BIG(*) FROM [dbo].[Loggin];",
            _ => throw new ArgumentOutOfRangeException(nameof(tableName), tableName, "Tabela nao suportada pelo teste.")
        };

        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken), CultureInfo.InvariantCulture);
    }

    private static async Task ExecuteNonQueryAsync(
        SqlConnection connection,
        System.Data.Common.DbTransaction transaction,
        string commandText,
        CancellationToken cancellationToken,
        params (string Name, object Value)[] parameters)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = (SqlTransaction)transaction;
        command.CommandText = commandText;
        AddParameters(command, parameters);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddParameters(SqlCommand command, params (string Name, object Value)[] parameters)
    {
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Name, parameter.Value);
        }
    }
}
