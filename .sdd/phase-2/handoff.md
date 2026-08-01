# Handoff - Phase 2 Task 04

## Branch

- Current branch: `phase/2-dotnet-10-migration`
- Prompt 01 commit: `b8593c5 build: migrate solution to .NET 10`
- Prompt 02 commit: `24f701d refactor: adopt modern ASP.NET Core hosting`
- Prompt 03 commit: `e56d29a refactor: standardize API errors with problem details`
- Prompt 04 commit: pending until delivery.

## Current Runtime

- SDK pinned by `global.json`: `10.0.302`
- Active target framework: `net10.0` in API, Business, Data and test projects.
- API hosting model: modern `WebApplication`.
- Error contract: ASP.NET Core Problem Details.
- Rate limiting: native ASP.NET Core middleware.

## Native Rate Limiting

- `HostingConfig` registers `AddNativeRateLimiting`.
- `ApiConfig` calls `UseRateLimiter` after routing, cookie policy and authentication, before authorization.
- Policies:
  - `public`: 3 requests / 1 second, no queue.
  - `authenticated`: 3 requests / 1 second, no queue.
  - `authentication-sensitive`: 2 requests / 1 second, no queue.
- Endpoint metadata:
  - V1/V2 auth controllers: `authentication-sensitive`.
  - V1 `PratosController`: `authenticated`.
  - V1 `PratosController.ObterLista`: `public`.
  - V1 `MesasController`: `authenticated`.
- `/hc` and `/swagger/*` remain exempt.

## Partitioning

- Authenticated requests use validated user identity claims in this order:
  - `ClaimTypes.NameIdentifier`;
  - `sub`;
  - `ClaimTypes.Email`.
- Anonymous requests use a hashed composite of optional `X-ClientId` and direct connection remote address.
- Partition inputs are hashed and not logged.
- Forwarded headers are not trusted until explicit `ForwardedHeadersOptions` with known proxies/networks exists.

## Rejection Contract

`429` responses use:

```text
Content-Type: application/problem+json
type: urn:problem:rate-limit
title: Limite de requisicoes excedido.
traceId: present
Retry-After: present when native metadata is available
```

## Removed

- `AspNetCoreRateLimit` package reference.
- `IpRateLimiting` settings.
- Legacy rate-limit stores, processing strategy and `UseIpRateLimiting`.

## Validation State

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed, 30 existing analyzer warnings, 0 errors.
- `dotnet test --configuration Release --no-build`: passed, 32 tests.
- `dotnet list package`: passed; `AspNetCoreRateLimit` absent.
- Active-code searches for `AspNetCoreRateLimit`, `IpRateLimit` and `ClientRateLimit`: no findings.
- HTTP smoke/regression through `WebApplicationFactory`: passed, 11 focused tests.
- Process-based local smoke was attempted but blocked by local shell policy before API startup.

## Known Risks

- Anonymous partitions can still group callers behind NAT or an unconfigured reverse proxy.
- Full real `/hc` validation remains dependent on external SQL Server availability.
- Swagger and legacy API Versioning packages are still pending modernization.

## Next Objective

```text
#8 - OpenAPI and API versioning
```
