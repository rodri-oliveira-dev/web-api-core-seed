using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries;
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

        public Task Adicionar(Prato prato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Pratos.Add(prato);
            return Task.CompletedTask;
        }

        public Task Atualizar(Prato prato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Pratos.Update(prato);
            return Task.CompletedTask;
        }

        public Task RemoverPorId(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Pratos.Remove(new Prato { Id = id });
            return Task.CompletedTask;
        }

        public async Task<Prato?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Pratos.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<bool> ExisteComId(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Pratos.AnyAsync(prato => prato.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<PratoListItem>> ListarPagina(PaginationParameter paginationParameter, CancellationToken cancellationToken = default)
        {
            return await _context.Pratos.AsNoTracking()
                .OrderBy(prato => prato.Titulo)
                .ThenBy(prato => prato.Id)
                .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                .Take(paginationParameter.PageSize)
                .Select(prato => new PratoListItem
                {
                    Id = prato.Id,
                    Titulo = prato.Titulo,
                    Descricao = prato.Descricao,
                    Foto = prato.Foto,
                    Preco = prato.Preco,
                    Ativo = prato.Ativo,
                    TipoPrato = prato.TipoPrato
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> Contar(CancellationToken cancellationToken = default)
        {
            return await _context.Pratos.CountAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
