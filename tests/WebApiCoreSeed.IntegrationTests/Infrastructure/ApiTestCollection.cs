namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class ApiIntegrationFixtureDefinition : ICollectionFixture<ApiFactory>
{
    public const string Name = "api-integration";
}
