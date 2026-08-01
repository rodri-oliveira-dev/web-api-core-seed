using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;

namespace Restaurante.IO.Data.Repository
{
    public class PratoRepository : Repository<Prato>, IPratoRepository
    {
        public PratoRepository(MeuDbContext context) : base(context) { }

    }
}