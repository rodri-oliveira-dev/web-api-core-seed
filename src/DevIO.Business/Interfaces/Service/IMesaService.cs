using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Service
{
    public interface IMesaService : IDisposable
    {
        Task<bool> Adicionar(Mesa mesa);
        Task<bool> Atualizar(Mesa mesa);
        Task<bool> Remover(Guid id);

    }
}