using System;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApiCoreSeed.Api.Errors;
using WebApiCoreSeed.Api.Settings;

namespace WebApiCoreSeed.Api.Configuration
{
    public static class NativeRateLimitPolicies
    {
        public const string Public = "public";
        public const string Authenticated = "authenticated";
        public const string AuthenticationSensitive = "authentication-sensitive";
    }

    public static class RateLimitConfig
    {
        private const string ClientIdHeader = "X-ClientId";
        private static readonly Action<ILogger, string, string, Exception?> RateLimitRejected =
            LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(1, nameof(RateLimitRejected)),
                "Rate limit rejected request for endpoint {EndpointName} on path {Path}");

        public static IServiceCollection AddNativeRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NativeRateLimitingSettings>(configuration.GetSection(nameof(NativeRateLimitingSettings)));
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    var httpContext = context.HttpContext;
                    var logger = httpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("WebApiCoreSeed.Api.RateLimiting");

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        httpContext.Response.Headers.RetryAfter =
                            Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                    }

                    httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    RateLimitRejected(
                        logger,
                        httpContext.GetEndpoint()?.DisplayName ?? "unknown",
                        httpContext.Request.Path.ToString(),
                        null);

                    var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = ApiProblemDetails.Create(
                            httpContext,
                            StatusCodes.Status429TooManyRequests,
                            ApiProblemDetails.RateLimitType,
                            "Limite de requisicoes excedido.",
                            "A cota de requisicoes foi excedida. Aguarde antes de tentar novamente.")
                    });
                };

                options.AddPolicy(
                    NativeRateLimitPolicies.Public,
                    httpContext =>
                    {
                        var settings = httpContext.RequestServices.GetRequiredService<IOptions<NativeRateLimitingSettings>>().Value;

                        return CreateFixedWindowPartition(
                            httpContext,
                            NativeRateLimitPolicies.Public,
                            settings.Public);
                    });

                options.AddPolicy(
                    NativeRateLimitPolicies.Authenticated,
                    httpContext =>
                    {
                        var settings = httpContext.RequestServices.GetRequiredService<IOptions<NativeRateLimitingSettings>>().Value;

                        return CreateFixedWindowPartition(
                            httpContext,
                            NativeRateLimitPolicies.Authenticated,
                            settings.Authenticated);
                    });

                options.AddPolicy(
                    NativeRateLimitPolicies.AuthenticationSensitive,
                    httpContext =>
                    {
                        var settings = httpContext.RequestServices.GetRequiredService<IOptions<NativeRateLimitingSettings>>().Value;

                        return CreateFixedWindowPartition(
                            httpContext,
                            NativeRateLimitPolicies.AuthenticationSensitive,
                            settings.AuthenticationSensitive);
                    });
            });

            return services;
        }

        private static RateLimitPartition<string> CreateFixedWindowPartition(
            HttpContext httpContext,
            string policyName,
            NativeRateLimitPolicySettings policy)
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                BuildPartitionKey(httpContext, policyName),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = Math.Max(1, policy.PermitLimit),
                    QueueLimit = Math.Max(0, policy.QueueLimit),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    Window = TimeSpan.FromSeconds(Math.Max(1, policy.WindowSeconds)),
                    AutoReplenishment = true
                });
        }

        private static string BuildPartitionKey(HttpContext httpContext, string policyName)
        {
            var userId = GetAuthenticatedUserId(httpContext);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return $"{policyName}:user:{Hash(userId)}";
            }

            var clientId = httpContext.Request.Headers[ClientIdHeader].ToString();
            var remoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var anonymousKey = string.IsNullOrWhiteSpace(clientId)
                ? $"anonymous|remote:{remoteAddress}"
                : $"anonymous|client:{clientId}|remote:{remoteAddress}";

            return $"{policyName}:anonymous:{Hash(anonymousKey)}";
        }

        private static string? GetAuthenticatedUserId(HttpContext httpContext)
        {
            if (httpContext.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            return httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext.User.FindFirstValue("sub")
                ?? httpContext.User.FindFirstValue(ClaimTypes.Email);
        }

        private static string Hash(string value)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));

            return Convert.ToHexString(bytes);
        }
    }
}
