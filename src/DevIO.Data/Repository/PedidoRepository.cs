using Restaurante.IO.Business.Intefaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;
using System;
using System.Threading.Tasks;

namespace Restaurante.IO.Data.Repository
{
    public class PedidoRepository : Repository<Pedido>, IPedidoRepository
    {
        public PedidoRepository(MeuDbContext context) : base(context) { }


        public Task<Pedido> ObterPedidoItens(Guid id)
        {
            return  base.ObterPorId(id);
        }
    }
}