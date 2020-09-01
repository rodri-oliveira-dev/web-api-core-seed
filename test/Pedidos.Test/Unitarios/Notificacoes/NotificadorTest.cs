using Restaurante.IO.Business.Notificacoes;
using Xunit;

namespace Pedidos.Test.Unitarios.Notificacoes
{
    public class NotificadorTest
    {

        [Fact(DisplayName = "Notificador instanciado vazio")]
        [Trait("Business", "Notificacoes")]
        public void IniciaNotificadorInstanciado()
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
        public void AdicionaNotificacao()
        {
            //Arrange
            Notificador notificador = new Notificador();

            //Act
            notificador.Handle(new Notificacao("Teste"));

            //Assert
            Assert.True(notificador.TemNotificacao());
            Assert.Single(notificador.ObterNotificacoes());
        }
    }
}
