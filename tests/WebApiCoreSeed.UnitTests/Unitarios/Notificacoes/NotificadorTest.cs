using WebApiCoreSeed.SampleRestaurant.Notificacoes;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Notificacoes
{
    public class NotificadorTest
    {

        [Fact(DisplayName = "Notificador instanciado vazio")]
        [Trait("Business", "Notificacoes")]
        public void NotificadorQuandoInstanciadoDeveIniciarSemNotificacoes()
        {
            //Arrange
            Notificador notificador = new Notificador();

            //Act


            //Assert
            Assert.False(notificador.TemNotificacao());
            Assert.Empty(notificador.ObterNotificacoes());
        }

        [Fact(DisplayName = "Adicionando notificação")]
        [Trait("Business", "Notificacoes")]
        public void HandleQuandoNotificacaoInformadaDeveRegistrarNotificacao()
        {
            //Arrange
            Notificador notificador = new Notificador();

            //Act
            notificador.Handle(new Notificacao("Teste"));

            //Assert
            Assert.True(notificador.TemNotificacao());
            var notificacao = Assert.Single(notificador.ObterNotificacoes());
            Assert.Equal("Teste", notificacao.Mensagem);
        }
    }
}
