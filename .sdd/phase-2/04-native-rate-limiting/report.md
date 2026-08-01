# Report - 04 Native Rate Limiting

## Changes

- Removed `AspNetCoreRateLimit` from the active API project.
- Replaced legacy `IpRateLimiting` settings with `NativeRateLimitingSettings`.
- Replaced legacy memory stores and `UseIpRateLimiting` middleware with ASP.NET Core native `AddRateLimiter` and `UseRateLimiter`.
- Added explicit policies:
  - `public`
  - `authenticated`
  - `authentication-sensitive`
- Added endpoint metadata through `EnableRateLimiting`.
- Integrated `429` rejections with the existing Problem Details contract.
- Added `Retry-After` when supplied by the native limiter.
- Added safe rejection logging without raw partition data.
- Updated integration tests for allowed requests, blocked requests, Problem Details, `Retry-After`, health exemption and independent partitions.

## Removed

- `AspNetCoreRateLimit` package reference.
- `IpRateLimiting` appsettings section.
- `IpRateLimitOptions`, `IIpPolicyStore`, `IRateLimitCounterStore`, `IProcessingStrategy`, `IRateLimitConfiguration` registrations.
- `UseIpRateLimiting` middleware.

## Policies

| Policy | Limit | Applied To |
| --- | --- | --- |
| `public` | 3 requests / 1 second | `GET /api/v1/Pratos` |
| `authenticated` | 3 requests / 1 second | V1 `Pratos` and `Mesas` protected endpoints |
| `authentication-sensitive` | 2 requests / 1 second | V1/V2 auth controllers |

## Partition Keys

- Authenticated: validated user claim, hashed.
- Anonymous: hashed composite of optional `X-ClientId` and direct connection remote address.
- No raw user ID, client ID or IP address is logged.

## Proxy Behavior

Forwarded client IP headers are not trusted. The repository has no configured trusted proxy or known network list. Behind a proxy, anonymous fallback partitioning may group callers by the proxy connection address until `ForwardedHeadersOptions` is explicitly configured.

## Rejection Contract

Rate-limit rejections return:

```text
HTTP 429
Content-Type: application/problem+json
Retry-After: <seconds>
```

Problem Details:

```text
type: urn:problem:rate-limit
title: Limite de requisicoes excedido.
status: 429
traceId: present
```

## Validation

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed, 30 existing analyzer warnings, 0 errors.
- `dotnet test --configuration Release --no-build`: passed, 32 tests.
- `dotnet list package`: passed; `AspNetCoreRateLimit` absent.
- Active-code searches for `AspNetCoreRateLimit`, `IpRateLimit` and `ClientRateLimit`: no findings.
- HTTP smoke/regression through `WebApplicationFactory`: passed, 11 focused tests.

Process-based local smoke was attempted but blocked by local shell policy before startup. The integration host smoke covers the API pipeline while isolating SQL Server and Redis.

## Next Step

Run Prompt 5 for issue `#8`, OpenAPI and API versioning modernization.
