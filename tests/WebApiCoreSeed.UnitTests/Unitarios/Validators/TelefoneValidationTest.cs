using FluentValidation.TestHelper;
using WebApiCoreSeed.SampleRestaurant.Models.Core;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Validators
{
    public class TelefoneValidationTest
    {
        [Fact(DisplayName = "Telefone falha validação")]
        [Trait("Validators", "Telefone")]
        public void TelefoneQuandoCamposInvalidosDeveFalharValidacao()
        {
            //Arrange
            var telefone = new Telefone();
            var validator = new TelefoneValidation();

            //Act
            var resultado = validator.TestValidate(telefone);

            //Assert
            resultado.ShouldHaveValidationErrorFor(t => t.Ddd);
            resultado.ShouldHaveValidationErrorFor(t => t.Numero);
        }

        [Fact(DisplayName = "Telefone passa validação")]
        [Trait("Validators", "Telefone")]
        public void TelefoneQuandoCamposValidosDevePassarValidacao()
        {
            //Arrange
            var telefone = new Telefone { Ddd = 19, Numero = 998861787, TipoTelefone = ETipoTelefone.Celular };
            var validator = new TelefoneValidation();

            //Act
            var resultado = validator.TestValidate(telefone);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(t => t.Ddd);
            resultado.ShouldNotHaveValidationErrorFor(t => t.Numero);
            resultado.ShouldNotHaveValidationErrorFor(t => t.TipoTelefone);
        }
    }
}
