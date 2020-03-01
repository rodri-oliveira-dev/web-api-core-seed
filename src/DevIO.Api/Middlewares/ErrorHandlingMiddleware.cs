using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Restaurante.IO.Api.Resources;
using Restaurante.IO.Api.Results;


namespace Restaurante.IO.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            const int code = (int) HttpStatusCode.InternalServerError;

            _logger.LogError(ex, HttpErrorMessages.RetornaMensagemErro(code));

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new CustomResult(false, new
            {
                statusCode = code,
                errorMessage = HttpErrorMessages.RetornaMensagemErro(code)
            })));
        }
    }
}