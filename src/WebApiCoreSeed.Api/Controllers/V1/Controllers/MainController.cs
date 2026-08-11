using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCoreSeed.SampleRestaurant.Intefaces;

namespace WebApiCoreSeed.Api.Controllers.V1.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public abstract class MainController : WebApiCoreSeed.Api.Controllers.MainControllerBase
    {
        protected MainController(INotificador notificador) : base(notificador)
        {
        }
    }
}
