using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Service
{
    public interface IPedidoService
    {
        Task<bool> Adicionar(Pedido pedido, CancellationToken cancellationToken = default);
        Task<bool> Atualizar(Pedido pedido, CancellationToken cancellationToken = default);
        Task<bool> Remover(Guid id, CancellationToken cancellationToken = default);

    }
}
