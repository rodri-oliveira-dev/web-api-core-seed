# WebApiCoreSeed

Generated from the [Web API Core Seed](https://github.com/rodri-oliveira-dev/web-api-core-seed) **.NET 10** template (`dotnet new webapi-seed`).

This is a starting point, **not** a production-ready application. Review the generated code, secrets, database permissions and security settings before any deployment.

## Prerequisites

- .NET SDK version in `global.json`
- Docker Engine and Docker Compose v2 (SQL Server and Redis)
- Strong local SQL Server password, signing key and seed password

## Get started

```bash
cp .env.local.example .env.local
# Replace the SQLSERVER_SA_PASSWORD, JWT_SECRET and DEVELOPMENT_SEED_PASSWORD placeholders.
docker compose --env-file .env.local up --build -d
docker compose --env-file .env.local --profile tools up seed
curl -i http://localhost:8080/health/live
```

To stop without deleting local data:

```bash
docker compose --env-file .env.local down
```

To run the API on your host, first start the SQL Server/Redis services and configure User Secrets with `scripts/setup/configure-user-secrets.sh` (Bash) or `scripts/setup/configure-user-secrets.ps1` (PowerShell). Set `ASPNETCORE_ENVIRONMENT=Development` and run:

```bash
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

## Test

```bash
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build
```

Integration tests require Docker and start isolated SQL Server/Redis containers with Testcontainers.

## Development data and migrations

The Compose `migrations` service automatically applies Identity and SampleRestaurant EF Core migrations. Run explicitly with `docker compose --env-file .env.local up migrations`. The `--seed` command applies migrations and upserts deterministic sample data but is blocked in Production. For EF CLI commands, supply `ConnectionStrings__DefaultConnection` in the terminal: design-time factories do not read the API User Secrets.

## Settings

- `AppSettings__Secret`: required JWT signing key
- `ConnectionStrings__DefaultConnection`: required SQL Server connection
- `RedisCacheSettings__Enabled`: optional shared response cache
- `OpenTelemetry__Enabled`: tracing and metrics, with opt-in OTLP export
- `SeqSettings__Enabled`: optional Seq logging

See `src/WebApiCoreSeed.Api/appsettings.json` and `compose.yaml`. The API includes `/health/live`, `/health/ready`, `/scalar` and versioned `/api/v*/` endpoints. The deterministic development user is configured through `.env.local` for the seed service.
