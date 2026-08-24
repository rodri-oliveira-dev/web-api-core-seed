namespace WebApiCoreSeed.IntegrationTests.Infrastructure.LegacyUpgrade;

[CollectionDefinition(Name)]
public sealed class LegacySchemaUpgradeFixtureDefinition : ICollectionFixture<LegacySchemaUpgradeFixture>
{
    public const string Name = "legacy-schema-upgrade";
}
