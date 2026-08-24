using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.Identity.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using Testcontainers.MsSql;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure.LegacyUpgrade;

public sealed class LegacySchemaUpgradeFixture : IAsyncLifetime
{
    private const string SqlServerPassword = "P@ssw0rd-Integration-2026!";
    private static readonly TimeSpan SqlServerStartupTimeout = TimeSpan.FromMinutes(2);

    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder(ApiFactory.SqlServerImage)
        .WithPassword(SqlServerPassword)
        .Build();

    private string? _connectionString;

    public string ConnectionString => _connectionString ?? BuildConnectionString();

    public ApplicationDbContext CreateApplicationContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                ConnectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .Options;

        return new ApplicationDbContext(options);
    }

    public SampleRestaurantDbContext CreateSampleRestaurantContext()
    {
        var options = new DbContextOptionsBuilder<SampleRestaurantDbContext>()
            .UseSqlServer(
                ConnectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(SampleRestaurantDbContext).Assembly.FullName))
            .Options;

        return new SampleRestaurantDbContext(options);
    }

    public async Task ApplyLegacySchemaAsync(CancellationToken cancellationToken)
    {
        var scriptPath = Path.Combine(
            AppContext.BaseDirectory,
            "Infrastructure",
            "LegacyUpgrade",
            "legacy-schema-baseline.sql");

        var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var batch in SplitBatches(script))
        {
            await using var command = new SqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        using var cancellationTokenSource = new CancellationTokenSource(SqlServerStartupTimeout);

        await _sqlServer.StartAsync(cancellationTokenSource.Token);
        _connectionString = BuildConnectionString();
        await WaitUntilSqlServerAcceptsConnectionsAsync(cancellationTokenSource.Token);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _sqlServer.DisposeAsync().AsTask();
    }

    private string BuildConnectionString()
    {
        var builder = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            ConnectTimeout = 5,
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }

    private async Task WaitUntilSqlServerAcceptsConnectionsAsync(CancellationToken cancellationToken)
    {
        var deadline = DateTimeOffset.UtcNow.Add(SqlServerStartupTimeout);
        Exception? lastException = null;

        while (DateTimeOffset.UtcNow < deadline)
        {
            try
            {
                await using var connection = new SqlConnection(ConnectionString);
                await connection.OpenAsync(cancellationToken);
                return;
            }
            catch (SqlException ex)
            {
                lastException = ex;
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        throw new InvalidOperationException("SQL Server container nao aceitou conexao dentro do timeout configurado.", lastException);
    }

    private static IEnumerable<string> SplitBatches(string script)
    {
        using var reader = new StringReader(script);
        var batch = new StringBuilder();

        while (reader.ReadLine() is { } line)
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                var commandText = batch.ToString().Trim();
                if (commandText.Length > 0)
                {
                    yield return commandText;
                }

                batch.Clear();
                continue;
            }

            batch.AppendLine(line);
        }

        var finalCommandText = batch.ToString().Trim();
        if (finalCommandText.Length > 0)
        {
            yield return finalCommandText;
        }
    }
}
