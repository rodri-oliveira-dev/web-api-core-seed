using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Restaurante.IO.Api.Extensions.Authorization
{
    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(CustomResult customResult) 
            : base(new CustomError(customResult))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}