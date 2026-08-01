# Handoff - Phase 2 Task 03

## Branch

- Current branch: `phase/2-dotnet-10-migration`
- Prompt 01 commit: `b8593c5 build: migrate solution to .NET 10`
- Prompt 02 commit: `24f701d refactor: adopt modern ASP.NET Core hosting`
- Prompt 03 commit: pending until delivery.

## Current Runtime

- SDK pinned by `global.json`: `10.0.302`
- Active target framework: `net10.0` in API, Business, Data and test projects.
- API hosting model: modern `WebApplication`.
- Error contract: ASP.NET Core Problem Details.

## Problem Details Implementation

- `HostingConfig` registers `AddProblemDetails`.
- `HostingConfig` registers:
  - `FluentValidationExceptionHandler`;
  - `PersistenceExceptionHandler`;
  - `UnhandledExceptionHandler`.
- `UseApiPipeline` now uses `UseExceptionHandler()` without route-based error controllers.
- `UseStatusCodePages` writes Problem Details.
- `ApiProblemDetails` centralizes `type`, `title`, safe `detail`, `instance`, `traceId` and error extensions.
- `ProblemDetailsResult` forces `application/problem+json` for manual controller/filter results.

## Removed

- `src/DevIO.Api/Controllers/ErrorController.cs`
- `src/DevIO.Api/Middlewares/ErrorHandlingMiddleware.cs`
- `src/DevIO.Api/Results/CustomErrorResult.cs`
- `src/DevIO.Api/Results/CustomUnauthorizedResult.cs`
- `src/DevIO.Api/Results/CustomForbiddenResult.cs`

## Preserved

- Routes were not intentionally changed except obsolete `/error*` endpoints.
- Responses of success still use current controller behavior and `CustomResult`.
- Authentication remains JWT Bearer.
- Rate limiting remains on `AspNetCoreRateLimit` until issue `#7`.
- `/hc` remains mapped.

## Validation State

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed.
- `dotnet test --configuration Release --no-build`: passed, 27 tests.
- Smoke real:
  - Swagger returned 200.
  - Invalid login payload returned 400 `application/problem+json`.
  - Missing route returned 404 `application/problem+json`.
  - Protected endpoint without token returned 401 `application/problem+json`.
  - `/hc` timed out locally because SQL Server remains unavailable.

## Known Risks

- Domain notification messages are still string-based.
- Conflict detection is narrow and based on known duplicate-resource notification wording.
- Local full health validation still needs SQL Server availability.

## Next Objective

```text
#7 - Native rate limiting
```
