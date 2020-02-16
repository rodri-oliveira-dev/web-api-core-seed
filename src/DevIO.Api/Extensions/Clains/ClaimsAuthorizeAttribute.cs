using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace Restaurante.IO.Api.Extensions.Clains
{
    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, [CallerMemberName]string claimValue = null) : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] { new System.Security.Claims.Claim(claimName, claimValue) };
        }
    }
}