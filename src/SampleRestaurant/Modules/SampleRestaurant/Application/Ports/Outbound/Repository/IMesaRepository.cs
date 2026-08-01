using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IMesaRepository : IDisposable
    {
        Task Adicionar(Mesa mesa);
        Task Atualizar(Mesa mesa);
        Task RemoverPorId(Guid id);
        Task<Mesa> ObterPorId(Guid id);
    }
}
