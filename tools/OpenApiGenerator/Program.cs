using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Restaurante.IO.Api;
using Restaurante.IO.Api.DataContext;
using Restaurante.IO.Api.Services.Interfaces;
using Restaurante.IO.Api.Settings;
using Restaurante.IO.Data.Context;

var documents = args.Length == 0
    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["/openapi/v1.json"] = "docs/openapi/openapi-v1.json",
        ["/openapi/v2.json"] = "docs/openapi/openapi-v2.json"
    }
    : ParseDocuments(args);

using var factory = new OpenApiFactory();
using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
{
    AllowAutoRedirect = false,
    BaseAddress = new Uri("https://localhost")
});

foreach (var document in documents)
{
    using var response = await client.GetAsync(document.Key);
    response.EnsureSuccessStatusCode();

    var outputPath = Path.GetFullPath(document.Value);
    var outputDirectory = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrEmpty(outputDirectory))
    {
        Directory.CreateDirectory(outputDirectory);
    }

    await using var output = File.Create(outputPath);
    await response.Content.CopyToAsync(output);
    Console.WriteLine($"{document.Key} -> {Path.GetRelativePath(Directory.GetCurrentDirectory(), outputPath)}");
}

static Dictionary<string, string> ParseDocuments(string[] arguments)
{
    var documents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    foreach (var argument in arguments)
    {
        var parts = argument.Split('=', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
        {
            throw new ArgumentException("Use arguments in the form /document/path.json=output/file.json.");
        }

        documents[parts[0]] = parts[1];
    }

    return documents;
}

sealed class OpenApiFactory : WebApplicationFactory<Restaurante.IO.Api.Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseContentRoot(Path.GetFullPath("src/DevIO.Api"));
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={_databaseName};Integrated Security=True;",
                ["AppSettings:Secret"] = "X-BURGUER@COCA-2-OPENAPI-GENERATOR-TEST-SECRET-2026",
                ["RedisCacheSettings:Enabled"] = "false",
                ["DatasulSeqSettings:Enabled"] = "false",
                ["DatasulSeqSettings:Url"] = "http://localhost",
                ["DatasulSeqSettings:FilePath"] = "openapi-generator.log"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<MeuDbContext>();
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<MeuDbContext>>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            services.AddDbContext<MeuDbContext>(options => options.UseInMemoryDatabase(_databaseName));
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(_databaseName + "-identity"));
            services.PostConfigure<HealthCheckServiceOptions>(options => options.Registrations.Clear());
            services.RemoveAll<RedisCacheSettings>();
            services.RemoveAll<IResponseCacheService>();
            services.AddSingleton(new RedisCacheSettings { Enabled = false });
        });
    }
}
