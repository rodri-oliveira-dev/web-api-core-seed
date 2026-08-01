using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public PedidoRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public Task Adicionar(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            return Task.CompletedTask;
        }

        public Task Atualizar(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            return Task.CompletedTask;
        }

        public Task RemoverPorId(Guid id)
        {
            _context.Pedidos.Remove(new Pedido { Id = id });
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
