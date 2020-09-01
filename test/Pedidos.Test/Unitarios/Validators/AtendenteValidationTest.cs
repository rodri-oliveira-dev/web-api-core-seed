using FluentValidation.TestHelper;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Core;
using Restaurante.IO.Business.Models.Enums;
using Restaurante.IO.Business.Models.Validations;
using Xunit;

namespace Pedidos.Test.Unitarios.Validators
{
    public class AtendenteValidationTest
    {
        [Fact(DisplayName = "Atendente tipo garçon falha validação")]
        [Trait("Validators", "Atendente")]
        public void AtendenteTipoGarconFalhaValidacao()
        {
            //Arrange
            var atendente = new Atendente();
            var validator = new AtendenteValidation();

            //Act
            var resultado = validator.TestValidate(atendente);

            //Assert
            resultado.ShouldHaveValidationErrorFor(atendente => atendente.Nome);
            resultado.ShouldHaveValidationErrorFor(atendente => atendente.Email);
            resultado.ShouldHaveValidationErrorFor(atendente => atendente.Telefone);
        }

        [Fact(DisplayName = "Atendente tipo garçom passa validação")]
        [Trait("Validators", "Atendente")]
        public void AtendenteTipoGarconPassaValidacao()
        {
            //Arrange
            var atendente = new Atendente { Nome = "Rodrigo de Oliveira", Email = "rodrigodotnet@gmail.com", Telefone = new Telefone { Ddd = 19, Numero = 998861785 } };
            var validator = new AtendenteValidation();

            //Act
            var resultado = validator.TestValidate(atendente);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(atendente => atendente.Nome);
            resultado.ShouldNotHaveValidationErrorFor(atendente => atendente.Email);
            resultado.ShouldNotHaveValidationErrorFor(atendente => atendente.Telefone);
        }

        [Fact(DisplayName = "Atendente tipo Totem falha validação")]
        [Trait("Validators", "Atendente")]
        public void AtendenteTipoTotemFalhaValidacao()
        {
            //Arrange
            var atendente = new Atendente { TipoAtendente = ETipoAtendente.Totem };
            var validator = new AtendenteValidation();

            //Act
            var resultado = validator.TestValidate(atendente);

            //Assert
            resultado.ShouldHaveValidationErrorFor(atendente => atendente.Nome);
        }

        [Fact(DisplayName = "Atendente tipo Totem passa validação")]
        [Trait("Validators", "Atendente")]
        public void AtendenteTipoTotemPassaValidacao()
        {
            //Arrange
            var atendente = new Atendente { Nome = "Totem Shop Patio", TipoAtendente = ETipoAtendente.Totem };
            var validator = new AtendenteValidation();

            //Act
            var resultado = validator.TestValidate(atendente);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(atendente => atendente.Nome);
        }
    }
}
