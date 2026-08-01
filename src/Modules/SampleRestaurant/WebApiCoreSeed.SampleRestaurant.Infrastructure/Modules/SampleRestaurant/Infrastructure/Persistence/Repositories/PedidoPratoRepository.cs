using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class PedidoPratoRepository : IPedidoPratoRepository
    {
        private readonly SampleRestaurantDbContext _context;

        public PedidoPratoRepository(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public Task Adicionar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.PedidoPrato.Add(pedidoPrato);
            return Task.CompletedTask;
        }

        public Task Atualizar(PedidoPrato pedidoPrato, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.PedidoPrato.Update(pedidoPrato);
            return Task.CompletedTask;
        }

        public Task RemoverPorId(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.PedidoPrato.Remove(new PedidoPrato { Id = id });
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
