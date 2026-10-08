using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebApiCoreSeed.Api.Attributes;
using WebApiCoreSeed.Api.Services.Interfaces;
using WebApiCoreSeed.Api.Settings;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Attributes;

public sealed class CachedAttributeSecurityTests
{
    [Theory]
    [InlineData("POST", false, false, false)]
    [InlineData("GET", true, false, false)]
    [InlineData("GET", false, true, false)]
    [InlineData("GET", false, false, true)]
    public async Task RequisicoesNaoPublicasNuncaDevemConsultarOuGravarCache(
        string method, bool authenticated, bool hasAuthorizationHeader, bool hasCookie)
    {
        var (context, cache, filters, actionContext) = CreateContext(method, authenticated);
        if (hasAuthorizationHeader)
        {
            context.HttpContext.Request.Headers.Authorization = "Bearer example";
        }
        if (hasCookie)
        {
            context.HttpContext.Request.Headers.Cookie = "session=example";
        }

        var nextCalls = 0;
        await new CachedAttribute(20).OnActionExecutionAsync(context, () =>
        {
            nextCalls++;
            return Task.FromResult(Success(actionContext, filters));
        });

        Assert.Equal(1, nextCalls);
        cache.Verify(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        cache.Verify(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(404)]
    [InlineData(500)]
    public async Task RespostasDeErroNaoDevemSerArmazenadas(int statusCode)
    {
        var (context, cache, filters, actionContext) = CreateContext();
        await new CachedAttribute(20).OnActionExecutionAsync(context, () =>
            Task.FromResult(new ActionExecutedContext(actionContext, filters, new object())
            {
                Result = new ObjectResult("error") { StatusCode = statusCode }
            }));

        cache.Verify(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RespostaComSetCookieNaoDeveSerArmazenada()
    {
        var (context, cache, filters, actionContext) = CreateContext();
        context.HttpContext.Response.Headers.SetCookie = "session=value; HttpOnly";

        await new CachedAttribute(20).OnActionExecutionAsync(context, () =>
            Task.FromResult(Success(actionContext, filters)));

        cache.Verify(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAnonimoPodeUsarCacheSemExporQueryStringNaChave()
    {
        var (context, cache, filters, actionContext) = CreateContext();
        context.HttpContext.Request.QueryString = new QueryString("?page=1&access_token=private-secret");
        string? readKey = null;
        string? writeKey = null;

        cache.Setup(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((key, _) => readKey = key)
            .ReturnsAsync((string?)null);
        cache.Setup(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Callback<string, object?, TimeSpan, CancellationToken>((key, _, _, _) => writeKey = key)
            .Returns(Task.CompletedTask);

        await new CachedAttribute(20).OnActionExecutionAsync(context, () =>
            Task.FromResult(Success(actionContext, filters)));

        Assert.NotNull(readKey);
        Assert.Equal(readKey, writeKey);
        Assert.StartsWith("response-cache:v2:", readKey, StringComparison.Ordinal);
        Assert.DoesNotContain("private-secret", readKey, StringComparison.Ordinal);
        Assert.DoesNotContain("access_token", readKey, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TimeToLiveInvalidoDeveSerRejeitado(int seconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CachedAttribute(seconds));
    }

    private static (ActionExecutingContext Context, Mock<IResponseCacheService> Cache,
        List<IFilterMetadata> Filters, ActionContext ActionContext) CreateContext(
        string method = "GET", bool authenticated = false)
    {
        var cache = new Mock<IResponseCacheService>();
        cache.Setup(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        cache.Setup(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(new RedisCacheSettings { Enabled = true });
        services.AddSingleton<IResponseCacheService>(cache.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };
        httpContext.Request.Method = method;
        httpContext.Request.Path = "/api/v1/Pratos";
        if (authenticated)
        {
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user-1") }, "Test"));
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var filters = new List<IFilterMetadata>();
        var context = new ActionExecutingContext(actionContext, filters, new Dictionary<string, object?>(), new object());

        return (context, cache, filters, actionContext);
    }

    private static ActionExecutedContext Success(ActionContext actionContext, List<IFilterMetadata> filters)
        => new(actionContext, filters, new object())
        {
            Result = new OkObjectResult(new { id = 1 })
        };
}
