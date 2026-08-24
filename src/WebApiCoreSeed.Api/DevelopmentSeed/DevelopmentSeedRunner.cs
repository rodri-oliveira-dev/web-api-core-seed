using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApiCoreSeed.Identity.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;

namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public sealed class DevelopmentSeedRunner
    {
        private static readonly Action<ILogger, Exception?> LogApplyingMigrations =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1000, nameof(LogApplyingMigrations)),
                "Applying development seed migrations.");

        private static readonly Action<ILogger, Exception?> LogRunningIdentitySeed =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1001, nameof(LogRunningIdentitySeed)),
                "Running Identity development seed.");

        private static readonly Action<ILogger, Exception?> LogRunningSampleRestaurantSeed =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1002, nameof(LogRunningSampleRestaurantSeed)),
                "Running SampleRestaurant development seed.");

        private static readonly Action<ILogger, int, int, Exception?> LogSeedCompleted =
            LoggerMessage.Define<int, int>(
                LogLevel.Information,
                new EventId(1003, nameof(LogSeedCompleted)),
                "Development seed completed with {IdentityChanges} Identity changes and {SampleRestaurantChanges} SampleRestaurant changes.");

        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly SampleRestaurantDbContext _sampleRestaurantDbContext;
        private readonly DevelopmentSeedIdentitySeeder _identitySeeder;
        private readonly DevelopmentSeedSampleRestaurantSeeder _sampleRestaurantSeeder;
        private readonly ILogger<DevelopmentSeedRunner> _logger;

        public DevelopmentSeedRunner(
            IHostEnvironment environment,
            IConfiguration configuration,
            ApplicationDbContext applicationDbContext,
            SampleRestaurantDbContext sampleRestaurantDbContext,
            DevelopmentSeedIdentitySeeder identitySeeder,
            DevelopmentSeedSampleRestaurantSeeder sampleRestaurantSeeder,
            ILogger<DevelopmentSeedRunner> logger)
        {
            _environment = environment;
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
            _sampleRestaurantDbContext = sampleRestaurantDbContext;
            _identitySeeder = identitySeeder;
            _sampleRestaurantSeeder = sampleRestaurantSeeder;
            _logger = logger;
        }

        public async Task<DevelopmentSeedResult> RunAsync(CancellationToken cancellationToken = default)
        {
            DevelopmentSeedConfiguration.EnsureAllowedEnvironment(_environment);
            var options = DevelopmentSeedConfiguration.ReadOptions(_configuration);

            cancellationToken.ThrowIfCancellationRequested();
            LogApplyingMigrations(_logger, null);
            await _applicationDbContext.Database.MigrateAsync(cancellationToken);
            await _sampleRestaurantDbContext.Database.MigrateAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            LogRunningIdentitySeed(_logger, null);
            var identityChanges = await _identitySeeder.SeedAsync(options.User, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            LogRunningSampleRestaurantSeed(_logger, null);
            var sampleRestaurantChanges = await _sampleRestaurantSeeder.SeedAsync(cancellationToken);

            LogSeedCompleted(_logger, identityChanges, sampleRestaurantChanges, null);

            return new DevelopmentSeedResult
            {
                IdentityChanges = identityChanges,
                SampleRestaurantChanges = sampleRestaurantChanges
            };
        }

    }
}
