using Microsoft.AspNetCore.Mvc;

namespace WebApiCoreSeed.Api.Results
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
