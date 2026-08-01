using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface IAtendenteService : IDisposable
    {
        Task<bool> Adicionar(Atendente atendente);
        Task<bool> Atualizar(Atendente atendente);
        Task<bool> Remover(Guid id);

    }
}