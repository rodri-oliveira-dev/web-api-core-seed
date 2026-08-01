using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface IAtendenteService : IDisposable
    {
        Task<bool> Adicionar(Atendente atendente, CancellationToken cancellationToken = default);
        Task<bool> Atualizar(Atendente atendente, CancellationToken cancellationToken = default);
        Task<bool> Remover(Guid id, CancellationToken cancellationToken = default);

    }
}
