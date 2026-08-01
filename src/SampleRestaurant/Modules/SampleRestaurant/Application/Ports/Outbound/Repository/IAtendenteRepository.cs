using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IAtendenteRepository : IDisposable
    {
        Task<int> Adicionar(Atendente atendente);
        Task<int> Atualizar(Atendente atendente);
        Task<int> RemoverPorId(Guid id);
    }
}
