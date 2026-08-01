using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;

namespace Restaurante.IO.Data.Repository
{
    public class PedidoPratoRepository : Repository<PedidoPrato>, IPedidoPratoRepository
    {
        public PedidoPratoRepository(MeuDbContext context) : base(context) { }

    }
}