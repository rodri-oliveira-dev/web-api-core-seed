using Microsoft.Extensions.Configuration;

namespace Restaurante.IO.Api.Resources
{
    internal static class ConnectionString
    {
        public static string GetConnectionString()
        {
            return Startup.Configuration.GetConnectionString("DefaultConnection");
        }
    }
}