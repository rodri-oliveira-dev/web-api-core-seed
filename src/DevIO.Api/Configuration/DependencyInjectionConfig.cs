using Microsoft.AspNetCore.Http;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Notificacoes;
using Restaurante.IO.Business.Services;
using Restaurante.IO.Data.Context;
using Restaurante.IO.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Restaurante.IO.Api.Configuration.Swagger;
using Restaurante.IO.Api.Extensions;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Restaurante.IO.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<MeuDbContext>();

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

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}