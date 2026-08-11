using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApiCoreSeed.Api.Configuration;
using WebApiCoreSeed.Api.Settings;
using WebApiCoreSeed.SampleRestaurant.Intefaces;

namespace WebApiCoreSeed.Api.Controllers.V2.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}")]
    [EnableRateLimiting(NativeRateLimitPolicies.AuthenticationSensitive)]
    public class AuthController : WebApiCoreSeed.Api.Controllers.AuthControllerBase
    {
        public AuthController(
            INotificador notificador,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings) : base(
                notificador,
                signInManager,
                userManager,
                appSettings,
                SecurityAlgorithms.HmacSha256Signature)
        {
        }
    }
}
