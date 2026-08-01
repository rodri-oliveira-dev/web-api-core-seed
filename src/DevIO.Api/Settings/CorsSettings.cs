using System.Collections.Generic;

namespace Restaurante.IO.Api.Settings
{
    public sealed class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = System.Array.Empty<string>();

        public string[] AllowedMethods { get; set; } = { "GET", "POST", "PUT", "DELETE", "OPTIONS" };

        public string[] AllowedHeaders { get; set; } = { "Authorization", "Content-Type", "X-ClientId" };

        public bool AllowCredentials { get; set; }

        public bool AllowWildcardSubdomains { get; set; } = true;

        public IEnumerable<string> GetAllowedOrigins()
        {
            return AllowedOrigins ?? System.Array.Empty<string>();
        }
    }
}
