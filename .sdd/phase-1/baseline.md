# Baseline - Phase 1 Prompt 01

## Baseline Identity

| Field | Value |
| --- | --- |
| Repository | `rodri-oliveira-dev/web-api-core-seed` |
| Remote URL | `https://github.com/rodri-oliveira-dev/web-api-core-seed.git` |
| Source branch before Prompt 01 | `main` |
| Phase branch | `phase/1-preserve-legacy` |
| `legacy_source_sha` | `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d` |
| Latest legacy commit date | `2020-09-01 08:34:26 -0300` |
| Latest legacy commit message | `Criacao dos testes unitarios de validators` |

## Required Files Read Before Edits

Confirmed absent:

- `AGENTS.md`
- `.agents/`
- `global.json`
- `.github/` workflows

Read files:

- `README.md`
- `src/README.md`
- `RestauranteAPI.sln`
- `src/DevIO.Api/Restaurante.IO.Api.csproj`
- `src/DevIO.Business/Restaurante.IO.Business.csproj`
- `src/DevIO.Data/Restaurante.IO.Data.csproj`
- `test/Pedidos.Test/Pedidos.Test.csproj`
- `.vscode/launch.json`
- `.vscode/tasks.json`
- `src/DevIO.Api/wwwroot/app/.vscode/launch.json`
- `src/DevIO.Api/appsettings.json`
- `src/DevIO.Api/appsettings.Development.json`
- `src/DevIO.Api/Program.cs`
- `src/DevIO.Api/Startup.cs`
- `src/DevIO.Api/web.config`
- `src/DevIO.Api/Resources/ConnectionString.cs`
- `src/DevIO.Api/Configuration/ApiConfig.cs`
- `src/DevIO.Api/Configuration/CacheConfig.cs`
- `src/DevIO.Api/Configuration/IdentityConfig.cs`
- `src/DevIO.Api/Configuration/DependencyInjectionConfig.cs`
- `src/DevIO.Api/Configuration/RateLimitConfig.cs`
- `src/DevIO.Api/DataContext/ApplicationContext.cs`
- `src/DevIO.Data/Context/MeuDbContext.cs`
- `src/DevIO.Api/Migrations/20200817223121_InitialCreate.cs`
- `src/DevIO.Data/Migrations/20200817223231_InitialCreate.cs`
- `docker/SqlServer.dockerfile_`
- `docker/redis.dockerfile`
- `docker/datalust-seq.dockerfile`
- `sql/restaurante.sql`
- `src/sonar-project.properties`
- `src/sonar-push.bat`

## Solution Structure

Confirmed projects in `RestauranteAPI.sln`:

| Project | Path | Type | Target framework |
| --- | --- | --- | --- |
| `Restaurante.IO.Business` | `src/DevIO.Business/Restaurante.IO.Business.csproj` | Class library | `netcoreapp3.1` |
| `Restaurante.IO.Data` | `src/DevIO.Data/Restaurante.IO.Data.csproj` | Class library | `netcoreapp3.1` |
| `Restaurante.IO.Api` | `src/DevIO.Api/Restaurante.IO.Api.csproj` | ASP.NET Core Web API | `netcoreapp3.1` |
| `Pedidos.Test` | `test/Pedidos.Test/Pedidos.Test.csproj` | xUnit test project | `netcoreapp3.1` |

Confirmed project references:

- `Restaurante.IO.Api` references `Restaurante.IO.Business` and `Restaurante.IO.Data`.
- `Restaurante.IO.Data` references `Restaurante.IO.Business`.
- `Pedidos.Test` references `Restaurante.IO.Business`.

## SDK And Runtime

Confirmed:

- No `global.json` exists in the repository.
- `dotnet --info` reported SDK `10.0.302` as active.
- Available SDKs: `8.0.423`, `10.0.110`, `10.0.204`, `10.0.302`.
- Installed runtimes include ASP.NET Core and .NET runtimes for 8.0 and 10.0 only.
- .NET Core 3.1 SDK/runtime is not installed in this environment.

Deducted:

- Because no `global.json` exists, the current machine selects the latest installed SDK by default.
- The README refers to a `global.json`, but none is present in this checkout.

## NuGet Packages

Confirmed from `.csproj` files.

### `src/DevIO.Api/Restaurante.IO.Api.csproj`

| Package | Version |
| --- | --- |
| `AspNetCore.HealthChecks.Redis` | `3.0.0` |
| `AspNetCore.HealthChecks.SqlServer` | `3.0.0` |
| `AspNetCore.HealthChecks.ui` | `3.0.9` |
| `AspNetCore.HealthChecks.Uris` | `3.0.0` |
| `AspNetCoreRateLimit` | `3.0.5` |
| `AutoMapper` | `9.0.0` |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | `7.0.0` |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `3.1.2` |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | `3.1.2` |
| `Microsoft.AspNetCore.Identity.UI` | `3.1.2` |
| `Microsoft.AspNetCore.Mvc.Versioning` | `4.1.1` |
| `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` | `4.1.1` |
| `Microsoft.CodeAnalysis.FxCopAnalyzers` | `2.9.8` |
| `Microsoft.EntityFrameworkCore` | `3.1.2` |
| `Microsoft.EntityFrameworkCore.Tools` | `3.1.2` |
| `Microsoft.Extensions.Caching.Redis` | `2.2.0` |
| `Microsoft.Extensions.Caching.StackExchangeRedis` | `3.1.2` |
| `Microsoft.Extensions.DependencyInjection` | `3.1.2` |
| `Microsoft.Extensions.Logging.Debug` | `3.1.2` |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | `3.1.1` |
| `Serilog.AspNetCore` | `3.2.0` |
| `Serilog.Filters.Expressions` | `2.1.0` |
| `Serilog.Sinks.ColoredConsole` | `3.0.1` |
| `Serilog.Sinks.Console` | `3.1.1` |
| `Serilog.Sinks.Seq` | `4.0.0` |
| `Swashbuckle.AspNetCore` | `5.0.0` |
| `Swashbuckle.AspNetCore.Swagger` | `5.0.0` |
| `Swashbuckle.AspNetCore.SwaggerGen` | `5.0.0` |

### `src/DevIO.Business/Restaurante.IO.Business.csproj`

| Package | Version |
| --- | --- |
| `FluentValidation` | `8.6.1` |
| `Microsoft.Extensions.Logging.Abstractions` | `3.1.2` |

### `src/DevIO.Data/Restaurante.IO.Data.csproj`

| Package | Version |
| --- | --- |
| `Microsoft.EntityFrameworkCore.SqlServer` | `3.1.2` |

### `test/Pedidos.Test/Pedidos.Test.csproj`

| Package | Version |
| --- | --- |
| `Bogus` | `30.0.4` |
| `Microsoft.NET.Test.Sdk` | `16.5.0` |
| `Moq` | `4.14.5` |
| `xunit` | `2.4.0` |
| `xunit.runner.visualstudio` | `2.4.0` |
| `coverlet.collector` | `1.2.0` |

## Application Runtime

Confirmed:

- Entry point: `src/DevIO.Api/Program.cs`.
- Host builder: `Host.CreateDefaultBuilder(args)` with `UseSerilog()` and `UseIIS()`.
- Startup class: `src/DevIO.Api/Startup.cs`.
- VS Code launch target: `src/DevIO.Api/bin/Debug/netcoreapp3.1/Restaurante.IO.Api.dll`.
- VS Code launch current directory: `src/DevIO.Api`.
- VS Code environment: `ASPNETCORE_ENVIRONMENT=Development`.
- No `Properties/launchSettings.json` file was found.

Deducted command to run the API:

```powershell
dotnet run --project src/DevIO.Api/Restaurante.IO.Api.csproj
```

This command is identified from the SDK-style web project and VS Code launch/task configuration. It is not yet successfully verified in this environment.

## Database And Persistence

Confirmed:

- Database provider: SQL Server through Entity Framework Core.
- Connection string source: `ConnectionStrings:DefaultConnection` in `src/DevIO.Api/appsettings.json`.
- Application domain DbContext: `Restaurante.IO.Data.Context.MeuDbContext`.
- Identity DbContext: `Restaurante.IO.Api.DataContext.ApplicationDbContext`.
- SQL script location: `sql/restaurante.sql`.
- SQL script creates database `restaurante`.
- Application connection string targets catalog `PedidosApi`.

Potential mismatch:

- The SQL script database name is `restaurante`, while the application connection string initial catalog is `PedidosApi`.

## Migrations

Confirmed locations:

- Domain/data migrations: `src/DevIO.Data/Migrations/`
- Identity/API migrations: `src/DevIO.Api/Migrations/`

Confirmed migration files:

- `src/DevIO.Data/Migrations/20200817223231_InitialCreate.cs`
- `src/DevIO.Data/Migrations/20200817223231_InitialCreate.Designer.cs`
- `src/DevIO.Data/Migrations/MeuDbContextModelSnapshot.cs`
- `src/DevIO.Api/Migrations/20200817223121_InitialCreate.cs`
- `src/DevIO.Api/Migrations/20200817223121_InitialCreate.Designer.cs`
- `src/DevIO.Api/Migrations/ApplicationDbContextModelSnapshot.cs`

Deducted migration commands, not yet verified:

```powershell
dotnet ef database update --project src/DevIO.Data/Restaurante.IO.Data.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context MeuDbContext
dotnet ef database update --project src/DevIO.Api/Restaurante.IO.Api.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context ApplicationDbContext
```

Still not verified:

- Whether `dotnet-ef` is installed.
- Whether both contexts can update the same configured catalog successfully.
- Whether the SQL script is intended as an alternative to EF migrations.

## Seed

Confirmed:

- No direct references to `Seed`, `EnsureCreated`, or automatic `Migrate()` were found by repository text search.

Still not verified:

- Whether seed data exists outside the searched source files or is expected to be applied manually.

## External Dependencies

Confirmed:

| Dependency | Evidence | Default location/configuration |
| --- | --- | --- |
| SQL Server | EF Core SQL Server packages, `UseSqlServer`, Dockerfile, connection string | `localhost,1433` |
| Redis | Redis cache settings, health check, Dockerfile | `localhost:7001` in app settings; Dockerfile exposes `6379` |
| Seq / Datasul Seq | Serilog Seq sink, health check, Dockerfile | `http://localhost:5341` |
| IIS/IIS Express hosting support | `UseIIS()`, `web.config`, `AspNetCoreHostingModel=InProcess` | No launch profile found |

## Sensitive Configuration Locations

These files may contain sensitive or environment-specific values and should be handled carefully:

| File | Sensitive content type |
| --- | --- |
| `src/DevIO.Api/appsettings.json` | SQL Server connection string, SQL credentials, JWT secret, Redis endpoint, Seq URL, local log file path |
| `docker/SqlServer.dockerfile_` | SQL Server SA password environment variable |
| `src/DevIO.Api/web.config` | Hosting/server behavior |
| `.vscode/launch.json` | Runtime paths and environment names |
| `.vscode/tasks.json` | Build/publish commands and project paths |

Values are intentionally not copied into this SDD artifact.

## Current Commands

Confirmed or deduced current commands:

| Purpose | Command | Status |
| --- | --- | --- |
| Restore dependencies | `dotnet restore` | Blocked by local NuGet metadata error |
| Build solution | `dotnet build --no-restore` | Blocked because restore assets are missing |
| Run tests | `dotnet test --no-build` | Inconclusive; exit code `0` with no output after failed build |
| Run API | `dotnet run --project src/DevIO.Api/Restaurante.IO.Api.csproj` | Deduced; bounded `--no-build` check failed because executable is missing |
| VS Code API build task | `dotnet build ${workspaceFolder}/src/DevIO.Api/Restaurante.IO.Api.csproj /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary` | Confirmed from `.vscode/tasks.json` |
| VS Code API publish task | `dotnet publish ${workspaceFolder}/src/DevIO.Api/Restaurante.IO.Api.csproj /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary` | Confirmed from `.vscode/tasks.json` |
| VS Code API watch task | `dotnet watch run ${workspaceFolder}/src/DevIO.Api/Restaurante.IO.Api.csproj /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary` | Confirmed from `.vscode/tasks.json` |
| Apply data migrations | `dotnet ef database update --project src/DevIO.Data/Restaurante.IO.Data.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context MeuDbContext` | Deduced, not verified |
| Apply identity migrations | `dotnet ef database update --project src/DevIO.Api/Restaurante.IO.Api.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context ApplicationDbContext` | Deduced, not verified |
| Execute seed | None identified | Not verified / no seed command found |

## Known Issues Before Validation

Confirmed:

- .NET Core 3.1 SDK/runtime is absent.
- A local NuGet cache metadata file for `microsoft.netcore.targets/1.1.0` is invalid; `dotnet list RestauranteAPI.sln package` failed while trying to restore.
- README instructs checking `global.json`, but no `global.json` exists.

Deducted:

- Validation may be blocked by the missing .NET Core 3.1 runtime/SDK and/or corrupt local NuGet metadata.
- The application likely needs SQL Server, Redis, and Seq running locally for full startup health.

Still not verified:

- Whether restore/build/test can complete with the available .NET 10 SDK.
- Whether the API can start without SQL Server, Redis, and Seq services running.
- Whether EF migration commands work with the current project layout.

## Validation Command Results

### `git status`

- Command executed: `git status --short`
- Result: empty output.
- Exit code: `0`.
- Error found: none.
- Interpretation: working tree was clean before Prompt 01 edits.
- Impact for preservation: safe to create documentation artifacts without touching preexisting work.

### `git branch --show-current`

- Command executed: `git branch --show-current`
- Result: `main`.
- Exit code: `0`.
- Error found: none.
- Interpretation: baseline discovery started from `main`.
- Impact for preservation: `phase/1-preserve-legacy` must be created from `main` at `legacy_source_sha`.

### `git log -5 --oneline`

- Command executed: `git log -5 --oneline`
- Result:

```text
6ce03d7 Criacao dos testes unitarios de validators
378f84a Merge branch 'master' of https://github.com/RodrigoDotNet/web-api-core-seed
8598bc3 *  Refatoracao do projeto * Criacao do HealthChecks/MemoryMetrics * Ajuste nas tratativas de erro
03aaec3 Update README.md
ba54463 Merge branch 'master' of https://github.com/RodrigoDotNet/web-api-core-seed
```

- Exit code: `0`.
- Error found: none.
- Interpretation: the latest legacy commit before this delivery is `6ce03d7`.
- Impact for preservation: full SHA recorded as `legacy_source_sha`.

### `git rev-parse HEAD`

- Command executed: `git rev-parse HEAD`
- Result: `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d`.
- Exit code: `0`.
- Error found: none.
- Interpretation: this is the commit-base for the legacy state.
- Impact for preservation: use this SHA as `legacy_source_sha`.

### `dotnet --info`

- Command executed: `dotnet --info`
- Result summary: active SDK `10.0.302`, host `10.0.10`, Windows `10.0.26200`, no `global.json`.
- Exit code: `0`.
- Error found: none.
- Interpretation: .NET is installed, but not the legacy .NET Core 3.1 SDK/runtime.
- Impact for preservation: validations for `netcoreapp3.1` may be blocked by environment.

### `dotnet --list-sdks`

- Command executed: `dotnet --list-sdks`
- Result:

```text
8.0.423
10.0.110
10.0.204
10.0.302
```

- Exit code: `0`.
- Error found: none.
- Interpretation: .NET Core 3.1 SDK is unavailable.
- Impact for preservation: do not install SDK, alter `global.json`, or change target frameworks; record limitation only.

### `dotnet list RestauranteAPI.sln package`

- Command executed: `dotnet list RestauranteAPI.sln package`
- Result: failed while attempting restore.
- Exit code: `1`.
- Error found:

```text
Erro ao analisar o arquivo de metadados nupkg C:\Users\rodrigooliveira\.nuget\packages\microsoft.netcore.targets\1.1.0\.nupkg.metadata : '0x00' is an invalid start of a value.
```

- Interpretation: local NuGet package metadata appears corrupt for `microsoft.netcore.targets/1.1.0`.
- Impact for preservation: package versions were recorded from `.csproj` files instead of relying on the failed CLI command.

### `dotnet restore`

- Command executed: `dotnet restore`
- Result: failed.
- Exit code: `1`.
- Error found:

```text
NETSDK1138 warning: target framework 'netcoreapp3.1' is out of support.
NuGet error: invalid metadata file C:\Users\rodrigooliveira\.nuget\packages\microsoft.netcore.targets\1.1.0\.nupkg.metadata.
'0x00' is an invalid start of a value.
```

- Interpretation: restore is blocked by a local NuGet cache metadata problem before the full solution can restore. The active .NET 10 SDK can evaluate the project but warns about the unsupported target framework.
- Impact for preservation: validation is blocked by the environment; do not alter project files, target frameworks, package versions, or SDK settings in this phase.

### `dotnet build --no-restore`

- Command executed: `dotnet build --no-restore`
- Result: failed.
- Exit code: `1`.
- Error found:

```text
NETSDK1004: project.assets.json not found for src/DevIO.Data/Restaurante.IO.Data.csproj.
NETSDK1004: project.assets.json not found for test/Pedidos.Test/Pedidos.Test.csproj.
NETSDK1004: project.assets.json not found for src/DevIO.Api/Restaurante.IO.Api.csproj.
```

- Additional result: `Restaurante.IO.Business` compiled to `src/DevIO.Business/bin/Debug/netcoreapp3.1/Restaurante.IO.Business.dll`, because its restore assets had been generated by the earlier failed `dotnet list` attempt.
- Interpretation: build cannot complete after the failed restore.
- Impact for preservation: build validation is blocked by environment/cache state, not by a code change from this delivery.

### `dotnet test --no-build`

- Command executed: `dotnet test --no-build`
- Result: command returned no output.
- Exit code: `0`.
- Error found: none reported.
- Interpretation: inconclusive. Because the build failed and test binaries were not confirmed, this exit code does not prove that tests passed.
- Impact for preservation: test validation should be repeated in a compatible/restored environment.

### API Run Command Identification

- Command identified from project and VS Code configuration:

```powershell
dotnet run --project src/DevIO.Api/Restaurante.IO.Api.csproj
```

- Command executed for bounded verification:

```powershell
dotnet run --project src\DevIO.Api\Restaurante.IO.Api.csproj --no-build
```

- Result: failed quickly.
- Exit code: `1`.
- Error found:

```text
Could not start process src\DevIO.Api\bin\Debug\netcoreapp3.1\Restaurante.IO.Api.exe because the system cannot find the file specified.
```

- Interpretation: the run command shape is identified, but runtime startup cannot be verified because build output is missing.
- Impact for preservation: repeat after restore/build succeeds in a compatible environment.

### `git diff --check`

- Command executed: `git diff --check`
- Result: no whitespace errors. After `git add -N .sdd/phase-1`, Git emitted line-ending normalization warnings that LF will be replaced by CRLF on Windows.
- Exit code: `0`.
- Error found: none.
- Interpretation: no whitespace errors were detected; CRLF normalization warnings are environmental and non-blocking.
- Impact for preservation: no formatting blocker identified.

### `git diff`

- Command executed: `git diff`
- Result: after `git add -N .sdd/phase-1`, diff showed only new files under `.sdd/phase-1/`.
- Exit code: `0`.
- Error found: none.
- Interpretation: only the required SDD artifacts are in scope.
- Impact for preservation: source code, project files, solution, dependencies, migrations, configuration, tests, README, and workflows were not modified.
