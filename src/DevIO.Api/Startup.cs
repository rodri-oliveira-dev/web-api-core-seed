using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Restaurante.IO.Api.Configuration;
using Restaurante.IO.Api.Configuration.Cache;
using Restaurante.IO.Api.Configuration.Swagger;
using Restaurante.IO.Api.Diagnostics;
using Restaurante.IO.Api.Extensions;
using Restaurante.IO.Api.Filters;
using Restaurante.IO.Data.Context;
using Serilog;
using Serilog.Events;
using System;
using System.Data.SqlClient;
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"))
            {
                Password = Configuration["DbPassword"]
            };

            services.AddDbContext<MeuDbContext>(options =>
            {
                options.UseSqlServer(builder.ConnectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
            services.AddControllers(options =>
                {
                    options.Filters.Add(new HttpResponseExceptionFilter());
                    options.Filters.Add<SerilogLoggingActionFilter>();
                }).AddJsonOptions(op => { op.JsonSerializerOptions.IgnoreNullValues = true; });

            services.AddHealthChecks().AddSqlServer(builder.ConnectionString, name: "Banco de Dados", tags: new[] { "db", "sql", "sqlserver" });

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
            app.UseHsts();

            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                logger.LogWarning(RetornaMensagemErro(context.HttpContext.Response.StatusCode));
                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new CustomResult(false, new
                {
                    statusCode = context.HttpContext.Response.StatusCode,
                    errorMessage = RetornaMensagemErro(context.HttpContext.Response.StatusCode)
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

        private static string RetornaMensagemErro(int statusCode)
        {
            return statusCode switch
            {
                400 => "O pedido não pode ser cumprido devido à erro de sintaxe.",
                401 => "A chamada precisa ser efetuada por um usuario autenticado.",
                403 => "O usuário esta autenticado, mas o não possui permissão para executar essa ação.",
                404 => "A página solicitada não pôde ser encontrada, mas pode estar disponível novamente no futuro.",
                405 => "Foi feita uma solicitação de uma página usando um método de solicitação não suportado por essa página.",
                _ => ReasonPhrases.GetReasonPhrase(statusCode)
            };
        }
    }
}
