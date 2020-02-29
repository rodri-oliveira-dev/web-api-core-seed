using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Api.Data
{
    public class LogginContext : DbContext
    {
        public LogginContext(DbContextOptions<LogginContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new SqlConnectionStringBuilder("Data Source=localhost,1433; Initial Catalog=restaurante;User Id=sa;Application Name=restaurante;MultipleActiveResultSets=true;")
                {
                    Password = "7R@f@el@"
                };
                optionsBuilder.UseSqlServer(builder.ConnectionString);
            }
        }
        public DbSet<LogginEntity> Login { get; set; }
    }
}