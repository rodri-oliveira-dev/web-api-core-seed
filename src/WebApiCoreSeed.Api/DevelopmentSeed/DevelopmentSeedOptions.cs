namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public sealed class DevelopmentSeedOptions
    {
        public const string SectionName = "DevelopmentSeed";

        public DevelopmentSeedUserOptions User { get; set; } = new();
    }

    public sealed class DevelopmentSeedUserOptions
    {
        public string Id { get; set; } = DevelopmentSeedDefinition.UserId;
        public string Email { get; set; } = "developer@example.local";
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
