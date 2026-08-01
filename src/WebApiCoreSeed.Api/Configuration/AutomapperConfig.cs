using AutoMapper;
using WebApiCoreSeed.Api.ViewModels;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.Api.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Atendente, AtendenteViewModel>().ReverseMap();
            CreateMap<Mesa, MesaViewModel>().ReverseMap();
            CreateMap<Pedido, PedidoViewModel>().ReverseMap();
            CreateMap<PedidoPrato, PedidoPratoViewModel>().ReverseMap();
            CreateMap<Prato, PratoViewModel>().ReverseMap();
        }
    }
}