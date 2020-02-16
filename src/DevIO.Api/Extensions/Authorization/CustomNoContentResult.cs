using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Restaurante.IO.Api.Extensions.Authorization
{
    public class CustomNoContentResult : JsonResult
    {
        public CustomNoContentResult(CustomResult customResult) 
            : base(new CustomError(customResult))
        {
            StatusCode = StatusCodes.Status204NoContent;
        }
    }
}