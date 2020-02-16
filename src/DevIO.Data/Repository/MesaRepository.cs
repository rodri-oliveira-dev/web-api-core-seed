using Restaurante.IO.Business.Intefaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;

namespace Restaurante.IO.Data.Repository
{
    public class MesaRepository : Repository<Mesa>, IMesaRepository
    {
        public MesaRepository(MeuDbContext context) : base(context) { }

    }
}