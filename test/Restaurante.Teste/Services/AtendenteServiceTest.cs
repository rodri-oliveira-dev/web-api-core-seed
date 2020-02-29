using Microsoft.EntityFrameworkCore;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Notificacoes;
using Restaurante.IO.Business.Services;
using Restaurante.IO.Data.Context;
using Restaurante.IO.Data.Repository;
using Xunit;

namespace Restaurante.Teste.Services
{
    public class AtendenteServiceTest
    {
        private readonly AtendenteService _atendenteService;

        public AtendenteServiceTest()
        {
            var context = new MeuDbContext(new DbContextOptions<MeuDbContext>());

            INotificador notificador = new Notificador();
            IAtendenteRepository atendenteRepository = new AtendenteRepository(context);
            _atendenteService = new AtendenteService(atendenteRepository, notificador);
        }

        [Fact]
        public void Test1()
        {

        }
    }
}
