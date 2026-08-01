using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class PedidoRepository : Repository<Pedido>, IPedidoRepository
    {
        public PedidoRepository(SampleRestaurantDbContext context) : base(context) { }


        public Task<Pedido> ObterPedidoItens(Guid id)
        {
            return  base.ObterPorId(id);
        }
    }
}