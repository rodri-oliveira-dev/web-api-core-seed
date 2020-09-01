using FluentValidation.TestHelper;
using Restaurante.IO.Business.Models.Core;
using Restaurante.IO.Business.Models.Enums;
using Restaurante.IO.Business.Models.Validations;
using Xunit;

namespace Pedidos.Test.Unitarios.Validators
{
    public class TelefoneValidationTest
    {
        [Fact(DisplayName = "Telefone falha validação")]
        [Trait("Validators", "Telefone")]
        public void TelefoneFalhaValidacao()
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
        public void TelefonePassaValidacao()
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
