using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class PratoService : BaseService, IPratoService
    {
        private readonly IPratoRepository _pratoRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;
        private readonly INotificador _notificador;

        public PratoService(IPratoRepository pratoRepository,
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _pratoRepository = pratoRepository;
            _unitOfWork = unitOfWork;
            _notificador = notificador;
        }

        public async Task<bool> Adicionar(Prato prato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new PratoValidation(), prato)) return false;

            if (await _pratoRepository.ExisteComId(prato.Id, cancellationToken))
            {
                _notificador.Handle(new Notificacao($"Já existe um objeto cadastrado com a ID {prato.Id}."));
                return false;
            }

            await _pratoRepository.Adicionar(prato, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Atualizar(Prato prato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new PratoValidation(), prato)) return false;

            await _pratoRepository.Atualizar(prato, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Remover(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _pratoRepository.RemoverPorId(id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
        {
            return await _pratoRepository.ObterPorId(id, cancellationToken);
        }

        public async Task<IReadOnlyList<PratoListItem>> Paginacao(PaginationParameter paginationParameter, CancellationToken cancellationToken = default)
        {
            return await _pratoRepository.ListarPagina(paginationParameter, cancellationToken);
        }

        public async Task<int> TotalRegistros(CancellationToken cancellationToken = default)
        {
            return await _pratoRepository.Contar(cancellationToken);
        }

    }
}
