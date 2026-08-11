# Legacy Runtime And Usage

This document records the confirmed legacy behavior of `rodri-oliveira-dev/web-api-core-seed` for Phase 1 preservation. It intentionally does not modernize runtime, dependencies, architecture, code, migrations, tests, or configuration.

## 1. Support State

- The solution targets `.NET Core 3.1` through `netcoreapp3.1` in every project.
- .NET Core 3.1 reached end of support on December 13, 2022.
- This repository is preserved as historical reference only and should not be used as the base for new projects.
- A future modernization to .NET 10 is planned after the legacy state is preserved.
- At the end of Phase 1, this legacy version will be preserved by the planned tag `v1.0.0-legacy` and planned branch `legacy/netcoreapp3.1`.

## 2. Historical Requirements

Confirmed repository files:

- Solution: `RestauranteAPI.sln`
- API project: `src/DevIO.Api/Restaurante.IO.Api.csproj`
- Business project: `src/DevIO.Business/Restaurante.IO.Business.csproj`
- Data project: `src/DevIO.Data/Restaurante.IO.Data.csproj`
- Test project: `test/Pedidos.Test/Pedidos.Test.csproj`

Historical tooling expected by the project:

- Visual Studio 2019, based on the original README and solution metadata.
- .NET Core 3.1 SDK/runtime.
- Entity Framework Core CLI compatible with EF Core 3.1, for example a `dotnet-ef` 3.1.x tool.

Confirmed limitation:

- No `global.json` exists in this checkout, although the historical README mentions one.

## 3. External Dependencies

The API uses external services configured in `src/DevIO.Api/appsettings.json`.

| Dependency | Confirmed evidence | Default location |
| --- | --- | --- |
| SQL Server | EF Core SQL Server provider, `UseSqlServer`, health check, SQL Server Dockerfile | `localhost,1433` through `ConnectionStrings:DefaultConnection` |
| Redis | Redis cache settings, response cache service, Redis health check, Redis Dockerfile | `localhost:7001` through `RedisCacheSettings:ConnectionString` |
| Seq | Serilog Seq sink, Seq health check, Seq Dockerfile | `http://localhost:5341` through `DatasulSeqSettings:Url` |

Dockerfiles exist in `docker/`, but no `docker-compose` file was identified. The Redis Dockerfile exposes `6379`, while the API configuration points to `localhost:7001`. The SQL Server Dockerfile and the API connection string also contain environment-specific credentials that should be reviewed locally before use.

## 4. Configuration

Configuration is loaded by `src/DevIO.Api/Program.cs` from:

- `src/DevIO.Api/appsettings.json`
- optional `src/DevIO.Api/appsettings.{ASPNETCORE_ENVIRONMENT}.json`
- environment variables

Relevant configuration keys:

- `ConnectionStrings:DefaultConnection`
- `AppSettings:Secret`
- `AppSettings:ExpiracaoHoras`
- `AppSettings:Emissor`
- `AppSettings:ValidoEm`
- `RedisCacheSettings:Enabled`
- `RedisCacheSettings:ConnectionString`
- `RedisCacheSettings:InstanceName`
- `RedisCacheSettings:DefaultSeconds`
- `DatasulSeqSettings:Enabled`
- `DatasulSeqSettings:Url`
- `DatasulSeqSettings:FilePath`
- `IpRateLimiting`
- `HealthChecks-UI`

The VS Code launch configuration sets:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

No `Properties/launchSettings.json` file was identified. No explicit Kestrel `UseUrls` or `applicationUrl` setting was identified in the repository. The health check UI configuration references `https://localhost:44340/hc`, but the runtime port was not successfully verified in this environment.

## 5. Restore

From the repository root:

```powershell
dotnet restore RestauranteAPI.sln
```

The validation command requested for this phase was also run from the repository root:

```powershell
dotnet restore
```

Current validation result: restore failed in this environment because the local NuGet cache contains invalid metadata for `microsoft.netcore.targets/1.1.0`. The active SDK also warns that `netcoreapp3.1` is out of support.

## 6. Build

From the repository root, after a successful restore:

```powershell
dotnet build RestauranteAPI.sln --no-restore
```

The validation command requested for this phase was also run from the repository root:

```powershell
dotnet build --no-restore
```

Current validation result: build failed because restore did not complete and `project.assets.json` files were missing for the API, Data, and test projects.

## 7. Run The API

From the repository root:

```powershell
dotnet run --project src/DevIO.Api/Restaurante.IO.Api.csproj
```

For Development configuration in PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src/DevIO.Api/Restaurante.IO.Api.csproj
```

The VS Code launch profile points to:

```text
src/DevIO.Api/bin/Debug/netcoreapp3.1/Restaurante.IO.Api.dll
```

Current validation result: the run command shape was identified from the project and VS Code configuration, but startup was not verified because build output was unavailable.

## 8. Run Tests

The only test project identified is:

```text
test/Pedidos.Test/Pedidos.Test.csproj
```

From the repository root:

```powershell
dotnet test test/Pedidos.Test/Pedidos.Test.csproj
```

After a successful build, the no-build form is:

```powershell
dotnet test test/Pedidos.Test/Pedidos.Test.csproj --no-build
```

The validation command requested for this phase was also run from the repository root:

```powershell
dotnet test --no-build
```

Current validation result: `dotnet test --no-build` returned exit code `0` with no output. This is recorded as inconclusive because restore/build did not complete.

## 9. Create A Migration

The repository has two EF Core DbContexts:

- Domain/data context: `Restaurante.IO.Data.Context.MeuDbContext` in `src/DevIO.Data/Context/MeuDbContext.cs`
- Identity context: `Restaurante.IO.Api.DataContext.ApplicationDbContext` in `src/DevIO.Api/DataContext/ApplicationContext.cs`

Create a domain/data migration:

```powershell
dotnet ef migrations add <MigrationName> --project src/DevIO.Data/Restaurante.IO.Data.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context MeuDbContext --output-dir Migrations
```

Create an Identity/API migration:

```powershell
dotnet ef migrations add <MigrationName> --project src/DevIO.Api/Restaurante.IO.Api.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context ApplicationDbContext --output-dir Migrations
```

These commands are documented from the current project layout. They were not executed in this phase because creating migrations would alter migration files.

## 10. Apply A Migration

Apply domain/data migrations:

```powershell
dotnet ef database update --project src/DevIO.Data/Restaurante.IO.Data.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context MeuDbContext
```

Apply Identity/API migrations:

```powershell
dotnet ef database update --project src/DevIO.Api/Restaurante.IO.Api.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context ApplicationDbContext
```

Confirmed migration folders:

- `src/DevIO.Data/Migrations/`
- `src/DevIO.Api/Migrations/`

Confirmed migration files:

- `src/DevIO.Data/Migrations/20200817223231_InitialCreate.cs`
- `src/DevIO.Data/Migrations/MeuDbContextModelSnapshot.cs`
- `src/DevIO.Api/Migrations/20200817223121_InitialCreate.cs`
- `src/DevIO.Api/Migrations/ApplicationDbContextModelSnapshot.cs`

The EF migration commands were not successfully verified in this environment because restore/build validation is currently blocked and `dotnet-ef` availability was not confirmed.

## 11. Data Seed

No seed process was identified.

Repository search did not identify `Seed`, `HasData`, `EnsureCreated`, automatic `Migrate()`, a database initializer, or SQL `INSERT` statements. The file `sql/restaurante.sql` exists, but it is a structural database script and no inserted seed data was identified in it.

Therefore, there is no documented seed command for this legacy version.

## 12. Known Limitations

- .NET Core 3.1 is out of support.
- No `global.json` exists, although the historical README tells users to check it.
- The current machine used for Phase 1 validation has .NET 8 and .NET 10 SDKs/runtimes, but no .NET Core 3.1 SDK/runtime.
- `dotnet restore` is blocked by invalid local NuGet metadata for `microsoft.netcore.targets/1.1.0`.
- `dotnet build --no-restore` is blocked because restore did not produce required asset files.
- `dotnet test --no-build` is inconclusive in this environment.
- API startup was not verified because build output was unavailable.
- No `launchSettings.json` file was identified, and runtime ports were not fully verified.
- The health check UI references `https://localhost:44340/hc`, but no successful API run confirmed that port.
- `sql/restaurante.sql` creates database `restaurante`, while `ConnectionStrings:DefaultConnection` points to catalog `PedidosApi`.
- Redis Dockerfile exposes `6379`, while the API configuration points to `localhost:7001`.
- SQL Server, Redis, and Seq configuration values are local and environment-specific.
- No seed command or seed implementation was identified.

## 13. Troubleshooting

- If restore fails with invalid `.nupkg.metadata`, inspect or repair the local NuGet cache outside this repository. Do not change project files as part of legacy preservation.
- If build fails with missing `project.assets.json`, run restore first in an environment compatible with .NET Core 3.1.
- If `dotnet ef` commands fail, confirm a compatible EF Core CLI tool is installed and that the SQL Server connection string points to an available database server.
- If the API starts but health checks are unhealthy, confirm SQL Server, Redis, and Seq are reachable at the configured endpoints.
- If Redis is started from the provided Dockerfile, align the exposed host port with `RedisCacheSettings:ConnectionString`.
- If the SQL script is used instead of EF migrations, note that it creates database `restaurante`; align the application connection string before running the API.

## 14. Security Notice

This legacy version contains runtime secrets and local credentials in configuration files and Dockerfiles. They are preserved to document the historical state, not because they are safe defaults.

Do not reuse the JWT secret, SQL credentials, local log path, or other environment-specific values in new work. Do not deploy this legacy application without replacing secrets, reviewing dependencies, and addressing the unsupported runtime.

## 15. References For Future .NET 10 Version

Future modernization work should use this document as a historical baseline and should verify behavior before changing runtime, dependencies, architecture, migrations, or configuration.

The planned .NET 10 phase should specifically revisit:

- target frameworks and SDK selection;
- ASP.NET Core hosting and startup model;
- EF Core version and migration strategy;
- Identity and JWT configuration;
- Redis caching package choices;
- Seq/Serilog configuration;
- health checks;
- secrets management;
- local development orchestration for SQL Server, Redis, and Seq;
- the absence of a seed process.
