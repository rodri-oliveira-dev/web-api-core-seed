using Microsoft.EntityFrameworkCore;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Restaurante.IO.Business.Interfaces.Pagination;
using Restaurante.IO.Business.Interfaces.Repository;


namespace Restaurante.IO.Data.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly MeuDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(MeuDbContext db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<int> TotalRegistros()
        {
            return await DbSet.CountAsync();
        }

        public async Task<IEnumerable<TEntity>> Paginacao(PaginationParameter paginationParameter)
        {
            return await DbSet.AsNoTracking()
                .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                .Take(paginationParameter.PageSize)
                .ToListAsync();
        }

        public virtual async Task<TEntity> ObterPorId(Guid id)
        {
            try
            {
                return await DbSet.FindAsync(id);
            }
            catch
            {
                return null;
            }
        }

        public virtual async Task<List<TEntity>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<int> Adicionar(TEntity entity)
        {
            try
            {
                DbSet.Add(entity);
                return await SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public virtual async Task<int> Atualizar(TEntity entity)
        {
            DbSet.Update(entity);
            return await SaveChanges();
        }

        public virtual async Task<int> Remover(Guid id)
        {
            DbSet.Remove(new TEntity { Id = id });
            return await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db?.Dispose();
        }
    }
}