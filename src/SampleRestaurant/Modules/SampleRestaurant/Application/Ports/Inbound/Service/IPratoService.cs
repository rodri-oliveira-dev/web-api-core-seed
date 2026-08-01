using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
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
