# Requirements - 04 Native Rate Limiting

## Objective

Replace the temporary `AspNetCoreRateLimit` bridge with ASP.NET Core native rate limiting for the active .NET 10 API.

Related issue:

```text
#7 - [Phase 2] Replace AspNetCoreRateLimit with native rate limiting
```

## Functional Requirements

- Remove the legacy `AspNetCoreRateLimit` package from the active API project.
- Remove legacy `IpRateLimiting` settings, memory stores, processing strategy and middleware.
- Register native `AddRateLimiter`.
- Place `UseRateLimiter` after routing and authentication so endpoint metadata and authenticated users are available.
- Define explicit policies instead of applying one implicit limit to every endpoint.
- Return deterministic `429 Too Many Requests` responses.
- Write rate-limit rejections through the API Problem Details contract.
- Emit `Retry-After` when the limiter provides retry metadata.
- Keep health checks exempt from API rate limiting.

## Security Requirements

- Prefer authenticated user identity when a request has a validated JWT.
- Do not trust `X-Forwarded-For`, `X-Real-IP` or other forwarded IP headers without explicit proxy trust configuration.
- Do not log raw partition keys, client IDs, user IDs or IP addresses.
- For anonymous requests, use the direct connection remote address only as a fallback signal and hash the composite partition key.

## Test Requirements

- Cover allowed requests below the limit.
- Cover requests rejected above the limit.
- Assert status `429`.
- Assert `application/problem+json`.
- Assert Problem Details type and `traceId`.
- Assert `Retry-After` on rejection.
- Cover exempt health endpoint behavior.
- Cover independent partitions for authenticated users and anonymous client identifiers.

## Out Of Scope

- Full OpenTelemetry integration.
- Distributed rate-limit stores.
- New proxy trust model.
- API Versioning or Swagger modernization.
- Testcontainers or SQL Server orchestration.
