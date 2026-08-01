using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class AtendenteRepository : IAtendenteRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public AtendenteRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<int> Adicionar(Atendente atendente)
        {
            _context.Atendentes.Add(atendente);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Atualizar(Atendente atendente)
        {
            _context.Atendentes.Update(atendente);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoverPorId(Guid id)
        {
            _context.Atendentes.Remove(new Atendente { Id = id });
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
