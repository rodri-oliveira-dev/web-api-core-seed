# Report - 01 .NET 10 Migration

## Changes

- Added `global.json` pinning SDK `10.0.302`.
- Added `Directory.Build.props` with `Nullable=disable`, `ImplicitUsings=enable` and `AnalysisLevel=latest-recommended`.
- Migrated API, Business, Data and test projects to `net10.0`.
- Updated Microsoft ASP.NET Core, EF Core and `Microsoft.Extensions.*` packages that remain required.
- Updated FluentValidation, AutoMapper, Serilog, Redis cache, health checks, rate limiting, Swagger bridge and test packages.
- Removed obsolete or unnecessary packages including FxCop analyzers, old Redis cache package, old Serilog colored console sink, Swagger subpackages, code generation tooling and AutoMapper DI extension.
- Replaced removed `JsonSerializerOptions.IgnoreNullValues` with `DefaultIgnoreCondition`.
- Removed legacy `SetCompatibilityVersion`.
- Updated FluentValidation cascade syntax and child validator registration.
- Adjusted Serilog and AutoMapper registration for modern package APIs.
- Added `IProcessingStrategy` registration required by `AspNetCoreRateLimit` 5.
- Disabled HealthChecks UI web `/hc-ui` temporarily; `/hc` remains registered.

## Problems Found

- Baseline restore was blocked by the known legacy NuGet cache issue before migration.
- Swashbuckle 10 requires OpenAPI source changes outside this prompt, so Swashbuckle was kept at `6.9.0`.
- `AspNetCore.HealthChecks.UI` latest stable is `9.0.0` and failed startup with EF Core 10.
- `/hc` does not complete in the local smoke environment without SQL Server; the SQL health check logs `Unhealthy`.

## Remaining Risks

- Deprecated API Versioning packages remain until the OpenAPI/versioning prompt.
- `AspNetCoreRateLimit` remains until native rate limiting is implemented.
- xUnit v2 is reported as legacy; xUnit v3 migration is deferred.
- Analyzer warnings remain and are documented in validation.
- Full database, Redis and Seq runtime behavior still needs environment-backed validation.

## Validation Summary

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed with warnings.
- `dotnet test --configuration Release --no-build`: passed, 21 tests.
- `dotnet list package --vulnerable`: no vulnerable packages.
- Smoke: API starts, Swagger returns `200`, `/error/404` returns `404`; `/hc` blocked by unavailable SQL Server/pipeline timeout.

## Next Steps

- Prompt 2 / issue `#5`: modernize hosting model.
- Do not repeat package migration already completed here.
- Revisit HealthChecks UI with a .NET 10-compatible strategy.
