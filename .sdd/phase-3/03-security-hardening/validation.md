# Validation - 03 Security Hardening

## Official Commands

| Command | Result |
| --- | --- |
| `dotnet restore` | Passed |
| `dotnet build --configuration Release --no-restore` | Passed |
| `dotnet test --configuration Release --no-build` | Passed: 36 tests in `Pedidos.Test`, 26 tests in `WebApiCoreSeed.IntegrationTests` |
| `dotnet list package --vulnerable` | Passed: no vulnerable packages reported for current sources |

## Smoke And Regression

Covered by integration tests:

- Public endpoint: `GET /api/v1/Pratos`.
- Protected endpoint: `GET /api/v1/Mesas/{id}` with missing or insufficient credentials.
- Problem Details: validation, not found, unauthorized, forbidden and rate limiting.
- Rate limiting: public API quota.
- OpenAPI: `/openapi/v1.json` and `/openapi/v2.json`.
- Health: `/health/live` and `/hc` minimal responses.
- Readiness: `/health/ready` checks SQL Server and Redis in `Testing`.
- CORS preflight: allowed and denied origins.
- Headers: selected security headers present and obsolete headers absent.
- Logging: captured Serilog output does not contain Authorization, access token or sensitive query values.

## Grep Validation

- `git grep -n "AllowAnyOrigin"`: occurrences only in SDD status/handoff documenting removal.
- `git grep -n "X-XSS-Protection"`: occurrences in SDD documentation and one commented legacy entry in `src/DevIO.Api/web.config`.
- `git grep -n "Feature-Policy"`: occurrences only in SDD documentation.
- `git grep -n "QueryString"`: no occurrence.
- Active `src`/`test` grep: no `AllowAnyOrigin`, no `Feature-Policy`, no `QueryString`; only the commented `web.config` `X-XSS-Protection` entry remains.

## Sensitive Terms Review

- `password` remains in Identity models, validation, tests and OpenAPI schemas as field names, not logged secrets.
- `src/DevIO.Api/appsettings.json` used non-production local placeholders after that phase.
- `test/WebApiCoreSeed.IntegrationTests/Infrastructure/ApiFactory.cs` contains a Testcontainers SQL password used only for ephemeral integration tests.
- `docker/SqlServer.dockerfile_` still contains a legacy fixed SQL password outside the active .NET 10 runtime path; this was not changed because production infrastructure and secret rotation are outside this prompt.
