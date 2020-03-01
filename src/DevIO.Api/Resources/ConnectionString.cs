using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Restaurante.IO.Api.Resources
{
    internal static class ConnectionString
    {
        public  static string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(Startup.Configuration.GetConnectionString("DefaultConnection"))
            {
                Password = Startup.Configuration["DbPassword"]
            };

            return builder.ConnectionString;
        }
    }
}