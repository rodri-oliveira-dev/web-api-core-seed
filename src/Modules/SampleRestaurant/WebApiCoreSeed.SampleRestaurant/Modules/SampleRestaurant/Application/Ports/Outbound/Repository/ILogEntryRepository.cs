using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface ILogEntryRepository
    {
        Task Registrar(LogEntry log, CancellationToken cancellationToken = default);
    }
}
