# Design - 01 .NET 10 Migration

## Migration Strategy

Use a narrow compatibility migration:

1. Pin the SDK to the installed .NET 10 SDK `10.0.302`.
2. Move all projects to `net10.0`.
3. Add common compiler settings without turning legacy nullable warnings into migration noise.
4. Update Microsoft packages to `10.0.10` where package references are still required.
5. Remove explicit packages now covered by the shared framework or superseded by better current packages.
6. Keep legacy hosting, `Startup.cs`, controllers, migrations, routes, rate limiting and API versioning shape intact.
7. Make only source edits required for removed APIs or package compatibility.
8. Validate restore/build/test repeatedly after package groups.

## Order Of Changes

1. Create SDD files and checklist.
2. Add `global.json`.
3. Add `Directory.Build.props` for shared build properties.
4. Update project target frameworks and package references.
5. Replace removed JSON null-ignore API.
6. Remove `SetCompatibilityVersion` if modern MVC no longer supports it.
7. Adjust logging sink call only if the legacy colored console sink is removed.
8. Run restore and build.
9. Apply minimal compile fixes only when the compiler proves they are needed.
10. Run final validation, smoke test when possible, update shared SDD context and commit.

## Framework And SDK

- Target framework: `net10.0` in every `.csproj`.
- SDK: `10.0.302`, because it is installed and is the highest available .NET 10 SDK in this environment.
- No `rollForward` is needed initially because the exact SDK exists locally.

## Common Build Properties

Add `Directory.Build.props` with:

```xml
<Nullable>disable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<AnalysisLevel>latest-recommended</AnalysisLevel>
```

Rationale:

- `ImplicitUsings` and analyzer level modernize compilation without changing runtime behavior.
- `Nullable` is explicitly disabled for now because the legacy codebase was written without nullable annotations. Enabling it globally would create broad warning churn and is better handled in a dedicated hardening prompt.
- Do not enable `TreatWarningsAsErrors`; the goal is a trustworthy migration baseline, not warning cleanup by force.

## Package Treatment

- Use `10.0.10` for Microsoft ASP.NET Core, EF Core and `Microsoft.Extensions.*` packages still needed outside the shared framework.
- Remove `Microsoft.Extensions.Caching.Redis` and use only `Microsoft.Extensions.Caching.StackExchangeRedis`.
- Remove explicit `Microsoft.Extensions.DependencyInjection` from the API.
- Remove `Microsoft.CodeAnalysis.FxCopAnalyzers`; use SDK analyzers.
- Remove `Serilog.Sinks.ColoredConsole`; use `Serilog.Sinks.Console`.
- Replace `Serilog.Filters.Expressions` with `Serilog.Expressions` if string expression filters are still required.
- Keep `AspNetCoreRateLimit` temporarily at its latest package line; replacing it with native rate limiting belongs to a future prompt.
- Keep legacy `Microsoft.AspNetCore.Mvc.Versioning*` temporarily unless it blocks build; migration to `Asp.Versioning.*` belongs to the OpenAPI/versioning prompt.
- Keep health checks packages on their latest available `9.0.0` line as a compatibility bridge where compatible.
- Keep `/hc` but temporarily disable HealthChecks UI web `/hc-ui` because the latest available UI package is not runtime-compatible with EF Core 10.
- Use AutoMapper `16.2.0` and remove the old DI extension package.
- Update FluentValidation to a modern supported line and fix removed cascade mode syntax if required.
- Update test infrastructure packages to current versions that support .NET 10.

## Warning Strategy

- Fix warnings only when they indicate an actual build, runtime or validation blocker.
- Document remaining warnings in validation/report files.
- Leave broad nullable, analyzer and style cleanup for a later prompt.

## Behavior Preservation

- Do not edit controllers for contract changes.
- Do not regenerate or modify EF migrations unless compilation proves it is unavoidable.
- Do not change authentication settings, JWT validation parameters, route templates, status code handling or error payload shapes.
- Preserve the legacy startup file and host hook until the hosting modernization prompt.
