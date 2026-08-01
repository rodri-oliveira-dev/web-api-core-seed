using Microsoft.AspNetCore.Http;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;
using WebApiCoreSeed.SampleRestaurant.Services;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using WebApiCoreSeed.Api.Extensions;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
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

            //Services
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IMesaService, MesaService>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IPedidoPratoService, PedidoPratoService>();
            services.AddScoped<IPratoService, PratoService>();

            services.AddScoped<INotificador, Notificador>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, AspNetUser>();

            return services;
        }
    }
}
