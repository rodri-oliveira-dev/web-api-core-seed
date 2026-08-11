using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebApiCoreSeed.Api.Configuration;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Configuration;

public sealed class IdentityConfigurationTests
{
    [Fact(DisplayName = "JWT exige HTTPS metadata por padrao")]
    [Trait("Configuration", "Identity")]
    public void JwtBearerQuandoAmbienteNaoRelaxadoDeveExigirHttpsMetadata()
    {
        var options = CreateJwtBearerOptions(environment: "Production");

        Assert.True(options.RequireHttpsMetadata);
    }

    [Fact(DisplayName = "JWT permite relaxar HTTPS metadata somente em Testing")]
    [Trait("Configuration", "Identity")]
    public void JwtBearerQuandoTestingDevePermitirHttpsMetadataRelaxado()
    {
        var options = CreateJwtBearerOptions(environment: "Testing");

        Assert.False(options.RequireHttpsMetadata);
    }

    private static JwtBearerOptions CreateJwtBearerOptions(string environment)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = environment,
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\MSSQLLocalDB;Database=IdentityConfigurationTests;Trusted_Connection=True;",
                ["AppSettings:Secret"] = "X-BURGUER@COCA-2-IDENTITY-CONFIGURATION-TEST-SECRET-2026",
                ["AppSettings:Emissor"] = "WebApiCoreSeed",
                ["AppSettings:ValidoEm"] = "https://localhost",
                ["AppSettings:ExpiracaoHoras"] = "2"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddIdentityConfiguration(configuration);

        using var provider = services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();

        return optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
    }
}
