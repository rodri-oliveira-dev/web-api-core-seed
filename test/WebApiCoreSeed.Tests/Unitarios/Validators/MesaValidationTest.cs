using FluentValidation.TestHelper;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using Xunit;

namespace WebApiCoreSeed.Tests.Unitarios.Validators
{
    public class MesaValidationTest
    {
        [Fact(DisplayName = "Mesa falha validação")]
        [Trait("Validators", "Mesa")]
        public void MesaQuandoCamposObrigatoriosInvalidosDeveFalharValidacao()
        {
            //Arrange
            var mesa = new Mesa();
            var validator = new MesaValidation();

            //Act
            var resultado = validator.TestValidate(mesa);

            //Assert
            resultado.ShouldHaveValidationErrorFor(m => m.Numero);
            resultado.ShouldHaveValidationErrorFor(m => m.Lugares);
        }

        [Fact(DisplayName = "Mesa passa validação")]
        [Trait("Validators", "Mesa")]
        public void MesaQuandoCamposObrigatoriosValidosDevePassarValidacao()
        {
            //Arrange
            var mesa = new Mesa { Numero = "07", Lugares = 4 };
            var validator = new MesaValidation();

            //Act
            var resultado = validator.TestValidate(mesa);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(m => m.Numero);
            resultado.ShouldNotHaveValidationErrorFor(m => m.Lugares);
        }
    }
}
