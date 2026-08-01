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

        public async Task<int> Adicionar(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Atualizar(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoverPorId(Guid id)
        {
            _context.Pedidos.Remove(new Pedido { Id = id });
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
