using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurante.IO.Api.Extensions;

namespace Restaurante.IO.Api.Results
{
    public class CustomForbiddenResult : JsonResult
    {
        public CustomForbiddenResult(CustomResult customResult) 
            : base(new CustomErrorResult(customResult))
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}