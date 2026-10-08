using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WebApiCoreSeed.Api.Services.Interfaces;
using WebApiCoreSeed.Api.Settings;

namespace WebApiCoreSeed.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter, IAsyncResultFilter
    {
        private static readonly object PendingCacheKey = new();
        private readonly int _timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeToLiveSeconds);
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();

            // Only share cache entries for public, anonymous GET requests. An authenticated
            // request (or one carrying credentials) may produce user-specific content.
            var request = context.HttpContext.Request;
            if (!cacheSettings.Enabled
                || !HttpMethods.IsGet(request.Method)
                || context.HttpContext.User.Identity?.IsAuthenticated == true
                || request.Headers.ContainsKey("Authorization")
                || request.Headers.ContainsKey("Cookie"))
            {
                await next();
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cancellationToken = context.HttpContext.RequestAborted;

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            var executedContext = await next();

            // Defer the write until MVC has executed the result. Result filters can
            // change headers and ObjectResult determines its effective status later.
            if (!executedContext.Canceled
                && executedContext.Exception == null
                && executedContext.Result is ObjectResult)
            {
                context.HttpContext.Items[PendingCacheKey] = cacheKey;
            }
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var executedContext = await next();

            if (context.HttpContext.Items.TryGetValue(PendingCacheKey, out var pendingKey)
                && pendingKey is string cacheKey
                && !executedContext.Canceled
                && executedContext.Exception == null
                && !context.HttpContext.RequestAborted.IsCancellationRequested
                && context.HttpContext.Response.StatusCode == StatusCodes.Status200OK
                && !context.HttpContext.Response.Headers.ContainsKey("Set-Cookie")
                && executedContext.Result is ObjectResult objectResult)
            {
                var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
                await cacheService.CacheResponseAsync(
                    cacheKey,
                    objectResult.Value,
                    TimeSpan.FromSeconds(_timeToLiveSeconds),
                    context.HttpContext.RequestAborted);
            }
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            var path = request.PathBase.ToString() + request.Path.ToString();

            // Encode path and each individual query value with lengths and counts:
            // "tag=a,b" must never collide with two values "tag=a&tag=b".
            keyBuilder.Append(path.Length).Append(':').Append(path);
            keyBuilder.Append('|').Append(request.Query.Count).Append(':');

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                keyBuilder.Append(key.Length).Append(':').Append(key);
                keyBuilder.Append('=').Append(value.Count).Append(':');
                for (var index = 0; index < value.Count; index++)
                {
                    var queryValue = value[index];
                    keyBuilder.Append(queryValue?.Length ?? -1).Append(':');
                    if (queryValue is not null)
                    {
                        keyBuilder.Append(queryValue);
                    }
                }
            }

            // New namespace prevents reading legacy keys with ambiguous query values.
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(keyBuilder.ToString()));
            return "response-cache:v3:" + Convert.ToHexString(hash);
        }
    }
}
