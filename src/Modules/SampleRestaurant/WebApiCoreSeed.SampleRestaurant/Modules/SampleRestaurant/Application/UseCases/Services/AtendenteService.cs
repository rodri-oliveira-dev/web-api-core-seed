using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class AtendenteService : BaseService, IAtendenteService
    {
        private readonly IAtendenteRepository _atendenteRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;

        public AtendenteService(IAtendenteRepository atendenteRepository,
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _atendenteRepository = atendenteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Adicionar(Atendente atendente, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new AtendenteValidation(), atendente)) return false;

            await _atendenteRepository.Adicionar(atendente, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Atualizar(Atendente atendente, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new AtendenteValidation(), atendente)) return false;

            await _atendenteRepository.Atualizar(atendente, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Remover(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _atendenteRepository.RemoverPorId(id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public void Dispose()
        {
            _atendenteRepository?.Dispose();
        }
    }
}
