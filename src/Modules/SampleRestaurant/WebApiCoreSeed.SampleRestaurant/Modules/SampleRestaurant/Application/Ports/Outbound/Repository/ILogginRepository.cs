using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface ILogginRepository
    {
        Task Registrar(LogginEntity log, CancellationToken cancellationToken = default);
    }
}
