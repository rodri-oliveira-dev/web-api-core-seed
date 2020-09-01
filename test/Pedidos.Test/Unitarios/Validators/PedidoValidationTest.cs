using FluentValidation.TestHelper;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Core;
using Restaurante.IO.Business.Models.Enums;
using Restaurante.IO.Business.Models.Validations;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pedidos.Test.Unitarios.Validators
{
    public class PedidoValidationTest
    {
        [Fact(DisplayName = "Pedido falha validação, campos obrigatorios")]
        [Trait("Validators", "Pedido")]
        public void PedidoFalhaValidacaoCamposObrigatorios()
        {
            //Arrange
            var pedido = new Pedido();
            var validator = new PedidoValidation();

            //Act
            var resultado = validator.TestValidate(pedido);

            //Assert
            resultado.ShouldHaveValidationErrorFor(p => p.Mesa);
            resultado.ShouldHaveValidationErrorFor(p => p.Atendente);
            resultado.ShouldHaveValidationErrorFor(p => p.PedidoPrato);
            resultado.ShouldHaveValidationErrorFor(p => p.Numero);
        }

        [Fact(DisplayName = "Pedido passa validação, campos obrigatorios")]
        [Trait("Validators", "Pedido")]
        public void PedidoPassaValidacaoCamposObrigatorios()
        {
            //Arrange
            var pedido = new Pedido
            {
                Atendente = new Atendente
                {
                    Nome = "Rodrigo de Oliveira",
                    Email = "rodrigodotnet@gmail.com",
                    Telefone = new Telefone
                    {
                        Ddd = 19,
                        Numero = 998861786,
                        TipoTelefone = ETipoTelefone.Celular,
                    }
                },
                Mesa = new Mesa
                {
                    LocalizacaoMesa = ELocalizacaoMesa.Interna,
                    Numero = DateTime.Now.AddSeconds(-7).Second.ToString(),
                    Lugares = 4
                },
                PedidoPrato = new List<PedidoPrato>()
                {
                    new PedidoPrato
                    {
                        Prato= new Prato()
                        {
                            Titulo = "X-Burguer",
                            Descricao = "Lanchão",
                            Foto = "x-burguer.jpg",
                            Preco = 25,
                            TipoPrato = ETipoPrato.Comida,
                        },
                        
                    }
                },
                Numero = DateTime.Now.Second.ToString(),
            };
            var validator = new PedidoValidation();

            //Act
            var resultado = validator.TestValidate(pedido);

            //Assert
            resultado.ShouldNotHaveValidationErrorFor(p => p.Mesa);
            resultado.ShouldNotHaveValidationErrorFor(p => p.Atendente);
            resultado.ShouldNotHaveValidationErrorFor(p => p.PedidoPrato);
            resultado.ShouldNotHaveValidationErrorFor(p => p.Numero);
        }
    }
}
