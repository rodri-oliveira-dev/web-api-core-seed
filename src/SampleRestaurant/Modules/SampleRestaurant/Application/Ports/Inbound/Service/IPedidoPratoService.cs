using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface IPedidoPratoService : IDisposable
    {
        Task<bool> Adicionar(PedidoPrato pedidoPrato);
        Task<bool> Atualizar(PedidoPrato pedidoPrato);
        Task<bool> Remover(Guid id);

    }
}