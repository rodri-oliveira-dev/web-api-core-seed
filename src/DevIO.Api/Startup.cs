using System;
using System.Data.SqlClient;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurante.IO.Api.Configuration;
using Restaurante.IO.Api.Configuration.Swagger;
using Restaurante.IO.Api.Filters;
using Restaurante.IO.Data.Context;
using System.IO.Compression;
using System.Linq;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpsPolicy;
using Restaurante.IO.Api.Configuration.Cache;

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
            services.AddDbContext<MeuDbContext>(options =>
            {
                var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"))
                {
                    Password = Configuration["DbPassword"]
                };

                options.UseSqlServer(builder.ConnectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddMvc(options => { options.RespectBrowserAcceptHeader = true; })
                .AddNewtonsoftJson()
                .AddJsonOptions(op => { op.JsonSerializerOptions.IgnoreNullValues = true; })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

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
            services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()))
                .AddJsonOptions(op => { op.JsonSerializerOptions.IgnoreNullValues = true; })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddHealthChecks().AddSqlServer(Configuration.GetConnectionString("DefaultConnection"), name: "Banco de Dados", tags: new[] { "db", "sql", "sqlserver" });

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

        public static void Configure(IApplicationBuilder app, IHostEnvironment env, IApiVersionDescriptionProvider provider)
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

            app.UseHsts();
            app.UseStatusCodePagesWithRedirects("/error/{0}");
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

            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/hc-ui";
            });
        }
    }
}
