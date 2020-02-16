using AutoMapper;
using Restaurante.IO.Api.ViewModels;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Api.Configuration
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