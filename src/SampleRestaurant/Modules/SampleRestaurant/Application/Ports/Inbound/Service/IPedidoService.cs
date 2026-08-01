using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface IPedidoService : IDisposable
    {
        Task<bool> Adicionar(Pedido pedido);
        Task<bool> Atualizar(Pedido pedido);
        Task<bool> Remover(Guid id);

    }
}