using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class PedidoPratoService : BaseService, IPedidoPratoService
    {
        private readonly IPedidoPratoRepository _fornecedorRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;

        public PedidoPratoService(IPedidoPratoRepository fornecedorRepository, 
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Adicionar(PedidoPrato pedidoPrato)
        {
            if (!ExecutarValidacao(new PedidoPratoValidation(), pedidoPrato) ) return false;

            await _fornecedorRepository.Adicionar(pedidoPrato);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Atualizar(PedidoPrato pedidoPrato)
        {
            if (!ExecutarValidacao(new PedidoPratoValidation(), pedidoPrato)) return false;

            await _fornecedorRepository.Atualizar(pedidoPrato);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Remover(Guid id)
        {
            await _fornecedorRepository.RemoverPorId(id);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public void Dispose()
        {
            _fornecedorRepository?.Dispose();
        }
    }
}
