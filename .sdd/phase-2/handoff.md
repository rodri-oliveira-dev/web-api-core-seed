# Handoff - Phase 2 Task 02

## Branch

- Current branch: `phase/2-dotnet-10-migration`
- Prompt 01 commit: `b8593c5 build: migrate solution to .NET 10`
- Prompt 02 commit: `refactor: adopt modern ASP.NET Core hosting`.

## Current Runtime

- SDK pinned by `global.json`: `10.0.302`
- Active target framework: `net10.0` in API, Business, Data and test projects.
- API hosting model: modern `WebApplication`.

## New Composition Root

- `src/DevIO.Api/Program.cs` creates one `WebApplicationBuilder`.
- `builder.WebHost.UseIIS()` preserves IIS hosting configuration.
- `builder.Host.UseApiSerilog()` configures Serilog from `IConfiguration`.
- `builder.Services.AddApiServices(builder.Configuration)` registers API services.
- `app.UseApiPipeline()` registers the HTTP pipeline.
- `await app.RunAsync()` starts the application.

## Extensions Created

- `HostingConfig.UseApiSerilog`
- `HostingConfig.AddApiServices`
- `HostingConfig.UseApiPipeline`

Existing focused extensions remain for API versioning/CORS/endpoints, Identity/JWT, rate limiting, Swagger, cache, cookies and dependency injection.

## Middleware Order

1. Environment CORS policy.
2. Environment exception handler route.
3. Serilog request logging.
4. Custom Serilog middleware.
5. Custom error handling middleware.
6. HSTS.
7. Status code pages.
8. Current IP rate limiting.
9. Security headers.
10. Response compression.
11. HTTPS redirection.
12. Static files.
13. Routing.
14. Cookie policy.
15. Authentication.
16. Authorization.
17. Controller endpoint mapping and conventional default route.
18. Swagger and Swagger UI.
19. Health check endpoint `/hc`.

## Behaviors Preserved

- JSON null-value behavior.
- Development and Production CORS policies.
- Development and Production exception handler routes.
- JWT authentication/authorization settings.
- Static files, cookies, HSTS, security headers and status-code JSON payloads.
- Current rate limiting package and configuration.
- Swagger compatibility setup.
- `/hc` with `UIResponseWriter`.

## Intentional Debts

- Problem Details remains pending.
- Native rate limiting remains pending for issue `#6`.
- OpenAPI and API Versioning modernization remains pending.
- HealthChecks UI web `/hc-ui` remains disabled.
- Existing analyzer warnings remain outside this prompt.

## Validation State

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed.
- `dotnet test --configuration Release --no-build`: passed, 21 tests.
- Host-cleanup grep commands: passed.
- Smoke: API starts; Swagger returns `200`; `/error/404` returns `404`; unauthenticated protected registration route returns authentication challenge; Development CORS preflight on `/api/v1/Pratos` returns configured CORS headers; smoke job stops and port cleanup is confirmed.
- `/hc`: endpoint remains registered; full healthy result still depends on local SQL Server availability.

## Next Objective

```text
#6 - Native rate limiting
```
