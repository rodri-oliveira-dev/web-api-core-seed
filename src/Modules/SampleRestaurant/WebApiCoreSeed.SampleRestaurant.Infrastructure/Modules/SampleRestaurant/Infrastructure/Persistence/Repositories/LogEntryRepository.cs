using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class LogEntryRepository : ILogEntryRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public LogEntryRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public Task Registrar(LogEntry log, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.LogEntries.Add(log);
            return Task.CompletedTask;
        }

    }
}
