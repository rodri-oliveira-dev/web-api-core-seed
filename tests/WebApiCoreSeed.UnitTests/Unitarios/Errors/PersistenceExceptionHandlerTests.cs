using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using WebApiCoreSeed.Api.Errors;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Errors
{
    public class PersistenceExceptionHandlerTests
    {
        [Fact(DisplayName = "Persistence handler ignora excecoes que nao sao de persistencia")]
        [Trait("Errors", "ProblemDetails")]
        public async Task TryHandleQuandoExcecaoNaoEhPersistenciaDeveRetornarFalse()
        {
            var handler = CreateHandler();
            var httpContext = CreateHttpContext();

            var handled = await handler.TryHandleAsync(httpContext, new InvalidOperationException("falha"), CancellationToken.None);

            Assert.False(handled);
            Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        }

        [Fact(DisplayName = "Persistence handler retorna conflito para concorrencia")]
        [Trait("Errors", "ProblemDetails")]
        public async Task TryHandleQuandoConflitoDeConcorrenciaDeveRetornarConflictProblemDetails()
        {
            var handler = CreateHandler();
            var httpContext = CreateHttpContext();

            var handled = await handler.TryHandleAsync(httpContext, new DbUpdateConcurrencyException("conflito"), CancellationToken.None);
            var problem = await ReadProblemDetailsAsync(httpContext);

            Assert.True(handled);
            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)httpContext.Response.StatusCode);
            Assert.Equal(ApiProblemDetails.ConflictType, problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty(ApiProblemDetails.TraceIdExtension, out _));
        }

        [Fact(DisplayName = "Persistence handler retorna erro de persistencia para DbUpdate")]
        [Trait("Errors", "ProblemDetails")]
        public async Task TryHandleQuandoFalhaDePersistenciaDeveRetornarPersistenceProblemDetails()
        {
            var handler = CreateHandler();
            var httpContext = CreateHttpContext();

            var handled = await handler.TryHandleAsync(httpContext, new DbUpdateException("falha"), CancellationToken.None);
            var problem = await ReadProblemDetailsAsync(httpContext);

            Assert.True(handled);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)httpContext.Response.StatusCode);
            Assert.Equal(ApiProblemDetails.PersistenceFailureType, problem.GetProperty("type").GetString());
            Assert.True(problem.TryGetProperty(ApiProblemDetails.TraceIdExtension, out _));
        }

        private static PersistenceExceptionHandler CreateHandler()
        {
            return new PersistenceExceptionHandler(NullLogger<PersistenceExceptionHandler>.Instance);
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddProblemDetails()
                .BuildServiceProvider();

            return new DefaultHttpContext
            {
                RequestServices = services,
                Response =
                {
                    Body = new MemoryStream()
                }
            };
        }

        private static async Task<JsonElement> ReadProblemDetailsAsync(HttpContext httpContext)
        {
            httpContext.Response.Body.Position = 0;
            using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);

            return document.RootElement.Clone();
        }
    }
}
