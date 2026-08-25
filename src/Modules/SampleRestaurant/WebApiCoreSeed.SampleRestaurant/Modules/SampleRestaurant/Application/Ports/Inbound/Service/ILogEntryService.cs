using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Service
{
    public interface ILogEntryService
    {
        Task<bool> Adicionar(LogEntry logEntry, CancellationToken cancellationToken = default);
    }
}
