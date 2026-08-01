using Microsoft.AspNetCore.Mvc;

namespace Restaurante.IO.Api.Results
{
    public sealed class ProblemDetailsResult : JsonResult
    {
        public ProblemDetailsResult(ProblemDetails problemDetails)
            : base(problemDetails)
        {
            StatusCode = problemDetails.Status;
            ContentType = "application/problem+json";
        }
    }
}
