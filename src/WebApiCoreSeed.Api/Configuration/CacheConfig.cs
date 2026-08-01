using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiCoreSeed.Api.Services;
using WebApiCoreSeed.Api.Services.Interfaces;
using WebApiCoreSeed.Api.Settings;

namespace WebApiCoreSeed.Api.Configuration
{
    public static class CacheConfig
    {
        public static IServiceCollection ConfigureCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisCacheSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisCacheSettings);
            services.AddSingleton(redisCacheSettings);

            if (redisCacheSettings.Enabled)
            {
                services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisCacheSettings.ConnectionString;
                        options.InstanceName = redisCacheSettings.InstanceName;
                    });
                services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            }

            return services;
        }
    }
}
