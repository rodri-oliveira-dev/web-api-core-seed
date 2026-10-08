# Web API Core Seed

[**English**](README.md) | [Português (Brasil)](README.pt-BR.md)

[![CI](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/ci.yml/badge.svg)](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/ci.yml)
[![CodeQL](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/codeql.yml/badge.svg)](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/codeql.yml)

> **Project status:** actively maintained sample application targeting **.NET 10**. This repository is **not yet distributed as an installable `dotnet new` template**; that work is tracked in [#22](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/22). The historical .NET Core 3.1 version is unsupported; see [Legacy](#legacy-version).

## Overview

Web API Core Seed is a reference ASP.NET Core REST API demonstrating authentication, a modular sample restaurant domain, explicit persistence ports, SQL Server, optional Redis response caching, OpenTelemetry and automated quality gates. It is a **working application** and learning/reference seed, not a production-ready deployment blueprint.

The active solution is `WebApiCoreSeed.slnx`. The application entry point is `src/WebApiCoreSeed.Api/Program.cs`. The sample domain is intentionally separated from the reusable HTTP/hosting concerns; consult [architecture and request flow](docs/architecture.md).

## Requirements

- [.NET SDK **10.0.401**](global.json) for host execution, matching `global.json`.
- Docker Engine with Compose v2 for SQL Server, Redis, the full stack and integration tests (Testcontainers).
- Git and a shell (Bash or PowerShell); `curl` is optional for smoke tests.
- A strong local SQL Server password, a JWT signing secret and a development seed password. **Never commit them**.
- Optional: matching EF Core 10 `dotnet-ef` CLI for manual schema changes.

No Visual Studio 2019 or .NET Core 3.1 installation is required for the maintained solution.

## Quick start with Docker Compose

From the repository root:

```bash
git clone https://github.com/rodri-oliveira-dev/web-api-core-seed.git
cd web-api-core-seed
cp .env.local.example .env.local
```

Edit `.env.local` and replace **all three** credential placeholders (`SQLSERVER_SA_PASSWORD`, `JWT_SECRET` and `DEVELOPMENT_SEED_PASSWORD`). Use strong values. The file is ignored by Git.

```bash
docker compose --env-file .env.local config
docker compose --env-file .env.local up --build -d
docker compose --env-file .env.local ps
docker compose --env-file .env.local --profile tools up seed
curl -i http://localhost:8080/health/live
```

The first `up` starts SQL Server, Redis and the API; a one-shot `migrations` service prepares both EF Core schemas. The optional `seed` command populates development data and exits. On success, the API is available at `http://localhost:8080` (or the configured `API_HTTP_PORT`). See [containerized development](docs/development/containerized-local-development.md) for logs, modes and service behavior.

To stop while retaining database data:

```bash
docker compose --env-file .env.local down
```

**Caution:** `docker compose down --volumes` deletes local SQL Server and Redis volumes.

## Run the API on the host

You need the .NET SDK and a configured local SQL Server. Start dependencies only:

```bash
docker compose --env-file .env.local up -d sqlserver redis
dotnet restore WebApiCoreSeed.slnx
```

Configure ASP.NET Core User Secrets using `./scripts/setup/configure-user-secrets.sh` (Bash) or `./scripts/setup/configure-user-secrets.ps1` (PowerShell). The script prompts for `ConnectionStrings:DefaultConnection`, `AppSettings:Secret` and `DevelopmentSeed:User:Password` without storing them in Git. For the host database connection, use `localhost,1433` rather than Compose's `sqlserver` DNS name; for Redis use `localhost:7001`.

For Bash, launch explicitly in Development:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

In PowerShell, set `$env:ASPNETCORE_ENVIRONMENT = "Development"` before running the same `dotnet run` command. The port can be set with `ASPNETCORE_URLS`; host `dotnet run` does **not** automatically load `.env.local`. Alternatively, run the API entirely via Compose as shown above.

## Run tests

```bash
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build
```

Integration tests start isolated SQL Server and Redis containers using Testcontainers; Docker must be running. The CI also verifies OpenAPI contracts, dependency vulnerabilities, SonarCloud (trusted contexts) and CodeQL. See [quality gates](docs/quality-gates.md).

## Apply and create migrations

For first-time local setup, Compose applies **Identity and SampleRestaurant** migrations automatically before the API starts. To run that service explicitly:

```bash
docker compose --env-file .env.local up migrations
```

For manual EF Core changes, install the compatible `dotnet-ef` 10 CLI. **The design-time factories do not load the API's User Secrets:** supply the local SQL connection through the temporary `ConnectionStrings__DefaultConnection` environment variable in the same terminal as `dotnet ef`. Each DbContext has its own Infrastructure project. See the [EF Core migrations guide](docs/development/ef-core-migrations.md) for secure Bash/PowerShell setup and full commands. Never run development migration or seed commands against production databases without a reviewed deployment plan.

## Seed sample data

The seed is **explicit, idempotent and prohibited in Production**. After creating `.env.local` and starting the stack:

```bash
docker compose --env-file .env.local --profile tools up seed
```

Or, after configuring host User Secrets and the Development environment:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed
```

PowerShell: set `$env:ASPNETCORE_ENVIRONMENT = "Development"` before running `dotnet run ... -- --seed`. The command applies both migration sets and upserts a test Identity account (`DEVELOPMENT_SEED_EMAIL`, default `developer@example.local`) and sample restaurant records; reruns should not create duplicates. See [seed details](docs/development/containerized-local-development.md#development-seed).

## Authentication and API exploration

- `POST /api/v2/entrar`: anonymous sign-in with the seeded email/password, returning a JWT. The v1 sign-in exists but is marked deprecated.
- `GET /api/v1/Pratos`: public sample list with bounded pagination; other restaurant endpoints may require a Bearer JWT and claims.
- `/scalar`: interactive API reference; OpenAPI documents are also generated for supported API versions.
- `/health/live`, `/health/ready`: liveness/readiness checks; `/hc` is a compatibility endpoint.

Example, once `seed` completed:

```bash
curl -i http://localhost:8080/health/ready
curl -X POST http://localhost:8080/api/v2/entrar -H "Content-Type: application/json" -d '{"email":"developer@example.local","password":"<your local seed password>"}'
```

Do not copy tokens or passwords into issues or logs. See [API contracts](docs/openapi/) for generated OpenAPI JSON and [architecture](docs/architecture.md) for the request path.

## Architecture

The HTTP API is the composition root. `SampleRestaurant` holds domain models, application services and explicit ports; `SampleRestaurant.Infrastructure` supplies EF Core repositories and a Unit of Work. `Identity.Infrastructure` owns the Identity DbContext and migrations. The API owns controllers, authentication, authorization, rate limiting, error responses, caching and telemetry. The **generic repository described in the historical project is not part of the active SampleRestaurant design**.

See [architecture](docs/architecture.md), [ADRs](docs/adr/) and the [legacy migration guide](docs/migration-from-legacy.md).

## Configuration and observability

- `ConnectionStrings:DefaultConnection` is required for SQL Server.
- `AppSettings:Secret`, `Emissor` and `ValidoEm` configure JWT signing, issuer and audience.
- `RedisCacheSettings:Enabled` controls distributed response caching. Only eligible anonymous GET responses are cached; credentials, failure responses and `Set-Cookie` responses are excluded.
- `Cors:AllowedOrigins` uses explicit allowed origins (empty by default).
- `OpenTelemetry:Enabled` enables tracing/metrics; OTLP export is disabled until `OpenTelemetry:Otlp:Enabled` and an endpoint are configured.
- `SeqSettings:Enabled` enables optional Seq export; console logging uses Serilog.

Compose uses environment variables with double underscores, e.g. `AppSettings__Secret`. In host mode use User Secrets. For behavior and security constraints see [architecture](docs/architecture.md) and [containerized development](docs/development/containerized-local-development.md).

## Troubleshooting

| Symptom | What to check |
| --- | --- |
| Compose refuses to start | Replace the placeholders in `.env.local`, then rerun `docker compose --env-file .env.local config`. |
| SQL Server or migrations fail | Run `docker compose --env-file .env.local logs sqlserver migrations`; ensure password complexity and port availability. |
| Host app fails on configuration | Confirm User Secrets, `ASPNETCORE_ENVIRONMENT=Development` and the host SQL connection string. |
| Login fails | Run the seed, use the configured development credentials and check Identity password requirements. |
| Readiness unhealthy | Inspect SQL Server/Redis and `docker compose --env-file .env.local logs api`. |
| Integration tests fail | Start Docker Engine and verify Testcontainers can start SQL Server and Redis. |
| Port 8080/1433/7001 occupied | Change the corresponding port in `.env.local`. |

More details: [containerized development troubleshooting](docs/development/containerized-local-development.md#troubleshooting).

## Contributing and support

See [CONTRIBUTING.md](CONTRIBUTING.md), [Code of Conduct](CODE_OF_CONDUCT.md), [security reporting](SECURITY.md) and [CHANGELOG.md](CHANGELOG.md). For usage questions, open a GitHub Discussion if enabled, otherwise an issue. **Do not report exploitable vulnerabilities publicly.**

## Legacy version

The unmodified .NET Core 3.1 source is preserved at [`v1.0.0-legacy`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/v1.0.0-legacy); the [`legacy/netcoreapp3.1` branch](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/legacy/netcoreapp3.1) adds an unsupported-runtime warning. .NET Core 3.1 went out of support on December 13, 2022. For behavioral and database considerations see [LEGACY.md](LEGACY.md) and the [migration guide](docs/migration-from-legacy.md).

## Roadmap

The next steps are [`dotnet new` packaging (#22)](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/22) and the [v2.0.0 release (#23)](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23). **Neither distribution nor release is claimed to be completed here.**

## Learning resources

For contributors learning .NET from another ecosystem: [ASP.NET Core fundamentals](https://learn.microsoft.com/aspnet/core/fundamentals/), [EF Core documentation](https://learn.microsoft.com/ef/core/), [Integration tests in ASP.NET Core](https://learn.microsoft.com/aspnet/core/test/integration-tests), and [Docker Compose](https://docs.docker.com/compose/).