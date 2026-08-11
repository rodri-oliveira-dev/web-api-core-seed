# Discovery - Containerized Local Development

## Repository State

- Branch: `phase/4-architecture-modernization`.
- Initial SHA: `66795661a594b855f9834ed1c9445d4729e0f9d7`.
- Active solution: `WebApiCoreSeed.slnx`.
- Active API project: `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`.
- API `UserSecretsId`: `c52dbe85-d94e-4cc2-9856-529f22712174`.
- Preexisting untracked files: `tmp/1.md`, `tmp/2.md`, `tmp/3.md`; ignored by explicit user instruction.

## Tooling

- .NET SDK: `10.0.302`.
- Docker Client: `29.1.4-rd`.
- Docker Engine: `29.1.3`.
- Docker Compose: `v5.0.1`.

## Containers

- Legacy Dockerfiles existed under `docker/`; they were not reused.
- `docker/SqlServer.dockerfile_` contained a fixed SA password and was removed.
- `docker/redis.dockerfile` used a floating `redis` tag and invalid health check and was removed.
- `docker/datalust-seq.dockerfile` used a floating `latest` tag and was removed.
- Integration tests use `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04` and `redis:7.4.2-alpine`.
- SQL Server image includes `/opt/mssql-tools18/bin/sqlcmd`.
- Redis image includes `/usr/local/bin/redis-cli`.
- `mcr.microsoft.com/dotnet/sdk:10.0` and `mcr.microsoft.com/dotnet/aspnet:10.0` are multi-arch manifest tags.

## Runtime Configuration

- `Program.cs` uses `WebApplication.CreateBuilder`.
- The API does not call `Database.MigrateAsync` on startup.
- `SampleRestaurantDbContext` and `ApplicationDbContext` both use `ConnectionStrings:DefaultConnection`.
- Health endpoints are `/health/live`, `/health/ready` and `/hc`.
- No `launchSettings.json` exists.
- `.vscode/tasks.json` and `.vscode/launch.json` exist.

## Migrations

- Identity context: `ApplicationDbContext`.
- Identity migrations assembly: `WebApiCoreSeed.Identity.Infrastructure`.
- Sample context: `SampleRestaurantDbContext`.
- Sample migrations assembly: `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- Startup project for EF commands: `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`.
- Design-time factories exist for both contexts and read JSON plus environment variables.

## Findings From Grep

- `appsettings.json` contained a local SQL Server connection string with a password placeholder and a JWT secret placeholder.
- Legacy SQL Server Dockerfile contained a fixed password.
- Test code contains generated or ephemeral test secrets only.
- Historical SDD and LEGACY files document old secret findings; they are historical records.
- `localhost`, `1433`, `7001` and `5341` appear in local development settings and documentation.
