# Discovery - 02 Modern Hosting

## Baseline Commands

| Command | Result |
| --- | --- |
| `git status --short` | Clean working tree. |
| `git branch --show-current` | `phase/2-dotnet-10-migration`. |
| `git log -3 --oneline` | `b8593c5 build: migrate solution to .NET 10`; `1bcce0d chore: bootstrap modernization tooling`; `2799562 chore: finalize legacy preservation`. |
| `git rev-parse HEAD` | `b8593c59823ad97b230099edb9f38090dcafac5c`. |
| `dotnet build --configuration Release` | Passed with existing analyzer warnings and legacy-hosting warnings. |
| `dotnet test --configuration Release --no-build` | Passed, 21 tests. |

## Confirmed Preconditions

- Branch is correct.
- Working tree was clean before implementation.
- Prompt 1 is committed at `b8593c5`.
- Active projects target `net10.0`.
- Build and unit tests were working before this prompt.
- HealthChecks UI web remains intentionally disabled from Prompt 1; `/hc` remains active.

## Service Registrations Found

- `MeuDbContext` using SQL Server and no query tracking.
- MVC/controllers were duplicated through `AddMvc` and `AddControllers`.
- JSON null values ignored with `JsonIgnoreCondition.WhenWritingNull`.
- Identity EF context and ASP.NET Identity.
- JWT bearer authentication from `AppSettings`.
- AutoMapper profiles from the API assembly.
- Swagger generation and versioned API explorer.
- Repository, service, notifier, user context and Swagger options dependencies.
- API versioning and `ApiBehaviorOptions.SuppressModelStateInvalidFilter`.
- Development and Production CORS policies.
- Current `AspNetCoreRateLimit` memory stores and processing strategy.
- Response compression with Brotli, Gzip and `application/json`.
- Application cookie and cookie policy.
- Health checks for SQL Server, optional Seq URL and optional Redis.
- HSTS options.
- Redis response cache service when Redis cache is enabled.

## Middleware Found

- Environment-specific CORS policy.
- Environment-specific exception handler route.
- Serilog request logging.
- Custom Serilog middleware.
- Custom error handling middleware.
- HSTS.
- Status code pages with existing `CustomResult` JSON payload.
- Current IP rate limiting middleware.
- Security headers middleware.
- Authentication.
- HTTPS redirection.
- Static files.
- Routing.
- Cookie policy.
- Authorization.
- Endpoint mapping.
- Response compression.
- Swagger and Swagger UI.
- Health check endpoint `/hc`.

## Static Configuration

- `Program.cs` built a static `IConfiguration`.
- `Resources/ConnectionString.cs` read connection strings through that static configuration.
- `MeuDbContext`, `ApplicationDbContext` and SQL health checks depended on the helper.

## Duplications And Host Shape

- Two host-builder entry points existed in `Program.cs`; one was obsolete and one drove runtime startup.
- MVC/controller registration was duplicated in `Startup.cs` and `ApiConfig.cs`.
- The legacy startup class combined DI, logging-adjacent config and middleware pipeline.

## Tests And HTTP Collections

- No `WebApplicationFactory`, `TestServer`, `.http` or `.rest` collection exists in the active repository.
- Existing tests are unit tests only.
