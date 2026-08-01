using FluentValidation.TestHelper;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;
using Xunit;

namespace Pedidos.Test.Unitarios.Validators
{
    public class PedidoPratoValidationTest
    {
        [Fact(DisplayName = "Pedido Prato falha validação, campos obrigatorios")]
        [Trait("Validators", "PedidoPrato")]
        public void PedidoPratoQuandoPratoAusenteDeveFalharValidacao()
        {
            //Arrange
            var pedidoPrato = new PedidoPrato();
            var validator = new PedidoPratoValidation();

            //Act
            var resultado = validator.TestValidate(pedidoPrato);

            //Assert
            resultado.ShouldHaveValidationErrorFor(pp => pp.Prato);
        }

        [Fact(DisplayName = "Pedido Prato falha validação, campo observação maior que o aceito")]
        [Trait("Validators", "PedidoPrato")]
        public void PedidoPratoQuandoObservacaoMaiorQueLimiteDeveFalharValidacao()
        {
            //Arrange
            var pedidoPrato = new PedidoPrato
            {
                Prato = new Prato(),
                Observacao = new string('a', 1001)
            };
            var validator = new PedidoPratoValidation();

            //Act
            var resultado = validator.TestValidate(pedidoPrato);

            //Assert
            resultado.ShouldHaveValidationErrorFor(pp => pp.Observacao);
        }

        [Fact(DisplayName = "Pedido Prato passa validação")]
        [Trait("Validators", "PedidoPrato")]
        public void PedidoPratoQuandoCamposValidosDevePassarValidacao()
        {
            //Arrange
            var pedidoPrato = new PedidoPrato
            {
                Pedido = new Pedido(),
                Prato = new Prato(),
            };
            var validator = new PedidoPratoValidation();

            //Act
            var resultado = validator.TestValidate(pedidoPrato);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(pp => pp.Prato);
            resultado.ShouldNotHaveValidationErrorFor(pp => pp.Observacao);
        }

        [Fact(DisplayName = "Pedido Prato aceita observação dentro do limite")]
        [Trait("Validators", "PedidoPrato")]
        public void PedidoPratoQuandoObservacaoDentroDoLimiteDevePassarValidacao()
        {
            //Arrange
            var pedidoPrato = new PedidoPrato
            {
                Prato = new Prato(),
                Observacao = "Sem cebola"
            };
            var validator = new PedidoPratoValidation();

            //Act
            var resultado = validator.TestValidate(pedidoPrato);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(pp => pp.Prato);
            resultado.ShouldNotHaveValidationErrorFor(pp => pp.Observacao);
        }
    }
}
