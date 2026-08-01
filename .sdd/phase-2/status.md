# Status - Phase 2

## Current State

| Field | Value |
| --- | --- |
| Phase | Phase 2 - Modernization to .NET 10 |
| Current task | `02 - Hosting moderno` |
| Current branch | `phase/2-dotnet-10-migration` |
| Branch base | `phase/1-preserve-legacy` |
| Base SHA | `2799562943ac03926d69bc716617d091d04ecc82` |
| Initial branch SHA | `2799562943ac03926d69bc716617d091d04ecc82` |
| Source repository SHA | `9029163f1a795a1bb18f138dd8fa9179f13f544e` |
| Related next issue | `#6 - Native rate limiting` |

## Base Selection

Phase 1 has not been integrated into `main` in this local repository. `git merge-base --is-ancestor phase/1-preserve-legacy main` returned false, so the Phase 2 branch was created from the final commit of `phase/1-preserve-legacy`.

## Prompt Status

| Prompt | Status |
| --- | --- |
| 00 - Bootstrap | concluido |
| 01 - Migracao .NET 10 | concluido |
| 02 - Hosting moderno | concluido |
| 03 - Problem Details | pendente |
| 04 - Rate limiting nativo | pendente |
| 05 - OpenAPI e versionamento | pendente |

## Validations

Final validation results for prompt 01 are recorded in `01-dotnet-10-migration/validation.md`.
Final validation results for prompt 02 are recorded in `02-modern-hosting/validation.md`.

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

## Blockers

No build or test blocker remains for the migrated solution.

Runtime limitations:

- Full `/hc` validation still depends on external SQL Server availability and the legacy health response pipeline.
- HealthChecks UI web `/hc-ui` is temporarily disabled because the latest available `AspNetCore.HealthChecks.UI` package is 9.0.0 and is not runtime-compatible with EF Core 10 in this solution.

## Next Step

Run Prompt 3 for:

```text
#6 - Native rate limiting
```
