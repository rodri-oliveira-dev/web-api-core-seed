using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Restaurante.IO.Api.Results
{
    public class CustomNoContentResult : JsonResult
    {
        public CustomNoContentResult(CustomResult customResult)
            : base(customResult)
        {
            StatusCode = StatusCodes.Status204NoContent;
        }
    }
}
