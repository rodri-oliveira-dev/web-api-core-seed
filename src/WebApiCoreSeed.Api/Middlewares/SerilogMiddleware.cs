using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace WebApiCoreSeed.Api.Middlewares
{
    public class SerilogMiddleware
    {
        const string MessageTemplate = "HTTP {RequestMethod} {RequestEndpoint} responded {StatusCode} in {Elapsed:0.0000} ms";

        static readonly Serilog.ILogger Log = Serilog.Log.ForContext<SerilogMiddleware>();

        readonly RequestDelegate _next;

        public SerilogMiddleware(RequestDelegate next)
        {
            ArgumentNullException.ThrowIfNull(next);

            _next = next;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            var start = Stopwatch.GetTimestamp();
            try
            {
                await _next(httpContext);
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;
                log.Write(level, MessageTemplate, GetKnownHttpMethod(httpContext), GetEndpointName(httpContext), statusCode, elapsedMs);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                LogException(httpContext, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex);
                throw;
            }
        }

        static void LogException(HttpContext httpContext, double elapsedMs, Exception ex)
        {
            LogForErrorContext(httpContext)
                .Error(ex, MessageTemplate, GetKnownHttpMethod(httpContext), GetEndpointName(httpContext), 500, elapsedMs);
        }

        static Serilog.ILogger LogForErrorContext(HttpContext httpContext)
        {
            return Log.ForContext("RequestEndpoint", GetEndpointName(httpContext));
        }

        static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }

        static string GetKnownHttpMethod(HttpContext httpContext)
        {
            var method = httpContext.Request.Method;

            if (HttpMethods.IsGet(method)) return "GET";
            if (HttpMethods.IsPost(method)) return "POST";
            if (HttpMethods.IsPut(method)) return "PUT";
            if (HttpMethods.IsDelete(method)) return "DELETE";
            if (HttpMethods.IsPatch(method)) return "PATCH";
            if (HttpMethods.IsOptions(method)) return "OPTIONS";
            if (HttpMethods.IsHead(method)) return "HEAD";

            return "OTHER";
        }

        static string GetEndpointName(HttpContext httpContext)
        {
            return httpContext.GetEndpoint()?.DisplayName ?? "Unmatched endpoint";
        }
    }
}
