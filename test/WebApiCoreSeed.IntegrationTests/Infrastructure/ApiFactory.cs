using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Restaurante.IO.Api;
using Restaurante.IO.Api.DataContext;
using Restaurante.IO.Data.Context;
using StackExchange.Redis;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";
    public const string RedisImage = "redis:7.4.2-alpine";

    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder(SqlServerImage)
        .WithPassword("P@ssw0rd-Integration-2026!")
        .Build();

    private readonly RedisContainer _redis = new RedisBuilder(RedisImage)
        .Build();

    private string? _sqlServerConnectionString;
    private string? _redisConnectionString;
    private ConnectionMultiplexer? _redisConnection;
    private DatabaseReset? _databaseReset;
    private Dictionary<string, string?>? _previousEnvironmentValues;
    private readonly string _serilogFilePath = Path.Combine(
        Path.GetTempPath(),
        "web-api-core-seed",
        $"integration-serilog-{Guid.NewGuid():N}.log");

    public string SqlServerConnectionString => _sqlServerConnectionString ?? BuildSqlServerConnectionString();

    public string RedisConnectionString => _redisConnectionString ?? _redis.GetConnectionString();

    public string SerilogFilePath => _serilogFilePath;

    public HttpClient CreateApiClient(params (string Type, string Value)[] claims)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(AuthenticationHelper.Audience)
        });

        if (claims.Length > 0)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AuthenticationHelper.CreateToken(claims));
        }

        return client;
    }

    public async Task ResetStateAsync()
    {
        if (_databaseReset is null || _redisConnection is null)
        {
            throw new InvalidOperationException("A infraestrutura de teste ainda nao foi inicializada.");
        }

        await _databaseReset.ResetAsync();
        await _redisConnection.GetDatabase().ExecuteAsync("FLUSHDB");
    }

    public async Task WithDomainContextAsync(Func<MeuDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MeuDbContext>();

        await action(context);
    }

    public async Task WithIdentityContextAsync(Func<ApplicationDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await action(context);
    }

    public async Task<T> WithDomainContextAsync<T>(Func<MeuDbContext, Task<T>> action)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MeuDbContext>();

        return await action(context);
    }

    public async Task<T> WithRedisAsync<T>(Func<IDatabase, Task<T>> action)
    {
        if (_redisConnection is null)
        {
            throw new InvalidOperationException("Redis ainda nao foi inicializado.");
        }

        return await action(_redisConnection.GetDatabase());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = SqlServerConnectionString,
                ["AppSettings:Secret"] = AuthenticationHelper.TestSecret,
                ["AppSettings:Emissor"] = AuthenticationHelper.Issuer,
                ["AppSettings:ValidoEm"] = AuthenticationHelper.Audience,
                ["AppSettings:ExpiracaoHoras"] = "2",
                ["RedisCacheSettings:Enabled"] = "true",
                ["RedisCacheSettings:ConnectionString"] = RedisConnectionString,
                ["RedisCacheSettings:InstanceName"] = "integration-tests",
                ["RedisCacheSettings:DefaultSeconds"] = "5",
                ["SeqSettings:Enabled"] = "false",
                ["SeqSettings:Url"] = "http://127.0.0.1:65535",
                ["SeqSettings:FilePath"] = SerilogFilePath,
                ["OpenTelemetry:Enabled"] = "true",
                ["OpenTelemetry:Otlp:Enabled"] = "false",
                ["Cors:AllowedOrigins:0"] = "https://app.example.test",
                ["Cors:AllowedMethods:0"] = "GET",
                ["Cors:AllowedMethods:1"] = "POST",
                ["Cors:AllowedMethods:2"] = "PUT",
                ["Cors:AllowedMethods:3"] = "DELETE",
                ["Cors:AllowedMethods:4"] = "OPTIONS",
                ["Cors:AllowedHeaders:0"] = "Authorization",
                ["Cors:AllowedHeaders:1"] = "Content-Type",
                ["Cors:AllowedHeaders:2"] = "X-ClientId",
                ["Cors:AllowCredentials"] = "false",
                ["ForwardedHeaders:Enabled"] = "false",
                ["RequestLimits:TimeoutSeconds"] = "30",
                ["RequestLimits:MaxRequestBodyBytes"] = "10485760",
                ["NativeRateLimitingSettings:Public:PermitLimit"] = "2",
                ["NativeRateLimitingSettings:Public:WindowSeconds"] = "30",
                ["NativeRateLimitingSettings:Public:QueueLimit"] = "0",
                ["NativeRateLimitingSettings:Authenticated:PermitLimit"] = "2",
                ["NativeRateLimitingSettings:Authenticated:WindowSeconds"] = "30",
                ["NativeRateLimitingSettings:Authenticated:QueueLimit"] = "0",
                ["NativeRateLimitingSettings:AuthenticationSensitive:PermitLimit"] = "2",
                ["NativeRateLimitingSettings:AuthenticationSensitive:WindowSeconds"] = "30",
                ["NativeRateLimitingSettings:AuthenticationSensitive:QueueLimit"] = "0"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.IssuerSigningKey =
                    new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(AuthenticationHelper.TestSecret));
            });
        });
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SerilogFilePath)!);
        await Task.WhenAll(_sqlServer.StartAsync(), _redis.StartAsync());

        _sqlServerConnectionString = BuildSqlServerConnectionString();
        _redisConnectionString = _redis.GetConnectionString();
        SetEnvironmentOverrides();
        _databaseReset = new DatabaseReset(SqlServerConnectionString);
        _redisConnection = await ConnectionMultiplexer.ConnectAsync($"{RedisConnectionString},allowAdmin=true");

        await WaitUntilSqlServerAcceptsConnectionsAsync();
        await ApplyMigrationsAsync();
        await ResetStateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        _redisConnection?.Dispose();
        Dispose();
        RestoreEnvironmentOverrides();

        await _redis.DisposeAsync().AsTask();
        await _sqlServer.DisposeAsync().AsTask();
    }

    private async Task ApplyMigrationsAsync()
    {
        using var scope = Services.CreateScope();

        var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var domainContext = scope.ServiceProvider.GetRequiredService<MeuDbContext>();

        await applicationContext.Database.MigrateAsync();
        await domainContext.Database.MigrateAsync();
    }

    private string BuildSqlServerConnectionString()
    {
        var builder = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            ConnectTimeout = 5,
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }

    private async Task WaitUntilSqlServerAcceptsConnectionsAsync()
    {
        var deadline = DateTimeOffset.UtcNow.AddMinutes(2);
        Exception? lastException = null;

        while (DateTimeOffset.UtcNow < deadline)
        {
            try
            {
                await using var connection = new SqlConnection(SqlServerConnectionString);
                await connection.OpenAsync();
                return;
            }
            catch (SqlException ex)
            {
                lastException = ex;
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        throw new InvalidOperationException("SQL Server container nao aceitou conexao dentro do timeout configurado.", lastException);
    }

    private void SetEnvironmentOverrides()
    {
        var values = CreateConfigurationOverrides();
        _previousEnvironmentValues = values.Keys.ToDictionary(key => key, key => Environment.GetEnvironmentVariable(key));

        foreach (var (key, value) in values)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private void RestoreEnvironmentOverrides()
    {
        if (_previousEnvironmentValues is null)
        {
            return;
        }

        foreach (var (key, value) in _previousEnvironmentValues)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private Dictionary<string, string> CreateConfigurationOverrides()
    {
        return new Dictionary<string, string>
        {
            ["ConnectionStrings__DefaultConnection"] = SqlServerConnectionString,
            ["AppSettings__Secret"] = AuthenticationHelper.TestSecret,
            ["AppSettings__Emissor"] = AuthenticationHelper.Issuer,
            ["AppSettings__ValidoEm"] = AuthenticationHelper.Audience,
            ["AppSettings__ExpiracaoHoras"] = "2",
            ["RedisCacheSettings__Enabled"] = "true",
            ["RedisCacheSettings__ConnectionString"] = RedisConnectionString,
            ["RedisCacheSettings__InstanceName"] = "integration-tests",
            ["RedisCacheSettings__DefaultSeconds"] = "5",
            ["SeqSettings__Enabled"] = "false",
            ["SeqSettings__Url"] = "http://127.0.0.1:65535",
            ["SeqSettings__FilePath"] = SerilogFilePath,
            ["OpenTelemetry__Enabled"] = "true",
            ["OpenTelemetry__Otlp__Enabled"] = "false",
            ["Cors__AllowedOrigins__0"] = "https://app.example.test",
            ["Cors__AllowedMethods__0"] = "GET",
            ["Cors__AllowedMethods__1"] = "POST",
            ["Cors__AllowedMethods__2"] = "PUT",
            ["Cors__AllowedMethods__3"] = "DELETE",
            ["Cors__AllowedMethods__4"] = "OPTIONS",
            ["Cors__AllowedHeaders__0"] = "Authorization",
            ["Cors__AllowedHeaders__1"] = "Content-Type",
            ["Cors__AllowedHeaders__2"] = "X-ClientId",
            ["Cors__AllowCredentials"] = "false",
            ["ForwardedHeaders__Enabled"] = "false",
            ["RequestLimits__TimeoutSeconds"] = "30",
            ["RequestLimits__MaxRequestBodyBytes"] = "10485760",
            ["NativeRateLimitingSettings__Public__PermitLimit"] = "2",
            ["NativeRateLimitingSettings__Public__WindowSeconds"] = "30",
            ["NativeRateLimitingSettings__Public__QueueLimit"] = "0",
            ["NativeRateLimitingSettings__Authenticated__PermitLimit"] = "2",
            ["NativeRateLimitingSettings__Authenticated__WindowSeconds"] = "30",
            ["NativeRateLimitingSettings__Authenticated__QueueLimit"] = "0",
            ["NativeRateLimitingSettings__AuthenticationSensitive__PermitLimit"] = "2",
            ["NativeRateLimitingSettings__AuthenticationSensitive__WindowSeconds"] = "30",
            ["NativeRateLimitingSettings__AuthenticationSensitive__QueueLimit"] = "0"
        };
    }
}
