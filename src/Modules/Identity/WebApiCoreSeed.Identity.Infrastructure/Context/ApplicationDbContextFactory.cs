using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebApiCoreSeed.Identity.Infrastructure.Context
{
    public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = DesignTimeConnectionString.GetDefaultConnection();

            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

    internal static class DesignTimeConnectionString
    {
        private const string DefaultConnectionName = "DefaultConnection";

        public static string GetDefaultConnection()
        {
            var repositoryRoot = FindRepositoryRoot(AppContext.BaseDirectory);
            var apiSettingsPath = Path.Combine(repositoryRoot, "src", "WebApiCoreSeed.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiSettingsPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(DefaultConnectionName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' was not found for design-time EF Core tooling.");
            }

            return connectionString;
        }

        private static string FindRepositoryRoot(string startPath)
        {
            var current = new DirectoryInfo(startPath);

            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "WebApiCoreSeed.slnx")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new InvalidOperationException("Could not locate repository root containing WebApiCoreSeed.slnx.");
        }
    }
}
