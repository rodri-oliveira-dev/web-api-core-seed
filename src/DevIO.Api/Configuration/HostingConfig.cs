using System;
using System.IO.Compression;
using System.Linq;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Restaurante.IO.Api.Configuration.Swagger;
using Restaurante.IO.Api.Errors;
using Restaurante.IO.Api.Filters;
using Restaurante.IO.Api.Middlewares;
using Restaurante.IO.Api.Settings;
using Restaurante.IO.Data.Context;
using Serilog;
using Serilog.Events;

namespace Restaurante.IO.Api.Configuration
{
    public static class HostingConfig
    {
        public static ConfigureHostBuilder UseApiSerilog(this ConfigureHostBuilder host)
        {
            host.UseSerilog((context, _, loggerConfiguration) =>
            {
                var datasulSeqSettings = new DatasulSeqSettings();
                context.Configuration.GetSection(nameof(DatasulSeqSettings)).Bind(datasulSeqSettings);

                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Debug()
                    .Filter.ByExcluding("RequestPath = '/hc' and StatusCode = 200")
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .WriteTo.Debug()
                    .WriteTo.Seq(datasulSeqSettings.Url)
                    .WriteTo.File(
                        datasulSeqSettings.FilePath,
                        fileSizeLimitBytes: 1_000_000,
                        rollOnFileSizeLimit: true,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1))
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
            });

            return host;
        }

        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<MeuDbContext>(options =>
            {
                options.UseSqlServer(defaultConnection).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddControllers(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.Filters.Add<SerilogLoggingActionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    var problemDetails = context.ProblemDetails;
                    var statusCode = problemDetails.Status ?? context.HttpContext.Response.StatusCode;

                    problemDetails.Status = statusCode;
                    problemDetails.Type ??= ApiProblemDetails.TypeForStatusCode(statusCode);
                    problemDetails.Title ??= ApiProblemDetails.TitleForStatusCode(statusCode);
                    problemDetails.Detail ??= ApiProblemDetails.DetailForStatusCode(statusCode);
                    problemDetails.Instance ??= context.HttpContext.Request.Path;
                    ApiProblemDetails.AddTraceId(problemDetails, context.HttpContext);
                };
            });
            services.AddExceptionHandler<FluentValidationExceptionHandler>();
            services.AddExceptionHandler<PersistenceExceptionHandler>();
            services.AddExceptionHandler<UnhandledExceptionHandler>();

            services.AddIdentityConfiguration(configuration);
            services.AddAutoMapper(_ => { }, typeof(AutomapperConfig).Assembly);
            services.AddSwaggerConfig();
            services.ResolveDependencies();
            services.WebApiConfig();
            services.ConfigureRateLimit(configuration);
            services.AddApiResponseCompression();
            services.ConfigureCookie();
            services.AddApiHealthChecks(configuration, defaultConnection);
            services.ConfigureApiHsts();
            services.ConfigureCache(configuration);

            return services;
        }

        public static WebApplication UseApiPipeline(this WebApplication app)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Restaurante.IO.Api.Pipeline");

            if (app.Environment.IsDevelopment())
            {
                app.UseCors("Development");
            }
            else
            {
                app.UseCors("Production");
            }

            app.UseExceptionHandler();

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "Handled {RequestPath}";
                options.GetLevel = (_, _, _) => LogEventLevel.Debug;
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var request = httpContext.Request;

                    diagnosticContext.Set("Host", request.Host);
                    diagnosticContext.Set("Protocol", request.Protocol);
                    diagnosticContext.Set("Scheme", request.Scheme);

                    if (request.QueryString.HasValue)
                    {
                        diagnosticContext.Set("QueryString", request.QueryString.Value);
                    }

                    diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

                    var endpoint = httpContext.GetEndpoint();
                    if (endpoint != null)
                    {
                        diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                    }
                };
            });

            app.UseMiddleware<SerilogMiddleware>();
            app.UseHsts();

            app.UseStatusCodePages(async context =>
            {
                var httpContext = context.HttpContext;
                var statusCode = httpContext.Response.StatusCode;
                var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

                logger.LogWarning(ApiProblemDetails.DetailForStatusCode(statusCode));

                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = ApiProblemDetails.Create(
                        httpContext,
                        statusCode,
                        ApiProblemDetails.TypeForStatusCode(statusCode),
                        ApiProblemDetails.TitleForStatusCode(statusCode),
                        ApiProblemDetails.DetailForStatusCode(statusCode))
                });
            });

            app.ConfigureRateLimit();
            app.AjustesSeguranca();
            app.UseResponseCompression();
            app.UseMvcConfiguration();
            app.UseSwaggerConfig(provider);
            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }

        private static IServiceCollection AddApiResponseCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            return services;
        }

        private static IServiceCollection AddApiHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration,
            string defaultConnection)
        {
            var healthChecks = services.AddHealthChecks()
                .AddSqlServer(defaultConnection, name: "Banco de Dados", tags: new[] { "db", "sql", "sqlserver" });

            var datasulSeqSettings = new DatasulSeqSettings();
            configuration.GetSection(nameof(DatasulSeqSettings)).Bind(datasulSeqSettings);

            if (datasulSeqSettings.Enabled)
            {
                healthChecks.AddUrlGroup(new Uri(datasulSeqSettings.Url), "Datasul Seq Log");
            }

            var cacheSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(cacheSettings);

            if (cacheSettings.Enabled)
            {
                healthChecks.AddRedis(cacheSettings.ConnectionString, "Cache Redis");
            }

            return services;
        }

        private static IServiceCollection ConfigureApiHsts(this IServiceCollection services)
        {
            services.Configure<HstsOptions>(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            return services;
        }
    }
}
