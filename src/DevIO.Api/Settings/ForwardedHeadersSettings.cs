namespace Restaurante.IO.Api.Settings
{
    public sealed class ForwardedHeadersSettings
    {
        public bool Enabled { get; set; }

        public string[] KnownProxies { get; set; } = System.Array.Empty<string>();

        public string[] KnownNetworks { get; set; } = System.Array.Empty<string>();

        public int ForwardLimit { get; set; } = 1;
    }
}
