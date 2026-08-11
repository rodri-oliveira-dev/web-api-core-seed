using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface IPedidoPratoService
    {
        Task<bool> Adicionar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default);
        Task<bool> Atualizar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default);
        Task<bool> Remover(Guid id, CancellationToken cancellationToken = default);

    }
}
