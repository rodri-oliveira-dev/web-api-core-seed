using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface ILogginService
    {
        Task<bool> Adicionar(LogginEntity mesa, CancellationToken cancellationToken = default);
    }
}
