using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApiCoreSeed.Api.Errors
{
    public sealed class UnhandledExceptionHandler : IExceptionHandler
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<UnhandledExceptionHandler> _logger;
        private static readonly Action<ILogger, Exception?> LogUnhandledException =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1000, nameof(LogUnhandledException)),
                "Unhandled exception while handling request.");

        public UnhandledExceptionHandler(IHostEnvironment environment, ILogger<UnhandledExceptionHandler> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is OperationCanceledException)
            {
                return false;
            }

            LogUnhandledException(_logger, exception);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var detail = _environment.IsDevelopment()
                ? exception.Message
                : "Ocorreu um erro inesperado ao processar a requisicao.";

            var problemDetails = ApiProblemDetails.Create(
                httpContext,
                StatusCodes.Status500InternalServerError,
                ApiProblemDetails.UnexpectedType,
                "Erro interno.",
                detail);

            var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
        }
    }
}
