using Restaurante.IO.Business.Intefaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;

namespace Restaurante.IO.Data.Repository
{
    public class AtendenteRepository : Repository<Atendente>, IAtendenteRepository
    {
        public AtendenteRepository(MeuDbContext context) : base(context) { }

    }
}