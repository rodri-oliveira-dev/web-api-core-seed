using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public static class DevelopmentSeedConfiguration
    {
        public static void EnsureAllowedEnvironment(IHostEnvironment environment)
        {
            if (environment.IsProduction())
            {
                throw new InvalidOperationException("Development seed is blocked in Production.");
            }
        }

        public static DevelopmentSeedOptions ReadOptions(IConfiguration configuration)
        {
            var options = new DevelopmentSeedOptions();
            configuration.GetSection(DevelopmentSeedOptions.SectionName).Bind(options);

            options.User.Email = NormalizeRequired(options.User.Email, "DevelopmentSeed:User:Email");
            options.User.UserName = string.IsNullOrWhiteSpace(options.User.UserName)
                ? options.User.Email
                : options.User.UserName.Trim();
            options.User.Id = string.IsNullOrWhiteSpace(options.User.Id)
                ? DevelopmentSeedDefinition.UserId
                : options.User.Id.Trim();

            if (string.IsNullOrWhiteSpace(options.User.Password))
            {
                throw new InvalidOperationException("DevelopmentSeed:User:Password is required for development seed.");
            }

            if (options.User.Password.StartsWith("replace-with", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("DevelopmentSeed:User:Password must be replaced with a local development password.");
            }

            return options;
        }

        private static string NormalizeRequired(string value, string key)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{key} is required for development seed.");
            }

            return value.Trim();
        }
    }
}
