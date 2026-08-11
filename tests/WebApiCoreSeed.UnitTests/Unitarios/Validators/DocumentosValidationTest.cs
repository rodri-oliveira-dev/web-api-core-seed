using WebApiCoreSeed.SampleRestaurant.Models.Validations.Documentos;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Validators
{
    public class DocumentosValidationTest
    {
        [Theory(DisplayName = "CPF valido passa validacao")]
        [Trait("Validators", "Documentos")]
        [InlineData("52998224725")]
        [InlineData("529.982.247-25")]
        public void CpfQuandoValidoDevePassarValidacao(string cpf)
        {
            Assert.True(CpfValidacao.Validar(cpf));
        }

        [Theory(DisplayName = "CPF invalido falha validacao")]
        [Trait("Validators", "Documentos")]
        [InlineData("123")]
        [InlineData("00000000000")]
        [InlineData("52998224726")]
        public void CpfQuandoInvalidoDeveFalharValidacao(string cpf)
        {
            Assert.False(CpfValidacao.Validar(cpf));
        }

        [Theory(DisplayName = "CNPJ valido passa validacao")]
        [Trait("Validators", "Documentos")]
        [InlineData("11222333000181")]
        [InlineData("11.222.333/0001-81")]
        public void CnpjQuandoValidoDevePassarValidacao(string cnpj)
        {
            Assert.True(CnpjValidacao.Validar(cnpj));
        }

        [Theory(DisplayName = "CNPJ invalido falha validacao")]
        [Trait("Validators", "Documentos")]
        [InlineData("123")]
        [InlineData("00000000000000")]
        [InlineData("11222333000182")]
        public void CnpjQuandoInvalidoDeveFalharValidacao(string cnpj)
        {
            Assert.False(CnpjValidacao.Validar(cnpj));
        }

        [Fact(DisplayName = "Apenas numeros remove caracteres de formatacao")]
        [Trait("Validators", "Documentos")]
        public void ApenasNumerosQuandoValorFormatadoDeveRetornarSomenteDigitos()
        {
            Assert.Equal("11222333000181", Utils.ApenasNumeros("11.222.333/0001-81"));
        }

        [Fact(DisplayName = "Digito verificador vazio retorna vazio")]
        [Trait("Validators", "Documentos")]
        public void DigitoVerificadorQuandoNumeroVazioDeveRetornarVazio()
        {
            var digitoVerificador = new DigitoVerificador(string.Empty);

            Assert.Equal(string.Empty, digitoVerificador.CalculaDigito());
        }

        [Fact(DisplayName = "Digito verificador aplica substituicao configurada")]
        [Trait("Validators", "Documentos")]
        public void DigitoVerificadorQuandoResultadoSubstituidoDeveRetornarSubstituto()
        {
            var digitoVerificador = new DigitoVerificador("0")
                .ComMultiplicadoresDeAte(2, 2)
                .Substituindo("0", 11);

            Assert.Equal("0", digitoVerificador.CalculaDigito());
        }
    }
}
