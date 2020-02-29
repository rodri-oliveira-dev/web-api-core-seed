using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Data.Context;

namespace Restaurante.IO.Data.Repository
{
    public class LogginRepository : Repository<LogginEntity>, ILogginRepository
    {
        public LogginRepository(MeuDbContext context) : base(context) { }

    }
}