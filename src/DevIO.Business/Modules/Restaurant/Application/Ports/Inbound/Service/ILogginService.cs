using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Service
{
    public interface ILogginService : IDisposable
    {
        Task<bool> Adicionar(LogginEntity mesa);
    }
}