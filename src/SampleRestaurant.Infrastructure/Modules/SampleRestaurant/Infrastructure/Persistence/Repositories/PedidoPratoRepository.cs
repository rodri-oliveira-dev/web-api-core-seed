using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class PedidoPratoRepository : IPedidoPratoRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public PedidoPratoRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<int> Adicionar(PedidoPrato pedidoPrato)
        {
            _context.PedidoPrato.Add(pedidoPrato);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Atualizar(PedidoPrato pedidoPrato)
        {
            _context.PedidoPrato.Update(pedidoPrato);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoverPorId(Guid id)
        {
            _context.PedidoPrato.Remove(new PedidoPrato { Id = id });
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
