using FluentValidation.TestHelper;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;
using Xunit;

namespace Pedidos.Test.Unitarios.Validators
{
    public class MesaValidationTest
    {
        [Fact(DisplayName = "Mesa falha validação")]
        [Trait("Validators", "Mesa")]
        public void MesaFalhaValidacao()
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
        public void MesaPassaValidacao()
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
