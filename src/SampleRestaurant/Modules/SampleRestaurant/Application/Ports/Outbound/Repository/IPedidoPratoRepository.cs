using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPedidoPratoRepository : IDisposable
    {
        Task Adicionar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default);
        Task Atualizar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default);
        Task RemoverPorId(Guid id, CancellationToken cancellationToken = default);
    }
}
