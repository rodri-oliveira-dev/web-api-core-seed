using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;

namespace Restaurante.IO.Business.Services
{
    public class PedidoPratoService : BaseService, IPedidoPratoService
    {
        private readonly IPedidoPratoRepository _fornecedorRepository;

        public PedidoPratoService(IPedidoPratoRepository fornecedorRepository, 
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<bool> Adicionar(PedidoPrato pedidoPrato)
        {
            if (!ExecutarValidacao(new PedidoPratoValidation(), pedidoPrato) ) return false;

            await _fornecedorRepository.Adicionar(pedidoPrato);
            return true;
        }

        public async Task<bool> Atualizar(PedidoPrato pedidoPrato)
        {
            if (!ExecutarValidacao(new PedidoPratoValidation(), pedidoPrato)) return false;

            await _fornecedorRepository.Atualizar(pedidoPrato);
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