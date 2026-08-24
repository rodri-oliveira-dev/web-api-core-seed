using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApiCoreSeed.Api.Errors
{
    public sealed class PersistenceExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<PersistenceExceptionHandler> _logger;
        private static readonly Action<ILogger, Exception?> LogPersistenceConcurrencyConflict =
            LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(1000, nameof(LogPersistenceConcurrencyConflict)),
                "Persistence concurrency conflict while handling request.");

        private static readonly Action<ILogger, Exception?> LogPersistenceFailure =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1001, nameof(LogPersistenceFailure)),
                "Persistence failure while handling request.");

        public PersistenceExceptionHandler(ILogger<PersistenceExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                LogPersistenceConcurrencyConflict(_logger, exception);
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

                return await WriteAsync(
                    httpContext,
                    StatusCodes.Status409Conflict,
                    ApiProblemDetails.ConflictType,
                    "Conflito com o estado atual do recurso.",
                    "A operação não pode ser concluída porque o recurso foi alterado.");
            }

            if (exception is not DbUpdateException)
            {
                return false;
            }

            LogPersistenceFailure(_logger, exception);
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
