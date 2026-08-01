using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPedidoPratoRepository : IDisposable
    {
        Task<int> Adicionar(PedidoPrato pedidoPrato);
        Task<int> Atualizar(PedidoPrato pedidoPrato);
        Task<int> RemoverPorId(Guid id);
    }
}
