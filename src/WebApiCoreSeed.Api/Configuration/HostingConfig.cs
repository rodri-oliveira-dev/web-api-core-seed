using System;
using System.IO.Compression;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApiCoreSeed.Api.Configuration.OpenApi;
using WebApiCoreSeed.Api.Errors;
using WebApiCoreSeed.Api.Filters;
using WebApiCoreSeed.Api.Middlewares;
using WebApiCoreSeed.Api.Settings;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using Serilog;
using Serilog.Events;

namespace WebApiCoreSeed.Api.Configuration
{
    public static class HostingConfig
    {
        private const string SerilogOutputTemplate =
            "[{Timestamp:HH:mm:ss} {Level:u3} TraceId={TraceId} SpanId={SpanId}] {Message:lj} {Properties:j}{NewLine}{Exception}";
        private static readonly string[] JsonMimeTypes = { "application/json" };
        private static readonly string[] DatabaseHealthCheckTags = { "db", "sql", "sqlserver" };
        private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, Exception?> LogStatusCodeProblem =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(1000, nameof(LogStatusCodeProblem)),
                "Status code page generated ProblemDetails: {ProblemDetail}");

        public static ConfigureHostBuilder UseApiSerilog(this ConfigureHostBuilder host)
        {
            host.UseSerilog((context, _, loggerConfiguration) =>
            {
                var seqSettings = new SeqSettings();
                context.Configuration.GetSection(SeqSettings.SectionName).Bind(seqSettings);

                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Debug()
                    .Filter.ByExcluding("RequestPath = '/hc' and StatusCode = 200")
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .WriteTo.Debug(outputTemplate: SerilogOutputTemplate, formatProvider: CultureInfo.InvariantCulture)
                    .WriteTo.Console(outputTemplate: SerilogOutputTemplate, formatProvider: CultureInfo.InvariantCulture);

                if (seqSettings.Enabled)
                {
                    loggerConfiguration.WriteTo.Seq(seqSettings.Url, formatProvider: CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrWhiteSpace(seqSettings.FilePath))
                {
                    loggerConfiguration.WriteTo.File(
                        seqSettings.FilePath,
                        outputTemplate: SerilogOutputTemplate,
                        formatProvider: CultureInfo.InvariantCulture,
                        fileSizeLimitBytes: 1_000_000,
                        rollOnFileSizeLimit: true,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1));
                }
            });

            return host;
        }

        public static ConfigureWebHostBuilder ConfigureApiKestrel(this ConfigureWebHostBuilder webHost, IConfiguration configuration)
        {
            var settings = GetRequestLimitsSettings(configuration);
            webHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = settings.MaxRequestBodyBytes;
            });

            return webHost;
        }

        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");

            services.AddDbContext<SampleRestaurantDbContext>(options =>
            {
                options
                    .UseSqlServer(
                        defaultConnection,
                        sqlOptions => sqlOptions.MigrationsAssembly(typeof(SampleRestaurantDbContext).Assembly.FullName))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
            services.ResolveDependencies();
            services.WebApiConfig(configuration);
            services.AddApiForwardedHeaders(configuration);
            services.AddApiRequestLimits(configuration);
            services.AddNativeRateLimiting(configuration);
            services.AddApiResponseCompression();
            services.ConfigureCookie();
            services.AddApiHealthChecks(configuration, defaultConnection);
            services.AddApiOpenTelemetry(configuration, environment);
            services.ConfigureApiHsts();
            services.ConfigureCache(configuration);

            return services;
        }

        public static WebApplication UseApiPipeline(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("WebApiCoreSeed.Api.Pipeline");

            app.UseConfiguredForwardedHeaders();
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

                    diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

                    var endpoint = httpContext.GetEndpoint();
                    if (endpoint != null)
                    {
                        diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                    }
                };
            });

            app.UseMiddleware<SerilogMiddleware>();
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseStatusCodePages(async context =>
            {
                var httpContext = context.HttpContext;
                var statusCode = httpContext.Response.StatusCode;
                var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                var detail = ApiProblemDetails.DetailForStatusCode(statusCode);

                LogStatusCodeProblem(logger, detail, null);

                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = ApiProblemDetails.Create(
                        httpContext,
                        statusCode,
                        ApiProblemDetails.TypeForStatusCode(statusCode),
                        ApiProblemDetails.TitleForStatusCode(statusCode),
                        detail)
                });
            });

            app.AjustesSeguranca();
            app.UseResponseCompression();
            app.UseMvcConfiguration();
            app.UseOpenApiConfig();
            app.MapApiHealthChecks();

            return app;
        }

        private static IServiceCollection AddApiForwardedHeaders(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ForwardedHeadersSettings();
            configuration.GetSection("ForwardedHeaders").Bind(settings);
            var environment = configuration["ASPNETCORE_ENVIRONMENT"];

            if (settings.Enabled
                && string.Equals(environment, Environments.Production, StringComparison.OrdinalIgnoreCase)
                && !HasKnownForwarder(settings))
            {
                throw new InvalidOperationException("ForwardedHeaders is enabled in production, but no known proxy or network was configured.");
            }

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = Math.Max(1, settings.ForwardLimit);
                options.KnownProxies.Clear();
                options.KnownIPNetworks.Clear();

                foreach (var proxy in settings.KnownProxies.Where(value => !string.IsNullOrWhiteSpace(value)))
                {
                    if (!IPAddress.TryParse(proxy, out var address))
                    {
                        throw new InvalidOperationException($"Invalid forwarded proxy IP address: {proxy}");
                    }

                    options.KnownProxies.Add(address);
                }

                foreach (var network in settings.KnownNetworks.Where(value => !string.IsNullOrWhiteSpace(value)))
                {
                    var parsedNetwork = ParseKnownNetwork(network);
                    options.KnownIPNetworks.Add(parsedNetwork);
                }
            });

            return services;
        }

        private static WebApplication UseConfiguredForwardedHeaders(this WebApplication app)
        {
            var settings = new ForwardedHeadersSettings();
            app.Configuration.GetSection("ForwardedHeaders").Bind(settings);

            if (settings.Enabled)
            {
                app.UseForwardedHeaders();
            }

            return app;
        }

        private static bool HasKnownForwarder(ForwardedHeadersSettings settings)
        {
            return settings.KnownProxies.Any(value => !string.IsNullOrWhiteSpace(value))
                || settings.KnownNetworks.Any(value => !string.IsNullOrWhiteSpace(value));
        }

        private static System.Net.IPNetwork ParseKnownNetwork(string value)
        {
            var parts = value.Split('/', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2
                || !IPAddress.TryParse(parts[0], out var prefix)
                || !int.TryParse(parts[1], out var prefixLength))
            {
                throw new InvalidOperationException($"Invalid forwarded network CIDR: {value}");
            }

            return new System.Net.IPNetwork(prefix, prefixLength);
        }

        private static IServiceCollection AddApiRequestLimits(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = GetRequestLimitsSettings(configuration);
            services.Configure<RequestLimitsSettings>(configuration.GetSection("RequestLimits"));
            services.AddRequestTimeouts(options =>
            {
                options.DefaultPolicy = new RequestTimeoutPolicy
                {
                    Timeout = TimeSpan.FromSeconds(Math.Max(1, settings.TimeoutSeconds)),
                    TimeoutStatusCode = StatusCodes.Status503ServiceUnavailable
                };
            });

            return services;
        }

        private static RequestLimitsSettings GetRequestLimitsSettings(IConfiguration configuration)
        {
            var settings = new RequestLimitsSettings();
            configuration.GetSection("RequestLimits").Bind(settings);

            settings.TimeoutSeconds = Math.Max(1, settings.TimeoutSeconds);
            settings.MaxRequestBodyBytes = Math.Max(1024, settings.MaxRequestBodyBytes);

            return settings;
        }

        private static void MapApiHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = WriteMinimalHealthResponse
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing")
                    ? WriteDetailedHealthResponse
                    : WriteMinimalHealthResponse
            });

            app.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = WriteMinimalHealthResponse
            });
        }

        private static async System.Threading.Tasks.Task WriteMinimalHealthResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                new
                {
                    status = report.Status.ToString()
                },
                cancellationToken: context.RequestAborted);
        }

        private static async System.Threading.Tasks.Task WriteDetailedHealthResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                new
                {
                    status = report.Status.ToString(),
                    entries = report.Entries.ToDictionary(
                        entry => entry.Key,
                        entry => new
                        {
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description
                        })
                },
                cancellationToken: context.RequestAborted);
        }

        private static IServiceCollection AddApiResponseCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(JsonMimeTypes);
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
                .AddSqlServer(defaultConnection, name: "Banco de Dados", tags: DatabaseHealthCheckTags);

            var seqSettings = new SeqSettings();
            configuration.GetSection(SeqSettings.SectionName).Bind(seqSettings);

            if (seqSettings.Enabled)
            {
                healthChecks.AddUrlGroup(new Uri(seqSettings.Url), "Seq Log");
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
