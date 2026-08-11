using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IAtendenteRepository
    {
        Task Adicionar(Atendente atendente, CancellationToken cancellationToken = default);
        Task Atualizar(Atendente atendente, CancellationToken cancellationToken = default);
        Task RemoverPorId(Guid id, CancellationToken cancellationToken = default);
    }
}
