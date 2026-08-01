using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IMesaRepository : IDisposable
    {
        Task Adicionar(Mesa mesa, CancellationToken cancellationToken = default);
        Task Atualizar(Mesa mesa, CancellationToken cancellationToken = default);
        Task RemoverPorId(Guid id, CancellationToken cancellationToken = default);
        Task<Mesa?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
    }
}
