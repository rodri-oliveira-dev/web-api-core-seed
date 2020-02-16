using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Restaurante.IO.Api.Extensions.Authorization
{
    public class CustomForbiddenResult : JsonResult
    {
        public CustomForbiddenResult(CustomResult customResult) 
            : base(new CustomError(customResult))
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}