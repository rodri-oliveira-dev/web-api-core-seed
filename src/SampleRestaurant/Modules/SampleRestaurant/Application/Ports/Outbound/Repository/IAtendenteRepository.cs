using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IAtendenteRepository : IDisposable
    {
        Task Adicionar(Atendente atendente);
        Task Atualizar(Atendente atendente);
        Task RemoverPorId(Guid id);
    }
}
