using System.Threading.Tasks;
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
using WebApiCoreSeed.Api.ViewModels.User;
using WebApiCoreSeed.SampleRestaurant.Interfaces;

namespace WebApiCoreSeed.Api.Controllers.V1.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ApiVersion("1.0", Deprecated = true)]
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
                SecurityAlgorithms.HmacSha384Signature)
        {
        }

        //[EnableCors("Development")]
        [Authorize]
        [HttpPost("nova-conta")]
        [ProducesResponseType(typeof(RegisterUserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await CreateUserAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                await SignInUserAsync(user);
                return CustomResponse(await GerarJwt(user.Email ?? registerUser.Email));
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(registerUser);
        }
    }
}
