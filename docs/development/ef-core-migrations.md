# EF Core migrations (maintained .NET 10 application)

[Back to README](../../README.md) | [Português (Brasil)](ef-core-migrations.pt-BR.md)

The modernized solution has **two independently migrated DbContexts**. Execute commands from the repository root. For a clean development environment, prefer the Compose migration service:

```bash
docker compose --env-file .env.local up migrations
```

The `migrations` service in `compose.yaml` applies `ApplicationDbContext` and then `SampleRestaurantDbContext`. Do not run migrations against production without deployment review and backup.

## Before running EF CLI commands

- Install the SDK pinned by `global.json` and the compatible `dotnet-ef` **10.x** tool: `dotnet tool install --global dotnet-ef --version "10.*"` (or update an existing tool within the 10.x channel).
- Prepare SQL Server, e.g. `docker compose --env-file .env.local up -d sqlserver`.
- Configure the API project User Secrets for `ConnectionStrings:DefaultConnection` and `AppSettings:Secret` using the setup script described in the README.
- Run in the Development environment with a trusted local database, not against production.

## Identity DbContext

Migrations live under `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/Migrations`:

```bash
dotnet ef migrations list --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --no-connect

dotnet ef migrations add AddIdentityChange --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --output-dir Migrations

dotnet ef database update --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext
```

## SampleRestaurant DbContext

Migrations live under `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/Migrations`:

```bash
dotnet ef migrations list --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --no-connect

dotnet ef migrations add AddRestaurantChange --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --output-dir Migrations

dotnet ef database update --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext
```

## Review before applying schema changes

Generate an idempotent SQL script for each context by replacing `database update` with `migrations script --idempotent --output <path>` and retaining the matching `--project`, `--startup-project` and `--context` arguments. Inspect it and obtain review before any non-local execution. Historical migration classes and table names such as `Loggin` are compatibility contracts and must not be casually renamed.

To check for model drift, use `dotnet ef migrations has-pending-model-changes` with the same context-specific arguments. To validate an upgrade from the preserved legacy database, run the integration suite with Docker/Testcontainers, including its legacy upgrade scenario. See [migration from legacy](../migration-from-legacy.md).

## Seed is a separate operation

The explicit `--seed` command invokes both `MigrateAsync` calls and then upserts sample data. See [README seed section](../../README.md#seed-sample-data). It refuses to run in Production. It is **not** an EF migration nor a production data migration. This prevents accidental data changes during ordinary API startup.