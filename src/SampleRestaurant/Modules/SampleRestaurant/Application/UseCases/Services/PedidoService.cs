using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class PedidoService : BaseService, IPedidoService
    {
        private readonly IPedidoRepository _fornecedorRepository;

        public PedidoService(IPedidoRepository fornecedorRepository, 
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<bool> Adicionar(Pedido pedido)
        {
            if (!ExecutarValidacao(new PedidoValidation(), pedido) ) return false;

            await _fornecedorRepository.Adicionar(pedido);
            return true;
        }

        public async Task<bool> Atualizar(Pedido pedido)
        {
            if (!ExecutarValidacao(new PedidoValidation(), pedido)) return false;

            await _fornecedorRepository.Atualizar(pedido);
            return true;
        }

        public async Task<bool> Remover(Guid id)
        {
            await _fornecedorRepository.RemoverPorId(id);
            return true;
        }

        public void Dispose()
        {
            _fornecedorRepository?.Dispose();
        }
    }
}
