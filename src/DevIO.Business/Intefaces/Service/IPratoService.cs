using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Service
{
    public interface IPratoService : IDisposable
    {
        Task<bool> Adicionar(Prato prato);
        Task<bool> Atualizar(Prato prato);
        Task<bool> Remover(Guid id);

    }
}