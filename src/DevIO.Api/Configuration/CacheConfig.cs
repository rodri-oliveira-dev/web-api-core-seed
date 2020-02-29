using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurante.IO.Api.Configuration.Cache;
using Restaurante.IO.Api.Services;
using Restaurante.IO.Api.Services.Interfaces;

namespace Restaurante.IO.Api.Configuration
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
                services.AddDistributedRedisCache(option =>
                {
                    option.Configuration = redisCacheSettings.ConnectionString;
                    option.InstanceName = redisCacheSettings.InstanceName;
                });
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
