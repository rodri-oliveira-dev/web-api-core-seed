using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface IMesaService : IDisposable
    {
        Task<bool> Adicionar(Mesa mesa);
        Task<bool> Atualizar(Mesa mesa);
        Task<bool> Remover(Guid id);
        Task<Mesa> ObterPorId(Guid id);
    }
}
