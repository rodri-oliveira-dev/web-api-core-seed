using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPedidoPratoRepository : IDisposable
    {
        Task Adicionar(PedidoPrato pedidoPrato);
        Task Atualizar(PedidoPrato pedidoPrato);
        Task RemoverPorId(Guid id);
    }
}
