using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPedidoRepository : IDisposable
    {
        Task<int> Adicionar(Pedido pedido);
        Task<int> Atualizar(Pedido pedido);
        Task<int> RemoverPorId(Guid id);
    }
}
