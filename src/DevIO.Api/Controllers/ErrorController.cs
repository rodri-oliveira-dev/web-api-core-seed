using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Restaurante.IO.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private const string ErrorMessage = "Essa ação não deve ser chamado fora do ambientes de não desenvolvimento.";

        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                context.Error.StackTrace,
                title: context.Error.Message,
                statusCode: 500);
        }

        [Route("/error")]
        public IActionResult Error() => Problem();

        [Route("/error/{id}")]
        public IActionResult ErrorId(int id)
        {
            return Problem(
                RetornaMensagemErro(id),
                title: RetornaMensagemErro(id),
                statusCode: id);
        }

        private string RetornaMensagemErro(int statusCode)
        {
            return statusCode switch
            {
                400 => "O pedido não pode ser cumprido devido à erro de sintaxe.",
                401 => "A chamada precisa ser efetuada por um usuario autenticado.",
                403 => "O usuário esta autenticado, mas o não possui permissão para executar essa ação.",
                404 => "A página solicitada não pôde ser encontrada, mas pode estar disponível novamente no futuro.",
                405 => "Foi feita uma solicitação de uma página usando um método de solicitação não suportado por essa página.",
                _ => ReasonPhrases.GetReasonPhrase(statusCode)
            };
        }
    }
}