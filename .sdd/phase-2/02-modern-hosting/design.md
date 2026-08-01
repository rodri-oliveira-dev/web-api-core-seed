# Design - 02 Modern Hosting

## Composition Root

`Program.cs` is the only composition root:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseIIS();
builder.Host.UseApiSerilog();
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
app.UseApiPipeline();
await app.RunAsync();
```

The bootstrap logger remains only for startup/fatal messages before Serilog reads application configuration.

## Extensions

- `UseApiSerilog`: configures the Serilog host integration from `IConfiguration`.
- `AddApiServices`: registers persistence, MVC controllers, Identity, Swagger, dependencies, rate limiting, compression, cookies, health checks, HSTS and Redis cache.
- `UseApiPipeline`: registers the HTTP middleware sequence explicitly.

Existing extension files remain where they already express one concern:

- `ApiConfig` keeps API versioning, CORS, security headers and endpoint routing details.
- `IdentityConfig` keeps Identity/JWT setup.
- `RateLimitConfig`, `SwaggerConfig`, `CacheConfig`, `CookieConfiguration` and `DependencyInjectionConfig` keep their current responsibilities.

## Configuration

- Services receive `IConfiguration` explicitly.
- SQL Server connection strings come from `configuration.GetConnectionString("DefaultConnection")`.
- No helper exists only to access static configuration.

## Intentional Minimal Changes

- Keep current package choices, Swagger, API Versioning and rate limiting packages.
- Keep `/hc` and keep `/hc-ui` disabled.
- Keep current error payloads and status-code handling.
- Keep controllers and route templates unchanged.
- Keep HSTS registration in both environments because that was the observed behavior.

## Pipeline Adjustments

- `UseAuthentication` moves next to routing and authorization.
- `MapControllers` is explicit, while the conventional default route remains.
- Response compression runs before endpoint execution so the configured middleware can affect controller responses.

These changes are limited to valid endpoint-routing behavior in modern ASP.NET Core.
