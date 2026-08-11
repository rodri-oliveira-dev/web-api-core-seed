using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class PedidoService : BaseService, IPedidoService
    {
        private readonly IPedidoRepository _fornecedorRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;

        public PedidoService(IPedidoRepository fornecedorRepository,
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Adicionar(Pedido pedido, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new PedidoValidation(), pedido)) return false;

            await _fornecedorRepository.Adicionar(pedido, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Atualizar(Pedido pedido, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new PedidoValidation(), pedido)) return false;

            await _fornecedorRepository.Atualizar(pedido, cancellationToken);
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
