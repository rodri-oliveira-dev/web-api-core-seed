using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Restaurante.IO.Api.Configuration;
using Serilog;

namespace Restaurante.IO.Api
{
    public static class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Getting the motors running...");

                var builder = WebApplication.CreateBuilder(args);
                builder.WebHost.UseIIS();
                builder.Host.UseApiSerilog();

                builder.Services.AddApiServices(builder.Configuration);

                var app = builder.Build();

                app.UseApiPipeline();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
