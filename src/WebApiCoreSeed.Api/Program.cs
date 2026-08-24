using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using WebApiCoreSeed.Api.Configuration;
using WebApiCoreSeed.Api.DevelopmentSeed;

namespace WebApiCoreSeed.Api
{
    public sealed class Program
    {
        private Program()
        {
        }

        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Getting the motors running...");

                var builder = WebApplication.CreateBuilder(args);
                builder.WebHost.UseIIS();
                builder.WebHost.ConfigureApiKestrel(builder.Configuration);
                builder.Host.UseApiSerilog();

                builder.Services.AddApiServices(builder.Configuration, builder.Environment);

                var app = builder.Build();

                if (DevelopmentSeedCommand.ShouldRun(args))
                {
                    await app.RunDevelopmentSeedAsync();
                    return;
                }

                app.UseApiPipeline();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                Environment.ExitCode = 1;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
