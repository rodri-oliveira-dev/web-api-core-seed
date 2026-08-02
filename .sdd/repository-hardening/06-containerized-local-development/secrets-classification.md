# Secrets Classification

## Required Secrets

- SQL Server SA password.
- JWT signing secret.
- Any connection string that embeds a password.

## Non-secret Configuration

- SQL host, port and database name.
- Redis host and port when Redis has no password.
- JWT issuer and audience.
- Feature flags such as Redis, Seq and OpenTelemetry enabled states.
- Local URLs without credentials.
- Timeouts, page sizes and service names.

## Source by Mode

| Configuration | Host Development | Docker Compose | Future Production |
| --- | --- | --- | --- |
| SQL password | User Secrets | `.env.local` or host environment | Secret manager |
| JWT secret | User Secrets | `.env.local` or host environment | Secret manager |
| SQL host | User Secrets connection string | service DNS `sqlserver` | environment configuration |
| Redis host | `appsettings.json` as `localhost:7001` | service DNS `redis` | environment configuration |
| SQL database | User Secrets connection string | `.env.local` optional value | environment configuration |
| Issuer/audience | tracked non-secret config | Compose env values | environment configuration |

No real values are recorded here.
