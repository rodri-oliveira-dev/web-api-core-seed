using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Service
{
    public interface IPedidoPratoService : IDisposable
    {
        Task<bool> Adicionar(PedidoPrato pedidoPrato);
        Task<bool> Atualizar(PedidoPrato pedidoPrato);
        Task<bool> Remover(Guid id);

    }
}