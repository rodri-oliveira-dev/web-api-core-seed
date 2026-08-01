using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class LogginRepository : ILogginRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public LogginRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<int> Registrar(LogginEntity log)
        {
            _context.Loggins.Add(log);
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
