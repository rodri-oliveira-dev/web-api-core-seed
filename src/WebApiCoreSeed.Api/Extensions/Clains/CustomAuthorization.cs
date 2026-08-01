using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace WebApiCoreSeed.Api.Extensions.Clains
{
    public static class CustomAuthorization
    {
        public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                if (context.User.Claims.Any(c => c.Type == claimName && string.Equals(c.Value, "*", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
                return context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

    }
}
