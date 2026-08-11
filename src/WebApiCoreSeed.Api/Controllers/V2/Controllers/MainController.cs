using Microsoft.AspNetCore.Mvc;
using WebApiCoreSeed.SampleRestaurant.Intefaces;

namespace WebApiCoreSeed.Api.Controllers.V2.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public abstract class MainController : WebApiCoreSeed.Api.Controllers.MainControllerBase
    {
        protected MainController(INotificador notificador) : base(notificador)
        {
        }
    }
}
