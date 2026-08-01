using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPedidoRepository : IDisposable
    {
        Task Adicionar(Pedido pedido);
        Task Atualizar(Pedido pedido);
        Task RemoverPorId(Guid id);
    }
}
