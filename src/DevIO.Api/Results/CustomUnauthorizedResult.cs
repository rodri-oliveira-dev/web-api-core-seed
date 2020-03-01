using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurante.IO.Api.Extensions;

namespace Restaurante.IO.Api.Results
{
    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(CustomResult customResult) 
            : base(new CustomErrorResult(customResult))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}