# Containerized Local Development

This document defines the local development container setup. It is for development only and is not a production deployment model.

## Prerequisites

- .NET SDK from `global.json`.
- Docker Engine.
- Docker Compose v2.
- A local `.env.local` file for Compose secrets.
- User Secrets configured for host `dotnet run`.

## Architecture

There are two supported modes.

Mode 1 runs the API on the host with `dotnet run`. SQL Server and Redis can run in Docker Compose, and secrets come from `dotnet user-secrets`.

Mode 2 runs the full stack in Docker Compose. The API, SQL Server, Redis and migrations run in containers. User Secrets are not copied or mounted into containers.

## Services

- `api`: ASP.NET Core API image built from the root `Dockerfile`.
- `migrations`: one-shot SDK image that applies EF Core migrations.
- `seed`: optional one-shot runtime image that applies migrations idempotently and runs the development seed.
- `sqlserver`: SQL Server development database.
- `redis`: local Redis cache.

## Ports

| Service | Host | Container | Purpose |
| --- | --- | --- | --- |
| API | `${API_HTTP_PORT:-8080}` | `8080` | HTTP API |
| SQL Server | `${SQLSERVER_HOST_PORT:-1433}` | `1433` | Host `dotnet run` and local tools |
| Redis | `${REDIS_HOST_PORT:-7001}` | `6379` | Host `dotnet run` and local tools |

## Volumes

- `sqlserver-data`: SQL Server data.
- `redis-data`: Redis append-only local data.

`docker compose down` preserves volumes. `docker compose down --volumes` deletes local data.

## User Secrets

The API project already has a `UserSecretsId`; do not generate a new one.

Run the setup script:

```bash
./scripts/setup/configure-user-secrets.sh
```

On Windows PowerShell:

```powershell
./scripts/setup/configure-user-secrets.ps1
```

Manual configuration is also supported:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<local connection string>" --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
dotnet user-secrets set "AppSettings:Secret" "<local JWT secret>" --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
dotnet user-secrets set "DevelopmentSeed:User:Password" "<local development password>" --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
dotnet user-secrets set "DevelopmentSeed:User:Email" "developer@example.local" --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

For host execution, use `localhost` for SQL Server and Redis in the configured values.

## Compose Environment

Create a local Compose environment file:

```bash
cp .env.local.example .env.local
```

Replace the placeholders locally. Do not commit `.env.local`.

Compose uses service DNS names inside the network: `sqlserver` and `redis`.

## Infrastructure Only

Start only SQL Server and Redis:

```bash
docker compose --env-file .env.local up -d sqlserver redis
```

Then run the API on the host:

```bash
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

## Full Stack

Validate, build and run:

```bash
docker compose --env-file .env.local config
docker compose --env-file .env.local build
docker compose --env-file .env.local up -d
docker compose --env-file .env.local ps
```

Follow API logs:

```bash
docker compose --env-file .env.local logs -f api
```

Stop without deleting data:

```bash
docker compose --env-file .env.local down
```

Reset all local data:

```bash
docker compose --env-file .env.local down --volumes
```

## Migrations

The `migrations` service waits for SQL Server to be healthy, applies `ApplicationDbContext` migrations, then applies `SampleRestaurantDbContext` migrations. The API depends on successful migration completion.

Run migrations explicitly:

```bash
docker compose --env-file .env.local up migrations
```

For host execution, the development seed command also applies migrations before inserting data:

```bash
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed
```

## Development Seed

The development seed is explicit and idempotent. It never runs during normal API startup and is blocked when the environment is `Production`.

Configure the local credential without committing it:

```bash
dotnet user-secrets set "DevelopmentSeed:User:Password" "<local development password>" --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

Optional email override:

```bash
dotnet user-secrets set "DevelopmentSeed:User:Email" "developer@example.local" --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

Start infrastructure for host mode:

```bash
docker compose --env-file .env.local up -d sqlserver redis
```

Apply migrations and run the seed:

```bash
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed
```

Run it again with the same command. The second execution updates only known seed records that drifted and does not duplicate users, claims, pratos, mesas, pedidos or itens.

The seed creates:

- Identity user `developer@example.local` by default, with the password supplied locally.
- Claims for representative protected endpoints such as `Mesas` and `Pratos`.
- Four pratos, three mesas, one atendente, one pedido and two pedido items with deterministic IDs.

Authenticate with the seeded user:

```bash
curl -X POST "http://localhost:8080/api/v1/entrar" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"developer@example.local\",\"password\":\"<local development password>\"}"
```

Use the returned `accessToken` for protected endpoints:

```bash
curl "http://localhost:8080/api/v1/Mesas/22000000-0000-0000-0000-000000000001" \
  -H "Authorization: Bearer <accessToken>"
```

Run the seed in Docker Compose with the optional tools profile:

```bash
docker compose --env-file .env.local --profile tools up seed
```

Remove local data by deleting Compose volumes:

```bash
docker compose --env-file .env.local down --volumes
```

For a host-managed SQL Server database, remove local data by dropping the local database or by pointing `ConnectionStrings:DefaultConnection` to a new disposable database. Do not run the seed against production data.

## Health Checks

- Liveness: `http://localhost:8080/health/live`.
- Readiness: `http://localhost:8080/health/ready`.
- Legacy health endpoint: `http://localhost:8080/hc`.

SQL Server and Redis have Compose health checks. The API image does not install `curl` or `wget`; smoke HTTP checks are external.

## Troubleshooting

- Docker unavailable: validate .NET restore/build locally and rerun Docker commands when the engine is running.
- Port occupied: change `API_HTTP_PORT`, `SQLSERVER_HOST_PORT` or `REDIS_HOST_PORT` in `.env.local`.
- Invalid SQL Server password: use a strong local password accepted by SQL Server complexity rules.
- SQL Server still starting: wait for `sqlserver` health to become healthy before inspecting API logs.
- Migration failure: inspect `docker compose --env-file .env.local logs migrations`.
- Redis unavailable: inspect `docker compose --env-file .env.local logs redis`.
- API starts before dependencies: use `docker compose --env-file .env.local up -d`; dependencies are declared with health conditions.
- Old schema in volume: use `docker compose --env-file .env.local down --volumes` after confirming local data can be deleted.
- CPU architecture: .NET images support amd64, arm and arm64; SQL Server local development is validated on linux/amd64.
- Certificate error: local SQL connections use `TrustServerCertificate=True`.
- Non-root permission error: the container disables file logging with `SeqSettings__FilePath=""` in Compose.
- Missing secrets: Compose fails if `SQLSERVER_SA_PASSWORD` or `JWT_SECRET` is absent.
- Missing seed password: host seed fails if `DevelopmentSeed:User:Password` is absent; Compose seed fails if `DEVELOPMENT_SEED_PASSWORD` is absent.
- `localhost` vs Compose DNS: host mode uses `localhost`; containers use `sqlserver` and `redis`.

## Security

No local secret file is mounted into the API container. User Secrets are only for host development. Compose secrets come from `.env.local` or host environment variables. Production should use environment-specific configuration and a secret manager, not this Compose file.

The seed does not store a password in source, does not log the configured password, does not emit JWTs, does not relax Identity password policy and does not use `EnsureCreated`. It uses EF Core migrations and `UserManager<IdentityUser>`. Identity and SampleRestaurant are persisted through separate DbContexts; there is no distributed Unit of Work across them.
