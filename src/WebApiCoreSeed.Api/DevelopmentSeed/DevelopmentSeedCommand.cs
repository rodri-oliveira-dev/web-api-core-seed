using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public static class DevelopmentSeedCommand
    {
        private const string SeedArgument = "--seed";

        public static bool ShouldRun(string[] args)
        {
            return args.Any(argument => string.Equals(argument, SeedArgument, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task RunDevelopmentSeedAsync(this WebApplication app)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            ConsoleCancelEventHandler cancelHandler = (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            Console.CancelKeyPress += cancelHandler;

            try
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationTokenSource.Token,
                    app.Lifetime.ApplicationStopping);

                using var scope = app.Services.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<DevelopmentSeedRunner>();

                await runner.RunAsync(linkedTokenSource.Token);
            }
            finally
            {
                Console.CancelKeyPress -= cancelHandler;
            }
        }
    }
}
