using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
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
using Restaurante.IO.Api.Configuration;
using Restaurante.IO.Api.Configuration.Swagger;
using Restaurante.IO.Api.Filters;
using Restaurante.IO.Api.Middlewares;
using Restaurante.IO.Api.Resources;
using Restaurante.IO.Api.Results;
using Restaurante.IO.Api.Settings;
using Restaurante.IO.Data.Context;
using Serilog;
using Serilog.Events;
using System;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;

namespace Restaurante.IO.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MeuDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString.GetConnectionString()).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddMvc(options => { options.RespectBrowserAcceptHeader = true; })
                .AddJsonOptions(op => { op.JsonSerializerOptions.IgnoreNullValues = true; });

            services.AddIdentityConfiguration(Configuration);
            services.AddAutoMapper(typeof(Startup));
            services.AddSwaggerConfig();
            services.ResolveDependencies();
            services.WebApiConfig();
            services.ConfigureRateLimit(Configuration);

            //Ativa compressão dados
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.ConfigureCookie();
            services.AddControllers(options => { options.Filters.Add<SerilogLoggingActionFilter>(); })
                .AddJsonOptions(op => { op.JsonSerializerOptions.IgnoreNullValues = true; });

            services.AddHealthChecks()
                .AddSqlServer(ConnectionString.GetConnectionString(), name: "Banco de Dados", tags: new[] { "db", "sql", "sqlserver" });

            var datasulSeqSettings = new DatasulSeqSettings();
            Configuration.GetSection("DatasulSeqSettings").Bind(datasulSeqSettings);

            if (datasulSeqSettings.Enabled)
            {
                services.AddHealthChecks().AddUrlGroup(new Uri(datasulSeqSettings.Url), "Datasul Seq Log");
            }

            var cacheSettings = new RedisCacheSettings();
            Configuration.GetSection("RedisCacheSettings").Bind(cacheSettings);

            if (cacheSettings.Enabled)
            {
                services.AddHealthChecks().AddRedis(cacheSettings.ConnectionString, "Cache Redis");
            }

            services.Configure<HstsOptions>(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddHealthChecksUI();
            services.ConfigureCache(Configuration);
        }

        public static void Configure(IApplicationBuilder app, IHostEnvironment env, IApiVersionDescriptionProvider provider, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("Development");
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseCors("Production");
                app.UseExceptionHandler("/error");
            }

            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                options.MessageTemplate = "Handled {RequestPath}";
                // Emit debug-level events instead of the defaults
                options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var request = httpContext.Request;

                    // Set all the common properties available for every request
                    diagnosticContext.Set("Host", request.Host);
                    diagnosticContext.Set("Protocol", request.Protocol);
                    diagnosticContext.Set("Scheme", request.Scheme);

                    // Only set it if available. You're not sending sensitive data in a querystring right?!
                    if (request.QueryString.HasValue)
                    {
                        diagnosticContext.Set("QueryString", request.QueryString.Value);
                    }

                    // Set the content-type of the Response at this point
                    diagnosticContext.Set("ContentType", httpContext.Response.ContentType);
                    // Retrieve the IEndpointFeature selected for the request
                    var endpoint = httpContext.GetEndpoint();
                    if (endpoint != null) // endpoint != null
                    {
                        diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                    }
                };
            });

            app.UseMiddleware<SerilogMiddleware>();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseHsts();

            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                logger.LogWarning(HttpErrorMessages.RetornaMensagemErro(context.HttpContext.Response.StatusCode));
                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new CustomResult(false, new
                {
                    statusCode = context.HttpContext.Response.StatusCode,
                    errorMessage = HttpErrorMessages.RetornaMensagemErro(context.HttpContext.Response.StatusCode)
                })));
            });

            app.ConfigureRateLimit();
            app.AjustesSeguranca();
            app.UseAuthentication();
            app.UseMvcConfiguration();
            app.UseResponseCompression();
            app.UseSwaggerConfig(provider);
            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(options => { options.UIPath = "/hc-ui"; });
        }
    }
}
