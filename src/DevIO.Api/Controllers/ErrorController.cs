using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Restaurante.IO.Api.Controllers.V1.Controllers;
using Restaurante.IO.Business.Intefaces;

namespace Restaurante.IO.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<PratosController> _logger;
        private readonly IUser _user;
        private const string ErrorMessage = "Essa ação não deve ser chamado fora do ambientes de não desenvolvimento.";

        public ErrorController(ILogger<PratosController> logger, IUser user)
        {
            _logger = logger;
            _user = user;
        }

        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            _logger.LogError("err");
            return Problem(
                context.Error.StackTrace,
                title: context.Error.Message,
                statusCode: 500);
        }

        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            _logger.LogError("err01");
            return Problem();
        }

        [Route("/error/{id}")]
        public IActionResult ErrorId(int id)
        {
            _logger.LogError("err02");
            return Problem(
                RetornaMensagemErro(id),
                title: RetornaMensagemErro(id),
                statusCode: id);
        }

        private static string RetornaMensagemErro(int statusCode)
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