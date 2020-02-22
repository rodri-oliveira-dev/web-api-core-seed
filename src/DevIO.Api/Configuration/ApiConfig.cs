using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Restaurante.IO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });

            services.AddCors(options =>
            {
                options.AddPolicy("Development",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

                options.AddPolicy("Production",
                    builder =>
                        builder
                            .WithMethods("GET", "POST", "PUT", "DELETE")
                            .AllowAnyOrigin()
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });

            return app;
        }

        public static IApplicationBuilder AjustesSeguranca(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'none';" +
                                                                        "script-src 'self' 'unsafe-inline'; " +
                                                                        "font-src 'self' https://fonts.gstatic.com; " +
                                                                        "img-src 'self' https://avatars3.githubusercontent.com data:;" +
                                                                        "connect-src 'self';style-src 'self' 'unsafe-inline' https://fonts.googleapis.com");
                context.Response.Headers.Add("Feature-Policy", "camera 'none'; microphone 'none'; speaker 'self';" +
                                                               "vibrate 'none'; geolocation 'none'; accelerometer 'none';" +
                                                               "ambient-light-sensor 'none'; autoplay 'none'; encrypted-media 'none';" +
                                                               "gyroscope 'none'; magnetometer 'none'; midi 'none'; payment 'none'; " +
                                                               "picture-in-picture 'none'; usb 'none'; vr 'none';");

                await next();
            });

            return app;
        }
    }
}