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

        public Task Adicionar(Mesa mesa)
        {
            _context.Mesas.Add(mesa);
            return Task.CompletedTask;
        }

        public Task Atualizar(Mesa mesa)
        {
            _context.Mesas.Update(mesa);
            return Task.CompletedTask;
        }

        public Task RemoverPorId(Guid id)
        {
            _context.Mesas.Remove(new Mesa { Id = id });
            return Task.CompletedTask;
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
