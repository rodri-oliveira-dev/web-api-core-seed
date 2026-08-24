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
    public class MesaService : BaseService, IMesaService
    {
        private readonly IMesaRepository _fornecedorRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;

        public MesaService(IMesaRepository fornecedorRepository,
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Adicionar(Mesa mesa, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new MesaValidation(), mesa)) return false;

            await _fornecedorRepository.Adicionar(mesa, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Atualizar(Mesa mesa, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new MesaValidation(), mesa)) return false;

            await _fornecedorRepository.Atualizar(mesa, cancellationToken);
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

        public async Task<Mesa?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
        {
            return await _fornecedorRepository.ObterPorId(id, cancellationToken);
        }

    }
}
