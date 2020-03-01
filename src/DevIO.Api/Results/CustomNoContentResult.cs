using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurante.IO.Api.Extensions;

namespace Restaurante.IO.Api.Results
{
    public class CustomNoContentResult : JsonResult
    {
        public CustomNoContentResult(CustomResult customResult) 
            : base(new CustomErrorResult(customResult))
        {
            StatusCode = StatusCodes.Status204NoContent;
        }
    }
}