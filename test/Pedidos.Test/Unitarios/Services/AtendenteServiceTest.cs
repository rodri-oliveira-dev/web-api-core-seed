using Moq;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Core;
using Restaurante.IO.Business.Models.Enums;
using Restaurante.IO.Business.Services;
using System.Threading.Tasks;
using Xunit;

namespace Pedidos.Test.Unitarios.Services
{
    public class AtendenteServiceTest
    {
        private readonly Mock<IAtendenteRepository> _atendenteRepository;
        private readonly Mock<INotificador> _notificador;
        public AtendenteServiceTest()
        {
            _atendenteRepository = new Mock<IAtendenteRepository>();
            _notificador = new Mock<INotificador>();
        }

        [Fact(DisplayName = "Atendente cadastrado com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task CadastraAtendente()
        {
            //Arrange
            var atendente = new Atendente
            {
                Nome = "Rodrigo",
                Email = "rodrigodotnet@gmail.com",
                Telefone = new Telefone
                {
                    Ddd = 19,
                    Numero = 998861788,
                    TipoTelefone = ETipoTelefone.Celular
                }
            };
            _atendenteRepository.Setup(r => r.Adicionar(atendente)).Returns(Task.FromResult(1));
            var atendenteService = new AtendenteService(_atendenteRepository.Object, _notificador.Object);

            //Act
            var retorno = await atendenteService.Adicionar(atendente);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.Object.TemNotificacao());
            _atendenteRepository.Verify(r => r.Adicionar(atendente), Times.Once);
        }

        [Fact(DisplayName = "Atendente erro na validação ao cadastrar")]
        [Trait("Services", "Atendente")]
        public async Task ErroValidacaoCadastraAtendente()
        {
            //Arrange
            var atendente = new Atendente();
            _atendenteRepository.Setup(r => r.Adicionar(atendente)).Returns(Task.FromResult(1));
            _notificador.Setup(n => n.TemNotificacao()).Returns(true);
            var atendenteService = new AtendenteService(_atendenteRepository.Object, _notificador.Object);

            //Act
            var retorno = await atendenteService.Adicionar(atendente);

            //Assert
            Assert.False(retorno);
            Assert.True(_notificador.Object.TemNotificacao());
            _atendenteRepository.Verify(r => r.Adicionar(atendente), Times.Never);
        }

        [Fact(DisplayName = "Atendente alterado com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task AlterarAtendente()
        {
            //Arrange
            var atendente = new Atendente
            {
                Nome = "Rodrigo",
                Email = "rodrigodotnet@gmail.com",
                Telefone = new Telefone
                {
                    Ddd = 19,
                    Numero = 998861788,
                    TipoTelefone = ETipoTelefone.Celular
                }
            };
            _atendenteRepository.Setup(r => r.Atualizar(atendente)).Returns(Task.FromResult(1));
            var atendenteService = new AtendenteService(_atendenteRepository.Object, _notificador.Object);

            //Act
            var retorno = await atendenteService.Atualizar(atendente);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.Object.TemNotificacao());
            _atendenteRepository.Verify(r => r.Atualizar(atendente), Times.Once);
        }

        [Fact(DisplayName = "Atendente erro na validação ao alterar")]
        [Trait("Services", "Atendente")]
        public async Task ErroValidacaoAtualizarAtendente()
        {
            //Arrange
            var atendente = new Atendente();
            _atendenteRepository.Setup(r => r.Atualizar(atendente)).Returns(Task.FromResult(1));
            _notificador.Setup(n => n.TemNotificacao()).Returns(true);
            var atendenteService = new AtendenteService(_atendenteRepository.Object, _notificador.Object);

            //Act
            var retorno = await atendenteService.Adicionar(atendente);

            //Assert
            Assert.False(retorno);
            Assert.True(_notificador.Object.TemNotificacao());
            _atendenteRepository.Verify(r => r.Atualizar(atendente), Times.Never);
        }
    }
}
