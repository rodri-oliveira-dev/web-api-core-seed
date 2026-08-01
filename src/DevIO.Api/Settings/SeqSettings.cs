namespace Restaurante.IO.Api.Settings
{
    public sealed class SeqSettings
    {
        public const string SectionName = "SeqSettings";

        public bool Enabled { get; set; }

        public string Url { get; set; } = "http://localhost:5341";

        public string FilePath { get; set; } = "logs/web-api-core-seed.log";
    }
}
