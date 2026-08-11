using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApiCoreSeed.Api;
using WebApiCoreSeed.Identity.Infrastructure.Context;
using WebApiCoreSeed.Api.Services.Interfaces;
using WebApiCoreSeed.Api.Settings;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.OpenApiGenerator;

const string SolutionFileName = "WebApiCoreSeed.slnx";
const string OpenApiV1Path = "/openapi/v1.json";
const string OpenApiV2Path = "/openapi/v2.json";
const string OpenApiV1OutputRelativePath = "docs/openapi/openapi-v1.json";
const string OpenApiV2OutputRelativePath = "docs/openapi/openapi-v2.json";
const string OpenApiServerBaseAddress = "https://localhost";

var repositoryRoot = FindRepositoryRoot(AppContext.BaseDirectory);

var documents = args.Length == 0
    ? CreateDefaultDocuments(repositoryRoot)
    : ParseDocuments(args, repositoryRoot);

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

using var factory = new OpenApiFactory(repositoryRoot);
using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
{
    AllowAutoRedirect = false,
    BaseAddress = new Uri(OpenApiServerBaseAddress)
});

foreach (var document in documents)
{
    using var response = await client.GetAsync(document.Key, cancellationTokenSource.Token);
    response.EnsureSuccessStatusCode();

    var outputPath = document.Value;
    var outputDirectory = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrEmpty(outputDirectory))
    {
        Directory.CreateDirectory(outputDirectory);
    }

    await using var output = File.Create(outputPath);
    await response.Content.CopyToAsync(output, cancellationTokenSource.Token);
    Console.WriteLine($"{document.Key} -> {Path.GetRelativePath(repositoryRoot, outputPath)}");
}

static Dictionary<string, string> CreateDefaultDocuments(string repositoryRoot)
{
    return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [OpenApiV1Path] = Path.GetFullPath(OpenApiV1OutputRelativePath, repositoryRoot),
        [OpenApiV2Path] = Path.GetFullPath(OpenApiV2OutputRelativePath, repositoryRoot)
    };
}

static Dictionary<string, string> ParseDocuments(string[] arguments, string repositoryRoot)
{
    var documents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    foreach (var argument in arguments)
    {
        var parts = argument.Split('=', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
        {
            throw new ArgumentException("Use arguments in the form /document/path.json=output/file.json.");
        }

        documents[parts[0]] = Path.GetFullPath(parts[1], repositoryRoot);
    }

    return documents;
}

static string FindRepositoryRoot(string startPath)
{
    var current = new DirectoryInfo(startPath);
    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, SolutionFileName)))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    throw new InvalidOperationException($"Could not find repository root containing {SolutionFileName}.");
}

namespace WebApiCoreSeed.OpenApiGenerator
{
sealed class OpenApiFactory : WebApplicationFactory<WebApiCoreSeed.Api.Program>
{
    private const string OpenApiSecret = "X-BURGUER@COCA-2-OPENAPI-GENERATOR-TEST-SECRET-2026";
    private const string ApiContentRootRelativePath = "src/WebApiCoreSeed.Api";
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly string _repositoryRoot;
    private readonly string? _previousDefaultConnection;
    private readonly string? _previousAppSecret;

    public OpenApiFactory(string repositoryRoot)
    {
        _repositoryRoot = repositoryRoot;
        _previousDefaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        _previousAppSecret = Environment.GetEnvironmentVariable("AppSettings__Secret");
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", BuildConnectionString(_databaseName));
        Environment.SetEnvironmentVariable("AppSettings__Secret", OpenApiSecret);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseContentRoot(Path.Combine(_repositoryRoot, ApiContentRootRelativePath));
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = BuildConnectionString(_databaseName),
                ["AppSettings:Secret"] = OpenApiSecret,
                ["RedisCacheSettings:Enabled"] = "false",
                ["SeqSettings:Enabled"] = "false",
                ["OpenTelemetry:Enabled"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<SampleRestaurantDbContext>();
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<SampleRestaurantDbContext>>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            services.AddDbContext<SampleRestaurantDbContext>(options => options.UseInMemoryDatabase(_databaseName));
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(_databaseName + "-identity"));
            services.PostConfigure<HealthCheckServiceOptions>(options => options.Registrations.Clear());
            services.RemoveAll<RedisCacheSettings>();
            services.RemoveAll<IResponseCacheService>();
            services.AddSingleton(new RedisCacheSettings { Enabled = false });
        });
    }

    protected override void Dispose(bool disposing)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _previousDefaultConnection);
        Environment.SetEnvironmentVariable("AppSettings__Secret", _previousAppSecret);

        base.Dispose(disposing);
    }

    private static string BuildConnectionString(string databaseName)
    {
        return $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={databaseName};Integrated Security=True;";
    }
}
}
