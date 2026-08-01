using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Restaurante.IO.Api.Errors
{
    public sealed class PersistenceExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<PersistenceExceptionHandler> _logger;

        public PersistenceExceptionHandler(ILogger<PersistenceExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                _logger.LogWarning(exception, "Persistence concurrency conflict while handling request.");
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

                return await WriteAsync(
                    httpContext,
                    StatusCodes.Status409Conflict,
                    ApiProblemDetails.ConflictType,
                    "Conflito com o estado atual do recurso.",
                    "A operacao nao pode ser concluida porque o recurso foi alterado.");
            }

            if (exception is not DbUpdateException)
            {
                return false;
            }

            _logger.LogError(exception, "Persistence failure while handling request.");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return await WriteAsync(
                httpContext,
                StatusCodes.Status500InternalServerError,
                ApiProblemDetails.PersistenceFailureType,
                "Falha de persistencia.",
                "Ocorreu um erro ao persistir os dados.");
        }

        private static async ValueTask<bool> WriteAsync(
            HttpContext httpContext,
            int statusCode,
            string type,
            string title,
            string detail)
        {
            var problemDetails = ApiProblemDetails.Create(httpContext, statusCode, type, title, detail);
            var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
        }
    }
}
