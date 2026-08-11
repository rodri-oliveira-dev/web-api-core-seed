# Threat Surface - 03 Security Hardening

## Endpoints

| Surface | Endpoint | Exposure | Data |
| --- | --- | --- | --- |
| Public catalog | `GET /api/v1/Pratos` | Anonymous, rate limited | Pagination query, cached response |
| Authentication | `POST /api/v1/entrar`, `POST /api/v2/entrar` | Anonymous, sensitive rate limit | Email, password, JWT response |
| Registration | `POST /api/v1/nova-conta` | Authenticated in v1 | Email, password, identity data |
| Authenticated API | `/api/v1/Mesas/*`, protected `Pratos` mutations | JWT and claim authorization | Domain identifiers and payloads |
| Documentation | `/openapi/{document}.json`, `/scalar/*` | Public documentation | OpenAPI metadata, auth UI client-side token entry |
| Health liveness | `/health/live`, legacy `/hc` | Public minimal | Aggregate process status only |
| Health readiness | `/health/ready` | Public minimal in production, detailed in Development/Testing | SQL Server, Redis and Seq status when configured |

## Data Received

- Headers: `Authorization`, cookies, `X-ClientId`, content negotiation and browser CORS headers.
- Query: pagination and possible accidental sensitive keys such as `access_token` or `password`.
- Payloads: login credentials, registration data and domain view models.
- Files: base64 photo upload on protected `Pratos` mutation.

## Data Registered

- Allowed: method, sanitized request path, status code, elapsed time, host, scheme, protocol, response content type, endpoint metadata and non-sensitive whitelisted headers on error.
- Not allowed: full query string, `Authorization`, cookies, API keys, access tokens, passwords, client secrets, raw payloads, connection strings and JWTs.
- Conditional: stack traces and exception messages may appear in internal logs, but production Problem Details responses stay generic.

## Proxies And Configurations

- `X-Forwarded-For` and `X-Forwarded-Proto` are untrusted unless `ForwardedHeaders:Enabled` is true and explicit `KnownProxies` or `KnownNetworks` are supplied.
- Production must supply real CORS origins and deployment-specific forwarded proxy settings when behind a reverse proxy.
- Local placeholders remain placeholders and must be overridden outside local development.
