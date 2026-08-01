using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IMesaRepository : IDisposable
    {
        Task<int> Adicionar(Mesa mesa);
        Task<int> Atualizar(Mesa mesa);
        Task<int> RemoverPorId(Guid id);
        Task<Mesa> ObterPorId(Guid id);
    }
}
