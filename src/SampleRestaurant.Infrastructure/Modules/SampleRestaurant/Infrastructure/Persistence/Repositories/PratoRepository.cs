using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class PratoRepository : IPratoRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public PratoRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<int> Adicionar(Prato prato)
        {
            _context.Pratos.Add(prato);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Atualizar(Prato prato)
        {
            _context.Pratos.Update(prato);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoverPorId(Guid id)
        {
            _context.Pratos.Remove(new Prato { Id = id });
            return await _context.SaveChangesAsync();
        }

        public async Task<Prato> ObterPorId(Guid id)
        {
            return await _context.Pratos.FindAsync(id);
        }

        public async Task<bool> ExisteComId(Guid id)
        {
            return await _context.Pratos.AnyAsync(prato => prato.Id == id);
        }

        public async Task<IEnumerable<Prato>> ListarPagina(PaginationParameter paginationParameter)
        {
            return await _context.Pratos.AsNoTracking()
                .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                .Take(paginationParameter.PageSize)
                .ToListAsync();
        }

        public async Task<int> Contar()
        {
            return await _context.Pratos.CountAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
