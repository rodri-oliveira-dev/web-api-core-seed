using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Service
{
    public interface IAtendenteService : IDisposable
    {
        Task<bool> Adicionar(Atendente atendente);
        Task<bool> Atualizar(Atendente atendente);
        Task<bool> Remover(Guid id);

    }
}