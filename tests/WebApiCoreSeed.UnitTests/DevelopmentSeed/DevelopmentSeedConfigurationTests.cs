using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using WebApiCoreSeed.Api.DevelopmentSeed;
using Xunit;

namespace WebApiCoreSeed.UnitTests.DevelopmentSeed;

public sealed class DevelopmentSeedConfigurationTests
{
    [Fact(DisplayName = "Configuracao de seed exige senha de desenvolvimento")]
    public void ReadOptionsQuandoSenhaAusenteDeveFalhar()
    {
        var configuration = CreateConfiguration();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            DevelopmentSeedConfiguration.ReadOptions(configuration));

        Assert.Contains("DevelopmentSeed:User:Password", exception.Message, StringComparison.Ordinal);
    }

    [Fact(DisplayName = "Configuracao de seed rejeita placeholder de senha")]
    public void ReadOptionsQuandoSenhaPlaceholderDeveFalhar()
    {
        var configuration = CreateConfiguration(("DevelopmentSeed:User:Password", "replace-with-local-password"));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            DevelopmentSeedConfiguration.ReadOptions(configuration));

        Assert.Contains("must be replaced", exception.Message, StringComparison.Ordinal);
    }

    [Fact(DisplayName = "Configuracao de seed aplica defaults determinisiticos")]
    public void ReadOptionsQuandoValoresOpcionaisAusentesDeveAplicarDefaults()
    {
        var configuration = CreateConfiguration(("DevelopmentSeed:User:Password", "NotASecret_ForTests_2026!"));

        var options = DevelopmentSeedConfiguration.ReadOptions(configuration);

        Assert.Equal(DevelopmentSeedDefinition.UserId, options.User.Id);
        Assert.Equal("developer@example.local", options.User.Email);
        Assert.Equal(options.User.Email, options.User.UserName);
    }

    [Fact(DisplayName = "Seed de desenvolvimento e bloqueado em Production")]
    public void EnsureAllowedEnvironmentQuandoProductionDeveFalhar()
    {
        var environment = new TestHostEnvironment(Environments.Production);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            DevelopmentSeedConfiguration.EnsureAllowedEnvironment(environment));

        Assert.Contains("Production", exception.Message, StringComparison.Ordinal);
    }

    [Fact(DisplayName = "Seed de desenvolvimento e permitido em Development")]
    public void EnsureAllowedEnvironmentQuandoDevelopmentNaoDeveFalhar()
    {
        var environment = new TestHostEnvironment(Environments.Development);

        DevelopmentSeedConfiguration.EnsureAllowedEnvironment(environment);
    }

    [Fact(DisplayName = "Definicao do seed nao possui IDs duplicados")]
    public void DevelopmentSeedDefinitionQuandoCarregadaNaoDeveTerIdsDuplicados()
    {
        var ids = DevelopmentSeedDefinition.Pratos.Select(prato => prato.Id)
            .Concat(DevelopmentSeedDefinition.Mesas.Select(mesa => mesa.Id))
            .Append(DevelopmentSeedDefinition.Atendente.Id)
            .Append(DevelopmentSeedDefinition.Pedido.Id)
            .Concat(DevelopmentSeedDefinition.PedidoPratos.Select(item => item.Id))
            .ToArray();

        Assert.Equal(ids.Length, ids.Distinct().Count());
    }

    [Fact(DisplayName = "Definicao do seed nao possui claims duplicadas")]
    public void DevelopmentSeedDefinitionQuandoCarregadaNaoDeveTerClaimsDuplicadas()
    {
        var claims = DevelopmentSeedDefinition.UserClaims
            .Select(claim => $"{claim.Type}:{claim.Value}")
            .ToArray();

        Assert.Equal(claims.Length, claims.Distinct(StringComparer.Ordinal).Count());
    }

    private static IConfiguration CreateConfiguration(params (string Key, string Value)[] values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values.ToDictionary(item => item.Key, item => (string?)item.Value))
            .Build();
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public TestHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "WebApiCoreSeed.UnitTests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
