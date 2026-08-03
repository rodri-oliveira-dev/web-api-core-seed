using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using WebApiCoreSeed.Api.Extensions.Clains;
using WebApiCoreSeed.Api.Filters;

namespace WebApiCoreSeed.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, [CallerMemberName] string claimValue = "") : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] { new System.Security.Claims.Claim(claimName, claimValue) };
        }
    }
}
