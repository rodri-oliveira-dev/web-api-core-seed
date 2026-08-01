using FluentValidation.TestHelper;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Validators
{
    public class PratoValidationTest
    {
        [Fact(DisplayName = "Prato falha validação, campos obrigatorios")]
        [Trait("Validators", "Prato")]
        public void PratoQuandoCamposObrigatoriosInvalidosDeveFalharValidacao()
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
        public void PratoQuandoCamposObrigatoriosValidosDevePassarValidacao()
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
