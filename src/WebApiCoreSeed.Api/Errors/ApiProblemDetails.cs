using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApiCoreSeed.Api.Results;
using WebApiCoreSeed.Api.Resources;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;

namespace WebApiCoreSeed.Api.Errors
{
    public static class ApiProblemDetails
    {
        public const string ValidationType = "urn:problem:validation";
        public const string DomainRuleType = "urn:problem:domain-rule";
        public const string ConflictType = "urn:problem:conflict";
        public const string AuthenticationType = "urn:problem:authentication";
        public const string AuthorizationType = "urn:problem:authorization";
        public const string NotFoundType = "urn:problem:not-found";
        public const string RateLimitType = "urn:problem:rate-limit";
        public const string PersistenceFailureType = "urn:problem:persistence-failure";
        public const string UnexpectedType = "urn:problem:unexpected-error";

        public const string TraceIdExtension = "traceId";
        public const string ErrorsExtension = "errors";

        public static ProblemDetails Create(HttpContext httpContext, int statusCode, string type, string title, string? detail = null)
        {
            var problemDetails = new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            AddTraceId(problemDetails, httpContext);

            return problemDetails;
        }

        public static ValidationProblemDetails CreateValidation(HttpContext httpContext, ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, string[]>();
            foreach (var entry in modelState)
            {
                if (entry.Value is not { Errors.Count: > 0 } value)
                {
                    continue;
                }

                errors[entry.Key] = value.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Valor informado inválido."
                        : error.ErrorMessage)
                    .ToArray();
            }

            return CreateValidation(httpContext, errors);
        }

        public static ValidationProblemDetails CreateValidation(HttpContext httpContext, IDictionary<string, string[]> errors)
        {
            var problemDetails = new ValidationProblemDetails(errors)
            {
                Type = ValidationType,
                Title = "Validação da requisição falhou.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Corrija os campos indicados e tente novamente.",
                Instance = httpContext.Request.Path
            };

            AddTraceId(problemDetails, httpContext);

            return problemDetails;
        }

        public static ProblemDetails CreateFromNotifications(HttpContext httpContext, IEnumerable<Notificacao> notifications)
        {
            var messages = notifications.Select(notification => notification.Mensagem).ToArray();
            var isConflict = messages.Any(message => message.StartsWith("Já existe", StringComparison.OrdinalIgnoreCase));
            var statusCode = isConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
            var type = isConflict ? ConflictType : DomainRuleType;
            var title = isConflict ? "Conflito com o estado atual do recurso." : "Regra de domínio violada.";
            var detail = isConflict
                ? "A operação conflita com um recurso existente."
                : "A operação não pode ser concluída com os dados informados.";

            var problemDetails = Create(httpContext, statusCode, type, title, detail);
            problemDetails.Extensions[ErrorsExtension] = new Dictionary<string, string[]>
            {
                ["notifications"] = messages
            };

            return problemDetails;
        }

        public static ProblemDetailsResult ToObjectResult(ProblemDetails problemDetails)
        {
            return new ProblemDetailsResult(problemDetails);
        }

        public static string TypeForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => ValidationType,
                StatusCodes.Status401Unauthorized => AuthenticationType,
                StatusCodes.Status403Forbidden => AuthorizationType,
                StatusCodes.Status404NotFound => NotFoundType,
                StatusCodes.Status409Conflict => ConflictType,
                StatusCodes.Status429TooManyRequests => RateLimitType,
                StatusCodes.Status500InternalServerError => UnexpectedType,
                _ => $"urn:problem:http-status-{statusCode}"
            };
        }

        public static string TitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Requisição inválida.",
                StatusCodes.Status401Unauthorized => "Autenticação necessária.",
                StatusCodes.Status403Forbidden => "Acesso negado.",
                StatusCodes.Status404NotFound => "Recurso não encontrado.",
                StatusCodes.Status409Conflict => "Conflito.",
                StatusCodes.Status429TooManyRequests => "Limite de requisições excedido.",
                StatusCodes.Status500InternalServerError => "Erro interno.",
                _ => HttpErrorMessages.RetornaMensagemErro(statusCode)
            };
        }

        public static string DetailForStatusCode(int statusCode)
        {
            return statusCode == StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado ao processar a requisição."
                : HttpErrorMessages.RetornaMensagemErro(statusCode);
        }

        public static void AddTraceId(ProblemDetails problemDetails, HttpContext httpContext)
        {
            problemDetails.Extensions[TraceIdExtension] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        }
    }
}
