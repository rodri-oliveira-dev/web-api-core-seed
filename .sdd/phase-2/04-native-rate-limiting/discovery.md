# Discovery - 04 Native Rate Limiting

## Required Commands

Initial repository state:

```text
git status --short
```

Result: clean.

```text
git branch --show-current
```

Result:

```text
phase/2-dotnet-10-migration
```

```text
git log -3 --oneline
```

Result:

```text
e56d29a refactor: standardize API errors with problem details
24f701d refactor: adopt modern ASP.NET Core hosting
b8593c5 build: migrate solution to .NET 10
```

```text
dotnet build --configuration Release
```

Result: passed, with existing analyzer warnings.

```text
dotnet test --configuration Release --no-build
```

Result: passed, 27 tests.

## Legacy Rate Limiting Search

```text
git grep -n "AspNetCoreRateLimit"
```

Active code findings before the change:

```text
src/DevIO.Api/Configuration/RateLimitConfig.cs:1:using AspNetCoreRateLimit;
src/DevIO.Api/Restaurante.IO.Api.csproj:35:<PackageReference Include="AspNetCoreRateLimit" Version="5.0.0" />
```

SDD and guidance files also documented that the package was temporary.

```text
git grep -n "ConfigureRateLimit"
```

Active code findings before the change:

```text
src/DevIO.Api/Configuration/HostingConfig.cs:101:services.ConfigureRateLimit(configuration);
src/DevIO.Api/Configuration/HostingConfig.cs:177:app.ConfigureRateLimit();
src/DevIO.Api/Configuration/RateLimitConfig.cs:10:public static IServiceCollection ConfigureRateLimit(...)
src/DevIO.Api/Configuration/RateLimitConfig.cs:24:public static IApplicationBuilder ConfigureRateLimit(...)
```

```text
git grep -n "IpRateLimit"
```

Active code findings before the change:

```text
src/DevIO.Api/Configuration/RateLimitConfig.cs:14:services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
src/DevIO.Api/Configuration/RateLimitConfig.cs:26:app.UseIpRateLimiting();
src/DevIO.Api/appsettings.json:30:"IpRateLimiting": { ... }
```

```text
git grep -n "ClientRateLimit"
```

Result: no findings.

```text
git grep -n "ForwardedHeaders"
```

Result: no findings.

```text
git grep -n "RemoteIpAddress"
```

Result: no findings before this prompt.

## Current Legacy Policy

The previous `IpRateLimiting` configuration applied to all `*:/api/*` endpoints:

| Period | Limit |
| --- | --- |
| `1s` | 3 |
| `1m` | 180 |
| `1h` | 10800 |

Other legacy options:

- `EnableEndpointRateLimiting`: `true`
- `StackBlockedRequests`: `true`
- `RealIPHeader`: `X-Real-IP`
- `ClientIdHeader`: `X-ClientId`
- `HttpStatusCode`: `429`

## Affected Endpoints

- V1 auth endpoints under `/api/v1`, including `/api/v1/entrar` and `/api/v1/nova-conta`.
- V2 auth endpoint `/api/v2/entrar`.
- V1 public list endpoint `GET /api/v1/Pratos`.
- V1 authenticated `Pratos` endpoints except the anonymous list action.
- V1 authenticated `Mesas` endpoints.

Non-API endpoints:

- `/hc` is not under `/api/*`; it was not covered by the legacy API rule and remains exempt.
- `/swagger/*` is not under `/api/*`; it remains exempt.

## Whitelist And Client IDs

- No explicit whitelist was configured.
- `X-ClientId` existed as a legacy header name, but no server-side client registry was identified.
- No API key mechanism was identified.

## Rejection Contract

Before this prompt, the actual 429 body was delegated to `AspNetCoreRateLimit`.

The Problem Details catalog from Prompt 03 already reserved:

```text
type: urn:problem:rate-limit
title: Limite de requisicoes excedido.
status: 429
```

## Proxy And NAT Risks

- The active application had no `ForwardedHeadersOptions` and no trusted proxy or network list.
- Trusting `X-Real-IP` from arbitrary clients would allow callers to spoof partitions.
- Partitioning anonymous users only by IP can punish unrelated users behind NAT.
- The native design therefore uses authenticated user identity first, and anonymous client ID plus direct connection remote address only as a fallback.
