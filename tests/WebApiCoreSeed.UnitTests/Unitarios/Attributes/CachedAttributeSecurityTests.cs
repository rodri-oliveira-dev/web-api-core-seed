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

        var filter = new CachedAttribute(20);
        var nextCalls = 0;
        var result = new OkObjectResult(new { id = 1 });
        await filter.OnActionExecutionAsync(context, () =>
        {
            nextCalls++;
            return Task.FromResult(Executed(actionContext, filters, result));
        });
        await ExecuteResultAsync(filter, actionContext, filters, result);

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
        var filter = new CachedAttribute(20);
        var result = new ObjectResult("error") { StatusCode = statusCode };

        await filter.OnActionExecutionAsync(context, () =>
            Task.FromResult(Executed(actionContext, filters, result)));
        await ExecuteResultAsync(filter, actionContext, filters, result,
            () => context.HttpContext.Response.StatusCode = statusCode);

        cache.Verify(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ObjectResultSemStatusMasComProblemDetails500NaoDeveSerArmazenado()
    {
        var (context, cache, filters, actionContext) = CreateContext();
        var filter = new CachedAttribute(20);
        var result = new ObjectResult(new ProblemDetails { Status = StatusCodes.Status500InternalServerError });
        Assert.Null(result.StatusCode);

        await filter.OnActionExecutionAsync(context, () =>
            Task.FromResult(Executed(actionContext, filters, result)));
        // The ObjectResult executor sets the effective HTTP status during result execution.
        await ExecuteResultAsync(filter, actionContext, filters, result,
            () => context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError);

        cache.Verify(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetCookieAdicionadoDuranteExecucaoDoResultadoImpedeGravacao()
    {
        var (context, cache, filters, actionContext) = CreateContext();
        var filter = new CachedAttribute(20);
        var result = new OkObjectResult(new { id = 1 });

        await filter.OnActionExecutionAsync(context, () =>
            Task.FromResult(Executed(actionContext, filters, result)));
        // Simulate a result filter setting the cookie after action filters have completed.
        await ExecuteResultAsync(filter, actionContext, filters, result,
            () => context.HttpContext.Response.Headers.SetCookie = "session=value; HttpOnly");

        cache.Verify(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
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

        var filter = new CachedAttribute(20);
        var result = new OkObjectResult(new { id = 1 });
        await filter.OnActionExecutionAsync(context, () =>
            Task.FromResult(Executed(actionContext, filters, result)));
        await ExecuteResultAsync(filter, actionContext, filters, result);

        Assert.NotNull(readKey);
        Assert.Equal(readKey, writeKey);
        Assert.StartsWith("response-cache:v3:", readKey, StringComparison.Ordinal);
        Assert.DoesNotContain("private-secret", readKey, StringComparison.Ordinal);
        Assert.DoesNotContain("access_token", readKey, StringComparison.Ordinal);
        cache.Verify(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), TimeSpan.FromSeconds(20), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValoresDeQueryDistintosNuncaDevemColidirPorVirgulas()
    {
        var singleValue = await GetReadCacheKeyAsync("?tag=a%2Cb");
        var multipleValues = await GetReadCacheKeyAsync("?tag=a&tag=b");

        Assert.NotEqual(singleValue, multipleValues);
    }

    [Fact]
    public async Task OrdemDeParametrosDeQueryNaoMudaChave()
    {
        var firstOrder = await GetReadCacheKeyAsync("?page=1&tag=a&tag=b");
        var secondOrder = await GetReadCacheKeyAsync("?tag=a&tag=b&page=1");

        Assert.Equal(firstOrder, secondOrder);
    }

    [Fact]
    public async Task CacheHitNaoGeraNovaGravacao()
    {
        var (context, cache, filters, actionContext) = CreateContext();
        cache.Setup(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"id\":1}");
        var filter = new CachedAttribute(20);
        var nextCalls = 0;

        await filter.OnActionExecutionAsync(context, () =>
        {
            nextCalls++;
            return Task.FromResult(Executed(actionContext, filters, new OkObjectResult(new { id = 1 })));
        });
        var result = Assert.IsType<ContentResult>(context.Result);
        await ExecuteResultAsync(filter, actionContext, filters, result);

        Assert.Equal(0, nextCalls);
        cache.Verify(c => c.CacheResponseAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TimeToLiveInvalidoDeveSerRejeitado(int seconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CachedAttribute(seconds));
    }

    private static async Task<string> GetReadCacheKeyAsync(string query)
    {
        var (context, cache, filters, actionContext) = CreateContext();
        context.HttpContext.Request.QueryString = new QueryString(query);
        string? readKey = null;
        cache.Setup(c => c.GetCachedResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((key, _) => readKey = key)
            .ReturnsAsync((string?)null);

        await new CachedAttribute(20).OnActionExecutionAsync(context, () =>
            Task.FromResult(Executed(actionContext, filters, new OkObjectResult(new { id = 1 }))));

        return Assert.IsType<string>(readKey);
    }

    private static async Task ExecuteResultAsync(
        CachedAttribute filter,
        ActionContext actionContext,
        List<IFilterMetadata> filters,
        IActionResult result,
        Action? duringResult = null)
    {
        var context = new ResultExecutingContext(actionContext, filters, result, new object());
        await filter.OnResultExecutionAsync(context, () =>
        {
            duringResult?.Invoke();
            return Task.FromResult(new ResultExecutedContext(actionContext, filters, result, new object()));
        });
    }

    private static ActionExecutedContext Executed(
        ActionContext actionContext, List<IFilterMetadata> filters, IActionResult result)
        => new(actionContext, filters, new object()) { Result = result };

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
}
