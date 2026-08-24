using Microsoft.Extensions.DependencyInjection;

namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public static class DevelopmentSeedServiceCollectionExtensions
    {
        public static IServiceCollection AddDevelopmentSeed(this IServiceCollection services)
        {
            services.AddScoped<DevelopmentSeedRunner>();
            services.AddScoped<DevelopmentSeedIdentitySeeder>();
            services.AddScoped<DevelopmentSeedSampleRestaurantSeeder>();

            return services;
        }
    }
}
