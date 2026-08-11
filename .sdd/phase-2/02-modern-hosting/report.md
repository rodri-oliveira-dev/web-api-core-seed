# Report - 02 Modern Hosting

## Changes

- Replaced the legacy host setup with `WebApplication.CreateBuilder`.
- Consolidated runtime startup in `Program.cs`.
- Added `HostingConfig` with Serilog host setup, API service registration and API pipeline registration.
- Removed static configuration access.
- Removed duplicate MVC registration and kept controller registration.
- Made controller endpoint mapping explicit.
- Removed the legacy startup class file and obsolete connection-string helper.

## Behavior Preserved

- JSON ignores null values in responses.
- Development and Production CORS policies remain.
- Development and Production exception handler routes remain.
- JWT authentication and authorization settings remain.
- Current `AspNetCoreRateLimit` configuration remains.
- Swagger and Swagger UI remain on the existing compatibility package line.
- `/hc` remains registered with `UIResponseWriter`.
- Static files, HSTS, security headers, cookies, status-code JSON payloads and custom middlewares remain.

## Intentional Debts Kept

- Problem Details is deferred.
- Native rate limiting is deferred to issue `#6`.
- OpenAPI and API Versioning modernization is deferred.
- `/hc-ui` remains disabled until a compatible strategy exists.
- Existing analyzer warnings are not addressed in this prompt.

## Validation Summary

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed.
- `dotnet test --configuration Release --no-build`: passed, 21 tests.
- Host-cleanup grep commands: passed with no matches.
- Smoke: startup, Swagger, `/error/404`, unauthenticated protected route, Development CORS and port cleanup passed.
- `/hc`: endpoint remains registered but timed out locally because SQL Server is unavailable.

## Next Step

Run Prompt 3 for issue `#6`, native rate limiting.
