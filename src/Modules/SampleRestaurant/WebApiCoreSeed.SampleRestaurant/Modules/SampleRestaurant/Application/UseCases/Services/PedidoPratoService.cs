using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Interfaces;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Service;
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

        public async Task<bool> Adicionar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new PedidoPratoValidation(), pedidoPrato)) return false;

            await _fornecedorRepository.Adicionar(pedidoPrato, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Atualizar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new PedidoPratoValidation(), pedidoPrato)) return false;

            await _fornecedorRepository.Atualizar(pedidoPrato, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Remover(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _fornecedorRepository.RemoverPorId(id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

    }
}
