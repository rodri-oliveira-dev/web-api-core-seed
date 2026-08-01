using Asp.Versioning;
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApiCoreSeed.Api.Settings;
using WebApiCoreSeed.Api.Configuration.OpenApi;

namespace WebApiCoreSeed.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddOpenApi(options => options.ConfigureSeedOpenApi());

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.Configure<CorsSettings>(configuration.GetSection("Cors"));
            services.AddCors(options =>
            {
                options.AddPolicy("Development", builder => ConfigureCorsPolicy(builder, configuration));
                options.AddPolicy("Production", builder => ConfigureCorsPolicy(builder, configuration));
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(ResolveCorsPolicyName(app));
            app.UseRequestTimeouts();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });

            return app;
        }

        public static IApplicationBuilder AjustesSeguranca(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
                context.Response.Headers["Permissions-Policy"] =
                    "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
                context.Response.Headers["Content-Security-Policy"] =
                    "default-src 'self'; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data: https:; font-src 'self' data:; connect-src 'self'";

                context.Response.OnStarting(() =>
                {
                    if (IsSensitiveResponse(context))
                    {
                        context.Response.Headers.CacheControl = "no-store";
                        context.Response.Headers.Pragma = "no-cache";
                    }

                    return System.Threading.Tasks.Task.CompletedTask;
                });

                await next();
            });

            return app;
        }

        private static void ConfigureCorsPolicy(Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder builder, IConfiguration configuration)
        {
            var settings = new CorsSettings();
            configuration.GetSection("Cors").Bind(settings);
            var origins = settings.GetAllowedOrigins()
                .Where(origin => !string.IsNullOrWhiteSpace(origin))
                .Select(origin => origin.Trim())
                .ToArray();

            if (origins.Any(origin => origin == "*"))
            {
                throw new InvalidOperationException("Cors:AllowedOrigins must not use the literal wildcard '*'. Configure explicit origins instead.");
            }

            builder.WithMethods(settings.AllowedMethods)
                .WithHeaders(settings.AllowedHeaders);

            if (origins.Length == 0)
            {
                builder.SetIsOriginAllowed(_ => false);
                return;
            }

            builder.WithOrigins(origins);

            if (settings.AllowWildcardSubdomains)
            {
                builder.SetIsOriginAllowedToAllowWildcardSubdomains();
            }

            if (settings.AllowCredentials)
            {
                builder.AllowCredentials();
            }
        }

        private static string ResolveCorsPolicyName(IApplicationBuilder app)
        {
            var environment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

            return environment.IsDevelopment() ? "Development" : "Production";
        }

        private static bool IsSensitiveResponse(HttpContext context)
        {
            var path = context.Request.Path;

            return path.StartsWithSegments("/api/v1/entrar")
                || path.StartsWithSegments("/api/v2/entrar")
                || path.StartsWithSegments("/api/v1/nova-conta")
                || context.Response.StatusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden;
        }
    }
}
