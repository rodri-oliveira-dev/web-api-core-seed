using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Repository
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        Task<Pedido> ObterPedidoItens(Guid id);
    }
}