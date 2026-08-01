using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class MesaRepository : IMesaRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public MesaRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<int> Adicionar(Mesa mesa)
        {
            _context.Mesas.Add(mesa);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Atualizar(Mesa mesa)
        {
            _context.Mesas.Update(mesa);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoverPorId(Guid id)
        {
            _context.Mesas.Remove(new Mesa { Id = id });
            return await _context.SaveChangesAsync();
        }

        public async Task<Mesa> ObterPorId(Guid id)
        {
            return await _context.Mesas.FindAsync(id);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
