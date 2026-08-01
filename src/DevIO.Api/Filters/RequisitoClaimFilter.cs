using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Restaurante.IO.Api.Errors;
using Restaurante.IO.Api.Extensions.Clains;
using Restaurante.IO.Api.Results;

namespace Restaurante.IO.Api.Filters
{
    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new ProblemDetailsResult(ApiProblemDetails.Create(
                    context.HttpContext,
                    StatusCodes.Status401Unauthorized,
                    ApiProblemDetails.AuthenticationType,
                    "Autenticacao necessaria.",
                    "A chamada precisa ser efetuada por um usuario autenticado."));
                return;
            }

            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
            {
                context.Result = new ProblemDetailsResult(ApiProblemDetails.Create(
                    context.HttpContext,
                    StatusCodes.Status403Forbidden,
                    ApiProblemDetails.AuthorizationType,
                    "Acesso negado.",
                    "O usuario esta autenticado, mas nao possui permissao para executar essa acao."));
            }
        }
    }
}
