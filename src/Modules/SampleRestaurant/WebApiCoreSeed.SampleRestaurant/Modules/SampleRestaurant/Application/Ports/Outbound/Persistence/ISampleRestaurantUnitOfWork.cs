using System.Threading;
using System.Threading.Tasks;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence
{
    public interface ISampleRestaurantUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}

