# Handoff - Phase 1 Prompt 02

## Completed

- Confirmed required preconditions before edits.
- Preserved the historical README content and added a visible .NET Core 3.1 legacy notice at the top.
- Created `LEGACY.md` with dedicated runtime, usage, migrations, seed, limitations, troubleshooting, security, and .NET 10 planning notes.
- Updated SDD artifacts for Prompt 02.
- Kept the change documentation-only.
- Did not create the planned legacy tag or branch.
- Did not push.

## Documentation Created Or Updated

- `README.md`
- `LEGACY.md`
- `.sdd/phase-1/status.md`
- `.sdd/phase-1/decisions.md`
- `.sdd/phase-1/baseline.md`
- `.sdd/phase-1/handoff.md`

## Commands Documented

Restore:

```powershell
dotnet restore RestauranteAPI.sln
```

Build:

```powershell
dotnet build RestauranteAPI.sln --no-restore
```

Run API:

```powershell
dotnet run --project src/DevIO.Api/Restaurante.IO.Api.csproj
```

Run tests:

```powershell
dotnet test test/Pedidos.Test/Pedidos.Test.csproj
dotnet test test/Pedidos.Test/Pedidos.Test.csproj --no-build
```

Create domain/data migration:

```powershell
dotnet ef migrations add <MigrationName> --project src/DevIO.Data/Restaurante.IO.Data.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context MeuDbContext --output-dir Migrations
```

Create Identity/API migration:

```powershell
dotnet ef migrations add <MigrationName> --project src/DevIO.Api/Restaurante.IO.Api.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context ApplicationDbContext --output-dir Migrations
```

Apply domain/data migration:

```powershell
dotnet ef database update --project src/DevIO.Data/Restaurante.IO.Data.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context MeuDbContext
```

Apply Identity/API migration:

```powershell
dotnet ef database update --project src/DevIO.Api/Restaurante.IO.Api.csproj --startup-project src/DevIO.Api/Restaurante.IO.Api.csproj --context ApplicationDbContext
```

## Seed Behavior

No seed process was identified.

Searches did not find `Seed`, `HasData`, `EnsureCreated`, automatic `Migrate()`, database initializer patterns, or SQL `INSERT` statements in `sql/restaurante.sql`. `LEGACY.md` records that there is no documented seed command for the legacy version.

## Limitations Identified

- .NET Core 3.1 is out of support since December 13, 2022.
- No `global.json` exists even though the historical README references it.
- Current validation machine does not have .NET Core 3.1 SDK/runtime installed.
- Local NuGet metadata for `microsoft.netcore.targets/1.1.0` is invalid and blocks restore.
- Build cannot complete after restore failure because asset files are missing.
- Test validation is inconclusive because build output is unavailable.
- API startup was not verified because build output is unavailable.
- No `launchSettings.json` was identified.
- `HealthChecks-UI` references `https://localhost:44340/hc`, but the runtime port was not verified.
- `sql/restaurante.sql` creates database `restaurante`, while `ConnectionStrings:DefaultConnection` points to catalog `PedidosApi`.
- Redis Dockerfile exposes `6379`, while `RedisCacheSettings:ConnectionString` points to `localhost:7001`.
- SQL Server, Redis, and Seq values are local/environment-specific.
- Legacy configuration contains secrets and credentials that must not be reused for new work.

## Validation State

- `dotnet restore`: blocked by invalid local NuGet metadata and unsupported-target warning.
- `dotnet build --no-restore`: blocked because restore assets are missing.
- `dotnet test --no-build`: inconclusive; returned exit code `0` with no output.
- Path and local documentation link validation: passed.
- `git diff --check`: passed.
- Scope review: only allowed documentation files were changed.

## Prompt 03 Should Verify

- Current branch is still `phase/1-preserve-legacy`.
- Working tree is clean before creating Git references.
- Prompt 01 and Prompt 02 commits are present.
- `.sdd/phase-1/status.md` marks Prompt 01 and Prompt 02 as completed.
- `README.md` and `LEGACY.md` accurately describe the preserved legacy state.
- No functional files were altered during Prompt 02.
- Existing refs named `v1.0.0-legacy` or `legacy/netcoreapp3.1` do not already exist before creation.

## Git References Still Needed

Create only in Prompt 03:

- tag `v1.0.0-legacy`
- branch `legacy/netcoreapp3.1`

Do not push unless a later prompt explicitly changes the Phase 1 rule.
