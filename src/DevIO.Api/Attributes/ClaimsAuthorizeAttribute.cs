using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Restaurante.IO.Api.Extensions.Clains;
using Restaurante.IO.Api.Filters;

namespace Restaurante.IO.Api.Attributes
{
    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, [CallerMemberName]string claimValue = null) : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] { new System.Security.Claims.Claim(claimName, claimValue) };
        }
    }
}