using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Service
{
    public interface IPedidoService : IDisposable
    {
        Task<bool> Adicionar(Pedido pedido);
        Task<bool> Atualizar(Pedido pedido);
        Task<bool> Remover(Guid id);

    }
}