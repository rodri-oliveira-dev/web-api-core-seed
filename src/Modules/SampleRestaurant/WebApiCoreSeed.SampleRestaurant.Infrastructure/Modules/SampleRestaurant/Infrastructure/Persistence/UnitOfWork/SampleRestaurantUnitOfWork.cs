using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Persistence
{
    public sealed class SampleRestaurantUnitOfWork : ISampleRestaurantUnitOfWork
    {
        private readonly SampleRestaurantDbContext _context;

        public SampleRestaurantUnitOfWork(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

