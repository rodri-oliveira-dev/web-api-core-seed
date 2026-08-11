using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPedidoRepository
    {
        Task Adicionar(Pedido pedido, CancellationToken cancellationToken = default);
        Task Atualizar(Pedido pedido, CancellationToken cancellationToken = default);
        Task RemoverPorId(Guid id, CancellationToken cancellationToken = default);
    }
}
