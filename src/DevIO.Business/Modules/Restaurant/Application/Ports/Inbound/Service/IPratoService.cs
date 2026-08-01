using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurante.IO.Business.Interfaces.Pagination;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Service
{
    public interface IPratoService : IDisposable
    {
        Task<bool> Adicionar(Prato prato);
        Task<bool> Atualizar(Prato prato);
        Task<bool> Remover(Guid id);
        Task<Prato> ObterPorId(Guid id);
        Task<IEnumerable<Prato>> Paginacao(PaginationParameter paginationParameter);
        Task<int> TotalRegistros();
    }
}
