using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiCoreSeed.Api.Results
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
