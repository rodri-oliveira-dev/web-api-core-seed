using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface IPratoRepository : IDisposable
    {
        Task<int> Adicionar(Prato prato);
        Task<int> Atualizar(Prato prato);
        Task<int> RemoverPorId(Guid id);
        Task<Prato> ObterPorId(Guid id);
        Task<bool> ExisteComId(Guid id);
        Task<IEnumerable<Prato>> ListarPagina(PaginationParameter paginationParameter);
        Task<int> Contar();
    }
}
