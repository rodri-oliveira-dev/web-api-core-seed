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
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
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

            // Never cache error results, failed actions or responses setting session cookies.
            if (!executedContext.Canceled
                && executedContext.Exception == null
                && !context.HttpContext.Response.Headers.ContainsKey("Set-Cookie")
                && executedContext.Result is ObjectResult okObjectResult
                && (okObjectResult.StatusCode == null || okObjectResult.StatusCode == StatusCodes.Status200OK))
            {
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds), cancellationToken);
            }
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.PathBase).Append(request.Path);

            // Length-prefix values to avoid ambiguous keys. Hashing also prevents
            // request query parameters from being exposed in Redis key listings.
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                var queryValue = value.ToString();
                keyBuilder.Append('|').Append(key.Length).Append(':').Append(key);
                keyBuilder.Append('=').Append(queryValue.Length).Append(':').Append(queryValue);
            }

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(keyBuilder.ToString()));
            return "response-cache:v2:" + Convert.ToHexString(hash);
        }
    }
}
