# Handoff - Phase 2 Task 01

## Branch

- Current branch: `phase/2-dotnet-10-migration`
- Branch base: `phase/1-preserve-legacy`
- Base SHA: `2799562943ac03926d69bc716617d091d04ecc82`
- Source repository SHA: `9029163f1a795a1bb18f138dd8fa9179f13f544e`
- Initial SHA for task 01: `1bcce0d290b6c9adf20a34287b357e174db7a202`
- Task 01 commit: pending until delivery command completes.

## Current Runtime

- SDK pinned by `global.json`: `10.0.302`
- Active target framework: `net10.0` in API, Business, Data and test projects.
- Common build props: `Nullable=disable`, `ImplicitUsings=enable`, `AnalysisLevel=latest-recommended`.

## Updated Packages

- Microsoft ASP.NET Core/EF Core/Microsoft.Extensions packages updated to `10.0.10` where still referenced.
- FluentValidation `12.1.1`.
- AutoMapper `16.2.0`.
- Serilog ASP.NET Core `10.0.0`, Console `6.1.1`, Seq `9.1.0`, Expressions `5.0.0`.
- Health checks Redis/SQL Server/Uris/UI.Client `9.0.0`.
- AspNetCoreRateLimit `5.0.0`.
- Test packages: Microsoft.NET.Test.Sdk `18.8.1`, xUnit `2.9.3`, xUnit runner `3.1.5`, Moq `4.20.72`, Bogus `35.6.5`, coverlet.collector `10.0.1`.
- KubernetesClient `19.0.2` added as private override for a vulnerable transitive package.

## Removed Packages

- `Microsoft.CodeAnalysis.FxCopAnalyzers`
- `Microsoft.Extensions.Caching.Redis`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Logging.Debug`
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- `Serilog.Filters.Expressions`
- `Serilog.Sinks.ColoredConsole`
- `Swashbuckle.AspNetCore.Swagger`
- `Swashbuckle.AspNetCore.SwaggerGen`
- `AutoMapper.Extensions.Microsoft.DependencyInjection`
- `AspNetCore.HealthChecks.UI`

## Temporary Compatibility

- `Startup.cs` and `UseStartup` remain for issue `#5`.
- `AspNetCoreRateLimit` remains for the native rate limiting prompt.
- `Microsoft.AspNetCore.Mvc.Versioning*` remains for the OpenAPI/versioning prompt and is reported deprecated by NuGet.
- Swashbuckle remains at `6.9.0`, although `10.2.3` exists, to avoid premature OpenAPI API-surface changes.
- xUnit remains v2 and is reported as legacy by NuGet; xUnit v3 migration is deferred.
- HealthChecks UI web `/hc-ui` is disabled until a .NET 10-compatible strategy is selected; `/hc` remains.

## Validation State

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed with analyzer warnings.
- `dotnet test --configuration Release --no-build`: passed, 21 tests.
- `dotnet list package --vulnerable`: no vulnerable packages.
- `dotnet list package --outdated`: only Swashbuckle `6.9.0` -> `10.2.3`.
- `dotnet list package --deprecated`: API Versioning packages and xUnit v2.
- Smoke: API starts; Swagger `/swagger/v1/swagger.json` returns `200`; `/error/404` returns `404`; `/hc` starts SQL health check but does not return before timeout without local SQL Server.

## Main Files Changed

- `global.json`
- `Directory.Build.props`
- `src/DevIO.Api/Restaurante.IO.Api.csproj`
- `src/DevIO.Business/Restaurante.IO.Business.csproj`
- `src/DevIO.Data/Restaurante.IO.Data.csproj`
- `test/Pedidos.Test/Pedidos.Test.csproj`
- Minimal compatibility edits in `Program.cs`, `Startup.cs`, cache, rate limit, Serilog middleware and FluentValidation validators.
- `.sdd/phase-2/01-dotnet-10-migration/`

## Restrictions For Next Prompt

- Do not repeat the framework/package migration already completed here.
- Do not move `legacy/netcoreapp3.1` or `v1.0.0-legacy`.
- Preserve behavior unless the next prompt explicitly changes hosting.
- Review HealthChecks UI separately; do not silently re-enable `/hc-ui` with the incompatible EF Core 9-backed package.

## Next Objective

```text
#5 - Modernize hosting model
```
