using FluentValidation.TestHelper;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;
using Xunit;

namespace Pedidos.Test.Unitarios.Validators
{
    public class PratoValidationTest
    {
        [Fact(DisplayName = "Prato falha validação, campos obrigatorios")]
        [Trait("Validators", "Prato")]
        public void PratoFalhaValidacaoCamposObrigatorios()
        {
            //Arrange
            var prato = new Prato();
            var validator = new PratoValidation();

            //Act
            var resultado = validator.TestValidate(prato);

            //Assert
            resultado.ShouldHaveValidationErrorFor(p => p.Titulo);
            resultado.ShouldHaveValidationErrorFor(p => p.Descricao);
            resultado.ShouldHaveValidationErrorFor(p => p.Foto);
            resultado.ShouldHaveValidationErrorFor(p => p.Preco);
        }

        [Fact(DisplayName = "Prato passa validação, campos obrigatorios")]
        [Trait("Validators", "Prato")]
        public void PratoPassaValidacaoCamposObrigatorios()
        {
            //Arrange
            var prato = new Prato
            {
                Titulo = "X-Tudo",
                Descricao = "Lanche",
                Foto = "x-tudo.png",
                Preco = 10.5
            };

            var validator = new PratoValidation();

            //Act
            var resultado = validator.TestValidate(prato);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(p => p.TipoPrato);
            resultado.ShouldNotHaveValidationErrorFor(p => p.Titulo);
            resultado.ShouldNotHaveValidationErrorFor(p => p.Descricao);
            resultado.ShouldNotHaveValidationErrorFor(p => p.Foto);
            resultado.ShouldNotHaveValidationErrorFor(p => p.Preco);
        }
    }
}
