using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface IMesaService
    {
        Task<bool> Adicionar(Mesa mesa, CancellationToken cancellationToken = default);
        Task<bool> Atualizar(Mesa mesa, CancellationToken cancellationToken = default);
        Task<bool> Remover(Guid id, CancellationToken cancellationToken = default);
        Task<Mesa?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
    }
}
