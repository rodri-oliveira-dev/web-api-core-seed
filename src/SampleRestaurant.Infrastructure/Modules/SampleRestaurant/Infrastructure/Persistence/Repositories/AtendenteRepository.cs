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

        public Task Adicionar(Atendente atendente)
        {
            _context.Atendentes.Add(atendente);
            return Task.CompletedTask;
        }

        public Task Atualizar(Atendente atendente)
        {
            _context.Atendentes.Update(atendente);
            return Task.CompletedTask;
        }

        public Task RemoverPorId(Guid id)
        {
            _context.Atendentes.Remove(new Atendente { Id = id });
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
