# Requirements - Containerized Local Development

## Specification

Provide two official local development modes.

Mode 1: run the API on the host with `dotnet run`, while SQL Server and Redis can be started by Docker Compose. Host secrets come from `dotnet user-secrets`.

Mode 2: run the full local stack with Docker Compose. API, SQL Server, Redis and migrations run in containers. Container secrets come from `.env.local` or host environment variables. User Secrets are not copied or mounted into containers.

## Acceptance

- Root `Dockerfile` uses official .NET 10 images and a multi-stage build.
- Restore uses the root build context and respects Central Package Management.
- Final API image uses the ASP.NET runtime image, exposes internal port `8080`, and runs as non-root.
- Root `compose.yaml` defines `api`, `migrations`, `sqlserver` and `redis`.
- SQL Server and Redis have persistent named volumes and health checks.
- Migrations run before API startup through a one-shot service.
- No tracked file contains functional SQL passwords, JWT secrets, tokens or API keys.
- API host mode keeps the existing `UserSecretsId`.
- Documentation includes reproducible commands and troubleshooting.

## Out of Scope

Production deployment, Kubernetes, cloud secret managers, Aspire, reverse proxies, internal HTTPS certificates, OpenTelemetry Collector and registry publishing are outside this task.
