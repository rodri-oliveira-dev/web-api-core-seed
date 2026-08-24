using Microsoft.AspNetCore.Http;
using WebApiCoreSeed.SampleRestaurant.Interfaces;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;
using WebApiCoreSeed.SampleRestaurant.Services;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Persistence;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using WebApiCoreSeed.Api.DevelopmentSeed;
using WebApiCoreSeed.Api.Extensions;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;

namespace WebApiCoreSeed.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<SampleRestaurantDbContext>();

            //Repository
            services.AddScoped<IAtendenteRepository, AtendenteRepository>();
            services.AddScoped<IMesaRepository, MesaRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPedidoPratoRepository, PedidoPratoRepository>();
            services.AddScoped<IPratoRepository, PratoRepository>();
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<ISampleRestaurantUnitOfWork, SampleRestaurantUnitOfWork>();

            //Services
            services.AddScoped<IAtendenteService, AtendenteService>();
            services.AddScoped<ILogEntryService, LogEntryService>();
            services.AddScoped<IMesaService, MesaService>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPedidoPratoService, PedidoPratoService>();
            services.AddScoped<IPratoService, PratoService>();

            services.AddScoped<INotificador, Notificador>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, AspNetUser>();
            services.AddDevelopmentSeed();

            return services;
        }
    }
}
