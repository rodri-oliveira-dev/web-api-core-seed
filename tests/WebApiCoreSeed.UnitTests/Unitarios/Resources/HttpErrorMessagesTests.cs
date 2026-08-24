using System.Reflection;
using WebApiCoreSeed.Api;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Resources;

public sealed class HttpErrorMessagesTests
{
    [Theory]
    [InlineData(400, "sintaxe")]
    [InlineData(401, "autenticado")]
    [InlineData(403, "permiss")]
    [InlineData(404, "encontrada")]
    [InlineData(405, "suportado")]
    [InlineData(418, "I'm a teapot")]
    public void RetornaMensagemErroQuandoStatusConhecidoOuPadraoDeveRetornarMensagem(int statusCode, string expectedText)
    {
        var message = InvokeMessage(statusCode);

        Assert.Contains(expectedText, message, StringComparison.OrdinalIgnoreCase);
    }

    private static string InvokeMessage(int statusCode)
    {
        var type = typeof(Program).Assembly.GetType("WebApiCoreSeed.Api.Resources.HttpErrorMessages", throwOnError: true)!;
        var method = type.GetMethod("RetornaMensagemErro", BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException("Metodo RetornaMensagemErro nao encontrado.");

        return Assert.IsType<string>(method.Invoke(null, new object[] { statusCode }));
    }
}
