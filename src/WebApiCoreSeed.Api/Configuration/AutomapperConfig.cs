using AutoMapper;
using WebApiCoreSeed.Api.ViewModels;
using WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.Api.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Atendente, AtendenteViewModel>().ReverseMap();
            CreateMap<Mesa, MesaViewModel>().ReverseMap();
            CreateMap<MesaRequestViewModel, Mesa>()
                .ForMember(destino => destino.Ativo, opt => opt.MapFrom(origem => origem.Ativo.GetValueOrDefault()))
                .ForMember(destino => destino.LocalizacaoMesa, opt => opt.MapFrom(origem => origem.LocalizacaoMesa.GetValueOrDefault()));
            CreateMap<MesaRequestViewModel, MesaViewModel>()
                .ForMember(destino => destino.Ativo, opt => opt.MapFrom(origem => origem.Ativo.GetValueOrDefault()))
                .ForMember(destino => destino.LocalizacaoMesa, opt => opt.MapFrom(origem => origem.LocalizacaoMesa.GetValueOrDefault()));
            CreateMap<Pedido, PedidoViewModel>().ReverseMap();
            CreateMap<PedidoPrato, PedidoPratoViewModel>().ReverseMap();
            CreateMap<Prato, PratoViewModel>().ReverseMap();
            CreateMap<PratoRequestViewModel, Prato>()
                .ForMember(destino => destino.Foto, opt => opt.MapFrom(origem => origem.Foto ?? string.Empty))
                .ForMember(destino => destino.Ativo, opt => opt.MapFrom(origem => origem.Ativo.GetValueOrDefault()))
                .ForMember(destino => destino.TipoPrato, opt => opt.MapFrom(origem => origem.TipoPrato.GetValueOrDefault()));
            CreateMap<PratoRequestViewModel, PratoViewModel>()
                .ForMember(destino => destino.Foto, opt => opt.MapFrom(origem => origem.Foto ?? string.Empty))
                .ForMember(destino => destino.Ativo, opt => opt.MapFrom(origem => origem.Ativo.GetValueOrDefault()))
                .ForMember(destino => destino.TipoPrato, opt => opt.MapFrom(origem => origem.TipoPrato.GetValueOrDefault()));
            CreateMap<PratoListItem, PratoViewModel>();
        }
    }
}
