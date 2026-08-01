# Validation - 01 .NET 10 Migration

This file is updated during the task. Baseline commands were executed before implementation.

## Baseline

| Command | Exit code | Result | Limitation |
| --- | ---: | --- | --- |
| `dotnet restore` | 1 | Failed with `NETSDK1138` for `netcoreapp3.1` and invalid local metadata for `microsoft.netcore.targets/1.1.0`. | Legacy restore blocked before migration. |
| `dotnet build --no-restore` | 1 | Failed because project assets were missing for API, Data and test projects. | Restore had failed. |
| `dotnet test --no-build` | 0 | Returned no output. | Inconclusive because no successful restore/build existed. |

## Final

| Command | Exit code | Result | Limitation |
| --- | ---: | --- | --- |
| `dotnet --info` | 0 | SDK `10.0.302`; host `10.0.10`; `global.json` detected at repository root. | No workloads installed, not required for this solution. |
| `dotnet restore` | 0 | Restore completed; all projects up to date. | None. |
| `dotnet build --configuration Release --no-restore` | 0 | Build completed for API, Business, Data and tests targeting `net10.0`. | 33 analyzer/SDK warnings remain. |
| `dotnet test --configuration Release --no-build` | 0 | 21 tests passed, 0 failed, 0 skipped. | Unit tests only; no integration or HTTP tests exist yet. |
| `dotnet list package` | 0 | Listed all top-level package references for `net10.0`. | None. |
| `dotnet list package --outdated` | 0 | Only `Swashbuckle.AspNetCore` is outdated: `6.9.0` -> `10.2.3`. | Kept intentionally for future OpenAPI prompt. |
| `dotnet list package --deprecated` | 0 | Deprecated packages: `Microsoft.AspNetCore.Mvc.Versioning`, `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer`, `xunit`. | API Versioning migration and xUnit v3 migration are deferred. |
| `dotnet list package --vulnerable` | 0 | No vulnerable packages reported. | Depends on current NuGet advisory feed. |
| `git grep -n "netcoreapp3.1"` | 0 | Matches only historical docs and repository guidance about the legacy branch/baseline. | `AGENTS.md` and active project files no longer state active projects target `netcoreapp3.1`. |
| `git grep -n "Microsoft.CodeAnalysis.FxCopAnalyzers"` | 0 | Match only in Phase 1 historical baseline. | Active project reference removed. |
| `git grep -n "IgnoreNullValues\|Microsoft.Extensions.Caching.Redis\|FxCopAnalyzers"` | 0 | Matches only Phase 1 historical baseline for removed package names. | Active code no longer uses `IgnoreNullValues`. |

## Smoke Test

Command shape:

```powershell
dotnet run --no-build --configuration Release --urls http://localhost:5068
```

Run from `src/DevIO.Api` with `ASPNETCORE_ENVIRONMENT=Development`.

Results:

| Check | Result |
| --- | --- |
| Process startup | Passed; API logged `Now listening on: http://localhost:5068`. |
| Swagger document | `GET /swagger/v1/swagger.json` returned `200`. |
| Known endpoint | `GET /error/404` returned `404` with the existing error controller behavior. |
| Health check | `/hc` started and logged SQL Server as `Unhealthy`, but the HTTP call did not return before a 35 second timeout when local SQL Server was unavailable. |
| Shutdown | Process was explicitly stopped by the smoke script. |

Limitations:

- SQL Server was not running locally.
- Redis and Seq were disabled through environment variables for the final smoke attempt.
- HealthChecks UI web `/hc-ui` was disabled because the latest available package line failed startup with EF Core 10.
