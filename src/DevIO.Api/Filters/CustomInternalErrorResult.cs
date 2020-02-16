using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurante.IO.Api.Extensions;
using Restaurante.IO.Api.Extensions.Authorization;

namespace Restaurante.IO.Api.Filters
{
    public class CustomInternalErrorResult : JsonResult
    {
        public CustomInternalErrorResult(CustomResult customResult, int? statusCodes = StatusCodes.Status500InternalServerError)
            : base(new CustomError(customResult))
        {
            if (statusCodes == null)
            {
                statusCodes = 500;
            }

            StatusCode = statusCodes;
        }
    }
}