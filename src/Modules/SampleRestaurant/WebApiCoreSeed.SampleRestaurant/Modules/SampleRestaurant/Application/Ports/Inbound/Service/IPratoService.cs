using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Service
{
    public interface IPratoService
    {
        Task<bool> Adicionar(Prato prato, CancellationToken cancellationToken = default);
        Task<bool> Atualizar(Prato prato, CancellationToken cancellationToken = default);
        Task<bool> Remover(Guid id, CancellationToken cancellationToken = default);
        Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PratoListItem>> Paginacao(PaginationParameter paginationParameter, CancellationToken cancellationToken = default);
        Task<int> TotalRegistros(CancellationToken cancellationToken = default);
    }
}
