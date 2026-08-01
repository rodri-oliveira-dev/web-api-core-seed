using FluentValidation.TestHelper;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Core;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using Xunit;

namespace WebApiCoreSeed.Tests.Unitarios.Validators
{
    public class AtendenteValidationTest
    {
        [Fact(DisplayName = "Atendente tipo garçon falha validação")]
        [Trait("Validators", "Atendente")]
        public void GarcomQuandoCamposObrigatoriosAusentesDeveFalharValidacao()
        {
            //Arrange
            var atendente = new Atendente { TipoAtendente = ETipoAtendente.Garcom };
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
        public void GarcomQuandoCamposObrigatoriosValidosDevePassarValidacao()
        {
            //Arrange
            var atendente = new Atendente
            {
                Nome = "Rodrigo de Oliveira",
                Email = "rodrigodotnet@gmail.com",
                Telefone = new Telefone
                {
                    Ddd = 19,
                    Numero = 998861785,
                    TipoTelefone = ETipoTelefone.Celular
                },
                TipoAtendente = ETipoAtendente.Garcom
            };
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
        public void TotemQuandoNomeAusenteDeveFalharValidacao()
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
        public void TotemQuandoNomeValidoDevePassarValidacao()
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
