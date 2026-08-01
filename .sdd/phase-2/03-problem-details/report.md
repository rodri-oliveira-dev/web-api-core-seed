# Report - 03 Problem Details

## Changes

- Added ASP.NET Core Problem Details registration.
- Added cohesive exception handlers:
  - `FluentValidationExceptionHandler`;
  - `PersistenceExceptionHandler`;
  - `UnhandledExceptionHandler`.
- Added `ApiProblemDetails` helper and `ProblemDetailsResult`.
- Replaced route-based exception handling with `UseExceptionHandler()`.
- Replaced status-code pages body from `CustomResult` to Problem Details.
- Migrated controller error paths for validation, not found and domain notifications.
- Migrated JWT challenge/forbidden and claim filter failures to Problem Details.
- Removed obsolete error pipeline artifacts:
  - `ErrorController`;
  - `ErrorHandlingMiddleware`;
  - `CustomErrorResult`;
  - `CustomUnauthorizedResult`;
  - `CustomForbiddenResult`.

## Contract

Errors now use `application/problem+json` and include `traceId`.

Mapped status codes:

- 400 validation: `urn:problem:validation`;
- 400 domain rule: `urn:problem:domain-rule`;
- 401 authentication: `urn:problem:authentication`;
- 403 authorization: `urn:problem:authorization`;
- 404 not found: `urn:problem:not-found`;
- 409 conflict: `urn:problem:conflict`;
- 429 rate limit: `urn:problem:rate-limit` cataloged for next prompt;
- 500 persistence: `urn:problem:persistence-failure`;
- 500 unexpected: `urn:problem:unexpected-error`.

## Compatibility

Breaking changes are documented in `contract-changes.md`.

Responses of success were not changed. `CustomResult` remains for success payloads and `CustomNoContentResult` remains for current 204 paths.

## Validation

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed.
- `dotnet test --configuration Release --no-build`: passed, 27 tests.
- Smoke real passed for Swagger, invalid payload, missing route and unauthenticated protected endpoint.
- `/hc` still depends on local SQL Server and timed out in real smoke; the host-test health endpoint is covered with health checks cleared.

## Risks

- Some business errors still originate from string-based notifications. Conflict detection for existing-resource notifications is intentionally narrow.
- `/hc` remains environment-sensitive until the local dependency strategy is revisited.
- Rate limiting still uses the legacy package and should be migrated in issue `#7`.

## Next Step

Run Prompt 4 for issue `#7 - Native rate limiting`.
