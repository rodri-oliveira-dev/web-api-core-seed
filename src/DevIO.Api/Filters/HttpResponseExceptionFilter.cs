using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Restaurante.IO.Api.Extensions;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Restaurante.IO.Api.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException exception)
            {
                context.Result = new CustomInternalErrorResult(new CustomResult(false, new
                {
                    Error = exception.Message,
                    StatusCode = exception.Status,
                    Detail = exception.Message
                }), exception.Status);

                context.ExceptionHandled = true;
            }
            else
            {
                if (context.Exception != null)
                {
                    context.Result = new CustomInternalErrorResult(new CustomResult(false, new
                    {
                        Error = context.Exception.Message,
                        StatusCode = 500,
                        Detail = context.Exception.Message
                    }));
                }
                else if (context.Result != null && context.Result is ObjectResult result)
                {
                    if (result.Value is ProblemDetails details)
                    {
                        context.Result = new CustomInternalErrorResult(new CustomResult(false, new
                        {
                            Error = details.Title,
                            StatusCode = details.Status,
                            details.Detail
                        }), result.StatusCode);
                    }
                }
            }
        }
    }
}