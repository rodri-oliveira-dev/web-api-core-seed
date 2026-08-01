# Status - Phase 2

## Current State

| Field | Value |
| --- | --- |
| Phase | Phase 2 - Modernization to .NET 10 |
| Current task | `05 - OpenAPI and API versioning` |
| Current branch | `phase/2-dotnet-10-migration` |
| Branch base | `phase/1-preserve-legacy` |
| Base SHA | `2799562943ac03926d69bc716617d091d04ecc82` |
| Initial branch SHA | `2799562943ac03926d69bc716617d091d04ecc82` |
| Source repository SHA | `9029163f1a795a1bb18f138dd8fa9179f13f544e` |
| Related next issue | `#9 - Phase 3 quality and safety` |
| Phase status | Fase 2: concluida localmente |
| Phase PR | pendente |
| Push | pendente |

## Base Selection

Phase 1 has not been integrated into `main` in this local repository. `git merge-base --is-ancestor phase/1-preserve-legacy main` returned false, so the Phase 2 branch was created from the final commit of `phase/1-preserve-legacy`.

## Prompt Status

| Prompt | Status |
| --- | --- |
| 00 - Bootstrap | concluido |
| 01 - Migracao .NET 10 | concluido |
| 02 - Hosting moderno | concluido |
| 03 - Problem Details | concluido |
| 04 - Rate limiting nativo | concluido |
| 05 - OpenAPI e versionamento | concluido |

## Validations

Final validation results for prompt 01 are recorded in `01-dotnet-10-migration/validation.md`.
Final validation results for prompt 02 are recorded in `02-modern-hosting/validation.md`.
Final validation results for prompt 03 are recorded in `03-problem-details/validation.md`.
Final validation results for prompt 04 are recorded in `04-native-rate-limiting/validation.md`.
Final validation results for prompt 05 are recorded in `05-openapi-versioning/validation.md`.

Prompt 01 summary:

- `dotnet restore` passed with SDK `10.0.302`.
- `dotnet build --configuration Release --no-restore` passed.
- `dotnet test --configuration Release --no-build` passed: 21 tests.
- `dotnet list package --vulnerable` reported no vulnerable packages.
- Smoke test started the API, confirmed Swagger `200` and `/error/404` returning `404`.
- `/hc` executed the SQL health check but did not return before timeout because local SQL Server was unavailable; Redis and Seq were disabled for the smoke attempt.

Prompt 02 summary:

- `dotnet restore` passed.
- `dotnet build --configuration Release --no-restore` passed.
- `dotnet test --configuration Release --no-build` passed: 21 tests.
- Host-cleanup searches found no active legacy startup class, duplicate host builder or static startup configuration access.
- Smoke test started the API, confirmed Swagger, existing error endpoint, authentication challenge, Development CORS preflight and port cleanup.
- `/hc` remains registered but local SQL Server unavailability still prevents a healthy local result.

Prompt 03 summary:

- `dotnet restore` passed.
- `dotnet build --configuration Release --no-restore` passed.
- `dotnet test --configuration Release --no-build` passed: 27 tests.
- API errors now use Problem Details with `traceId`.
- Legacy `ErrorController`, `ErrorHandlingMiddleware` and redundant error wrappers were removed.
- Smoke confirmed Swagger `200`, invalid payload `400 application/problem+json`, missing route `404 application/problem+json`, and protected endpoint without token `401 application/problem+json`.
- `/hc` remains registered but real smoke timed out locally because SQL Server is unavailable; host-test health check is covered with health registrations cleared.

Prompt 04 summary:

- `AspNetCoreRateLimit` was removed from the active API project.
- Native ASP.NET Core rate limiting is registered with explicit `public`, `authenticated` and `authentication-sensitive` policies.
- `UseRateLimiter` runs after routing and authentication, before authorization.
- Rate-limit rejections return `429 application/problem+json` with `urn:problem:rate-limit`, `traceId` and `Retry-After`.
- Authenticated partitions use validated user identity; anonymous fallback uses a hashed composite of optional `X-ClientId` and direct connection remote address.
- Forwarded IP headers are not trusted because no trusted proxy configuration exists.
- Integration tests cover allowed, rejected, exempt and independent partition behavior.
- `dotnet restore`, `dotnet build --configuration Release --no-restore`, `dotnet test --configuration Release --no-build`, `dotnet list package` and active-code legacy searches passed.
- HTTP smoke/regression through `WebApplicationFactory` passed with 11 focused tests; process-based local smoke was blocked by local shell policy before API startup.

Prompt 05 summary:

- `Microsoft.AspNetCore.Mvc.Versioning*` and `Swashbuckle.AspNetCore` were removed from the active API project.
- API versioning now uses `Asp.Versioning.Mvc`, `Asp.Versioning.Mvc.ApiExplorer` and `Asp.Versioning.OpenApi`.
- OpenAPI generation now uses `Microsoft.AspNetCore.OpenApi` with versioned documents.
- Scalar UI is registered at `/scalar/`.
- Generated documents are available at `/openapi/v1.json` and `/openapi/v2.json`.
- Committed generated contracts are stored in `docs/openapi/openapi-v1.json` and `docs/openapi/openapi-v2.json`; previous Swagger contracts are stored under `docs/openapi/baseline/`.
- JWT is documented as HTTP bearer JWT.
- Problem Details, `401`, `403` and `429` responses are documented where applicable.
- `dotnet --info`, `dotnet restore`, `dotnet build --configuration Release --no-restore`, `dotnet test --configuration Release --no-build`, `dotnet list package --deprecated`, `dotnet list package --vulnerable`, OpenAPI generation, JSON parse validation and `git diff --check` were executed.
- Consolidated tests passed: 34 tests.
- `dotnet list package --deprecated` reports only the existing `xunit` 2.9.3 deprecation in the test project.
- `dotnet list package --vulnerable` reports no vulnerable packages.

## Blockers

No build or test blocker remains for the migrated solution.

Runtime limitations:

- Full `/hc` validation still depends on external SQL Server availability.
- HealthChecks UI web `/hc-ui` is temporarily disabled because the latest available `AspNetCore.HealthChecks.UI` package is 9.0.0 and is not runtime-compatible with EF Core 10 in this solution.

## Next Step

Phase 2 is complete locally. Prepare Phase 3 on a new branch:

```text
phase/3-quality-and-safety
```

Planned next issues:

- `#9`
- `#10`
- `#11`
- `#12`
- `#13`
