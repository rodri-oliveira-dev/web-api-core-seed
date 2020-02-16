using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Enums;
using Restaurante.IO.Data.Context;
using Restaurante.IO.Data.Repository;
using Xunit;

namespace Restaurante.Teste.Repositories
{
    public class AtendenteRepositoryTest
    {
        private readonly AtendenteRepository _atendenteRepository;
        private Atendente _atendente;

        public AtendenteRepositoryTest()
        {
            _atendente = new Atendente { Nome = "Rodrigo Oliveira", TipoAtendente = ETipoAtendente.Garcom };

            var context = new MeuDbContext(new DbContextOptions<MeuDbContext>());

            _atendenteRepository = new AtendenteRepository(context);
        }

        [Fact]
        public async Task TesteAdicionar()
        {
            //+ Arrange

            //+ Act
            var retorno = await _atendenteRepository.Adicionar(_atendente);

            //+ Assert
            Assert.True(retorno.Equals(1));
        }
    }
}