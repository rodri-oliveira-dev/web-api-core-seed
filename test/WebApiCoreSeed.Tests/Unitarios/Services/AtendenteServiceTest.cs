using Moq;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Core;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;
using WebApiCoreSeed.SampleRestaurant.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace WebApiCoreSeed.Tests.Unitarios.Services
{
    public class AtendenteServiceTest
    {
        private readonly Mock<IAtendenteRepository> _atendenteRepository;
        private readonly Notificador _notificador;

        public AtendenteServiceTest()
        {
            _atendenteRepository = new Mock<IAtendenteRepository>(MockBehavior.Strict);
            _notificador = new Notificador();
        }

        [Fact(DisplayName = "Atendente cadastrado com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoAtendenteValidoDeveCadastrarAtendente()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            _atendenteRepository.Setup(r => r.Adicionar(atendente)).ReturnsAsync(1);
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Adicionar(atendente);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.TemNotificacao());
            _atendenteRepository.Verify(r => r.Adicionar(atendente), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente erro na validação ao cadastrar")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoAtendenteInvalidoDeveNotificarENaoCadastrar()
        {
            //Arrange
            var atendente = new Atendente();
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Adicionar(atendente);

            //Assert
            Assert.False(retorno);
            Assert.True(_notificador.TemNotificacao());
            Assert.NotEmpty(_notificador.ObterNotificacoes());
            _atendenteRepository.Verify(r => r.Adicionar(atendente), Times.Never);
            _atendenteRepository.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente alterado com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task AtualizarQuandoAtendenteValidoDeveAtualizarAtendente()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            _atendenteRepository.Setup(r => r.Atualizar(atendente)).ReturnsAsync(1);
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Atualizar(atendente);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.TemNotificacao());
            _atendenteRepository.Verify(r => r.Atualizar(atendente), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente erro na validação ao alterar")]
        [Trait("Services", "Atendente")]
        public async Task AtualizarQuandoAtendenteInvalidoDeveNotificarENaoAtualizar()
        {
            //Arrange
            var atendente = new Atendente();
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Atualizar(atendente);

            //Assert
            Assert.False(retorno);
            Assert.True(_notificador.TemNotificacao());
            Assert.NotEmpty(_notificador.ObterNotificacoes());
            _atendenteRepository.Verify(r => r.Adicionar(atendente), Times.Never);
            _atendenteRepository.Verify(r => r.Atualizar(atendente), Times.Never);
            _atendenteRepository.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente removido com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task RemoverQuandoIdInformadoDeveRemoverAtendente()
        {
            //Arrange
            var id = Guid.Parse("6b0bb7e2-49ca-4b02-bff4-e6fed1007391");
            _atendenteRepository.Setup(r => r.RemoverPorId(id)).ReturnsAsync(1);
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Remover(id);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.TemNotificacao());
            _atendenteRepository.Verify(r => r.RemoverPorId(id), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
        }

        private AtendenteService CriarService()
        {
            return new AtendenteService(_atendenteRepository.Object, _notificador);
        }

        private static Atendente CriarAtendenteValido()
        {
            return new Atendente
            {
                Nome = "Rodrigo",
                Email = "rodrigodotnet@gmail.com",
                Telefone = new Telefone
                {
                    Ddd = 19,
                    Numero = 998861788,
                    TipoTelefone = ETipoTelefone.Celular
                },
                TipoAtendente = ETipoAtendente.Garcom
            };
        }
    }
}
