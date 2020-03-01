using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Restaurante.IO.Api.Extensions;
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
                context.Result = new CustomUnauthorizedResult(new CustomResult(false, "A chamada precisa ser efetuada por um usuario autenticado."));
                return;
            }

            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
            {
                context.Result = new CustomForbiddenResult(new CustomResult(false, "O usuário esta autenticado, mas o não possui permissão para executar essa ação."));
            }
        }
    }
}