using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPratoRepository : IDisposable
    {
        Task Adicionar(Prato prato, CancellationToken cancellationToken = default);
        Task Atualizar(Prato prato, CancellationToken cancellationToken = default);
        Task RemoverPorId(Guid id, CancellationToken cancellationToken = default);
        Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExisteComId(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PratoListItem>> ListarPagina(PaginationParameter paginationParameter, CancellationToken cancellationToken = default);
        Task<int> Contar(CancellationToken cancellationToken = default);
    }
}
