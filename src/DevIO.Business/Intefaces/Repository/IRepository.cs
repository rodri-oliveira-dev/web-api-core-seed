using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Restaurante.IO.Business.Intefaces.Pagination;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Business.Intefaces.Repository
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task<int> Adicionar(TEntity entity);
        Task<TEntity> ObterPorId(Guid id);
        Task<List<TEntity>> ObterTodos();
        Task<int> TotalRegistros();
        Task<IEnumerable<TEntity>> Paginacao(PaginationParameter paginationParameter);
        Task<int> Atualizar(TEntity entity);
        Task<int> Remover(Guid id);
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate);
        Task<int> SaveChanges();
    }
}