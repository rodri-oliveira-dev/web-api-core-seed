# Discovery - 01 .NET 10 Migration

## Repository State

Initial commands:

| Command | Exit code | Result |
| --- | ---: | --- |
| `git status --short` | 0 | Clean working tree. |
| `git branch --show-current` | 0 | `phase/2-dotnet-10-migration`. |
| `git log -5 --oneline` | 0 | HEAD starts at `1bcce0d chore: bootstrap modernization tooling`. |
| `git rev-parse HEAD` | 0 | `1bcce0d290b6c9adf20a34287b357e174db7a202`. |
| `dotnet --info` | 0 | SDK `10.0.302`, host runtime `10.0.10`; no `global.json`. |
| `dotnet --list-sdks` | 0 | `8.0.423`, `10.0.110`, `10.0.204`, `10.0.302`. |
| `dotnet --list-runtimes` | 0 | ASP.NET Core and .NETCore runtimes `8.0.29`, `10.0.8`, `10.0.10`. |

Confirmed governance:

- Branch is correct.
- Working tree was clean before the task.
- Bootstrap commit exists at HEAD.
- Phase 1 is documented as preserved in `LEGACY.md` and `.sdd/phase-2/status.md`.
- No other pending work was present.

## Current Project Targets

| Project | SDK | Target framework |
| --- | --- | --- |
| `src/DevIO.Api/Restaurante.IO.Api.csproj` | `Microsoft.NET.Sdk.Web` | `netcoreapp3.1` |
| `src/DevIO.Business/Restaurante.IO.Business.csproj` | `Microsoft.NET.Sdk` | `netcoreapp3.1` |
| `src/DevIO.Data/Restaurante.IO.Data.csproj` | `Microsoft.NET.Sdk` | `netcoreapp3.1` |
| `test/Pedidos.Test/Pedidos.Test.csproj` | `Microsoft.NET.Sdk` | `netcoreapp3.1` |

No `.slnx`, `global.json`, `Directory.Build.props`, `Directory.Packages.props` or `NuGet.config` existed initially.

## Baseline Validation

| Command | Exit code | Observed result |
| --- | ---: | --- |
| `dotnet restore` | 1 | Failed before migration. SDK emitted `NETSDK1138` for `netcoreapp3.1` and NuGet failed parsing `<user-home>\.nuget\packages\microsoft.netcore.targets\1.1.0\.nupkg.metadata` because it starts with `0x00`. |
| `dotnet build --no-restore` | 1 | Failed because assets files were missing for API, Data and test projects; Business built from existing state but still targeted `netcoreapp3.1`. |
| `dotnet test --no-build` | 0 | No output. Recorded as inconclusive because restore/build had not succeeded. |

## Package Inventory

### API

- `AspNetCore.HealthChecks.Redis` `3.0.0`
- `AspNetCore.HealthChecks.SqlServer` `3.0.0`
- `AspNetCore.HealthChecks.ui` `3.0.9`
- `AspNetCore.HealthChecks.Uris` `3.0.0`
- `AspNetCoreRateLimit` `3.0.5`
- `AutoMapper` `9.0.0`
- `AutoMapper.Extensions.Microsoft.DependencyInjection` `7.0.0`
- `Microsoft.AspNetCore.Authentication.JwtBearer` `3.1.2`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` `3.1.2`
- `Microsoft.AspNetCore.Identity.UI` `3.1.2`
- `Microsoft.AspNetCore.Mvc.Versioning` `4.1.1`
- `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` `4.1.1`
- `Microsoft.CodeAnalysis.FxCopAnalyzers` `2.9.8`
- `Microsoft.EntityFrameworkCore` `3.1.2`
- `Microsoft.EntityFrameworkCore.Tools` `3.1.2`
- `Microsoft.Extensions.Caching.Redis` `2.2.0`
- `Microsoft.Extensions.Caching.StackExchangeRedis` `3.1.2`
- `Microsoft.Extensions.DependencyInjection` `3.1.2`
- `Microsoft.Extensions.Logging.Debug` `3.1.2`
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` `3.1.1`
- `Serilog.AspNetCore` `3.2.0`
- `Serilog.Filters.Expressions` `2.1.0`
- `Serilog.Sinks.ColoredConsole` `3.0.1`
- `Serilog.Sinks.Console` `3.1.1`
- `Serilog.Sinks.Seq` `4.0.0`
- `Swashbuckle.AspNetCore` `5.0.0`
- `Swashbuckle.AspNetCore.Swagger` `5.0.0`
- `Swashbuckle.AspNetCore.SwaggerGen` `5.0.0`

### Business

- `FluentValidation` `8.6.1`
- `Microsoft.Extensions.Logging.Abstractions` `3.1.2`

### Data

- `Microsoft.EntityFrameworkCore.SqlServer` `3.1.2`

### Tests

- `Bogus` `30.0.4`
- `Microsoft.NET.Test.Sdk` `16.5.0`
- `Moq` `4.14.5`
- `xunit` `2.4.0`
- `xunit.runner.visualstudio` `2.4.0`
- `coverlet.collector` `1.2.0`

## NuGet Version Discovery

The NuGet flat container was queried for current stable versions available to this environment. Relevant results:

| Package | Latest stable observed |
| --- | --- |
| Microsoft ASP.NET Core packages | `10.0.10` |
| Microsoft EF Core packages | `10.0.10` |
| Microsoft.Extensions packages | `10.0.10` |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | `10.0.2` |
| AspNetCore.HealthChecks.* | `9.0.0` |
| AspNetCoreRateLimit | `5.0.0` |
| Microsoft.AspNetCore.Mvc.Versioning* | `5.1.0` |
| Asp.Versioning.Mvc* | `10.0.1` |
| AutoMapper | `16.2.0` |
| AutoMapper.Extensions.Microsoft.DependencyInjection | `12.0.1` |
| FluentValidation | `12.1.1` |
| Serilog.AspNetCore | `10.0.0` |
| Serilog.Expressions | `5.0.0` |
| Serilog.Sinks.Console | `6.1.1` |
| Serilog.Sinks.Seq | `9.1.0` |
| Swashbuckle.AspNetCore* | `10.2.3` |
| Microsoft.NET.Test.Sdk | `18.8.1` |
| xunit | `2.9.3` |
| xunit.runner.visualstudio | `3.1.5` |
| coverlet.collector | `10.0.1` |

## Obsolete Or Duplicated Packages

- `Microsoft.CodeAnalysis.FxCopAnalyzers` is obsolete; SDK analyzers replace it.
- `Microsoft.Extensions.Caching.Redis` is obsolete and duplicated by `Microsoft.Extensions.Caching.StackExchangeRedis`.
- `Microsoft.Extensions.DependencyInjection` is supplied by the ASP.NET Core shared framework for the API and is not needed explicitly.
- Separate `Swashbuckle.AspNetCore.Swagger` and `Swashbuckle.AspNetCore.SwaggerGen` references duplicate the aggregate `Swashbuckle.AspNetCore` package.
- `Serilog.Sinks.ColoredConsole` is a legacy sink; `Serilog.Sinks.Console` remains enough for console logging.
- `Serilog.Filters.Expressions` is legacy and can be replaced by `Serilog.Expressions` if the string filter overload is needed.
- `AutoMapper.Extensions.Microsoft.DependencyInjection` can be removed because the selected AutoMapper line includes DI registration.
- `AspNetCore.HealthChecks.UI` 9.0.0 requires EF-backed storage and failed at runtime with EF Core 10 through `MissingMethodException` in EF InMemory initialization.

## Problematic API Search

| Search | Result |
| --- | --- |
| `IgnoreNullValues` | Found in `Startup.cs` and `ResponseCacheService.cs`; replaced by `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull`. |
| `SetCompatibilityVersion` | Found in `ApiConfig.cs`; compatibility version API is legacy and should be removed if it blocks build. |
| `IWebHostBuilder` | Found in `Program.cs`; retained because hosting modernization is future scope. |
| Legacy startup host hook | Found in `Program.cs`; retained because hosting modernization is future scope. |
| `AddMvc` | Found in `Startup.cs` and `ApiConfig.cs`; retained unless compilation requires a minimal adjustment. |
| `UseMvc` | No direct legacy `UseMvc` call found. |
| `Microsoft.Extensions.Caching.Redis` | Found in API `.csproj`; remove and keep StackExchange Redis cache. |
| `FxCopAnalyzers` | Found in API `.csproj`; remove and rely on SDK analyzers. |

## Possible Breaking Changes

- `JsonSerializerOptions.IgnoreNullValues` was removed from modern `System.Text.Json`.
- `CompatibilityVersion`/`SetCompatibilityVersion` is not part of the modern ASP.NET Core MVC surface.
- Header mutation through `IHeaderDictionary.Add` can throw when duplicate headers are written; changing to index assignment may be required if analyzers or runtime checks flag it, but behavior should stay equivalent.
- HealthChecks UI package casing differs in common modern references; normalize to `AspNetCore.HealthChecks.UI`.
- HealthChecks UI latest stable is 9.0.0 and no 10.x package was available in NuGet during this task; enabling the UI with EF Core 10 blocked API startup.
- API Versioning package has moved to `Asp.Versioning.*`; this is intentionally deferred to the OpenAPI/versioning prompt unless old package blocks the migration.
- Rate limiting package remains a compatibility bridge until the native rate limiting prompt.
- Nullable enablement on legacy models and EF contexts would produce many warnings; it should not be a blocker for the initial migration.
