using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;

namespace Restaurante.IO.Business.Services
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
            await _fornecedorRepository.Remover(id);
            return true;
        }

        public void Dispose()
        {
            _fornecedorRepository?.Dispose();
        }
    }
}