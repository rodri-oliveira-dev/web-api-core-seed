# Handoff - Phase 2 Final

## Branches

- Current branch: `phase/2-dotnet-10-migration`
- Push: pendente
- PR da fase: pendente
- Next planned branch: `phase/3-quality-and-safety`

## Phase 2 Commits

- `b8593c5 build: migrate solution to .NET 10`
- `24f701d refactor: adopt modern ASP.NET Core hosting`
- `e56d29a refactor: standardize API errors with problem details`
- `e4be85c refactor: use native ASP.NET Core rate limiting`
- Prompt 05 delivery commit: `refactor: modernize OpenAPI and API versioning`

## Final Runtime State

- SDK pinned by `global.json`: `10.0.302`
- Active target framework: `net10.0`
- Active solution: `RestauranteAPI.sln`
- API project: `src/DevIO.Api/Restaurante.IO.Api.csproj`
- Hosting model: modern ASP.NET Core `WebApplication`
- Legacy `Startup` class: absent from active API code
- Architecture: legacy layered architecture preserved; Phase 4 modularization was not anticipated

## Hosting

- `Program.cs` creates the builder, configures Serilog, registers services and starts the app through `WebApplication`.
- `HostingConfig` owns service and middleware composition.
- Static startup configuration access was removed in earlier Phase 2 work.
- Controllers remain the active HTTP adapters; no Minimal API migration was performed.

## Error Contract

- API errors use ASP.NET Core Problem Details.
- Problem Details responses include `traceId`.
- Domain notifications are mapped to Problem Details.
- Legacy custom error middleware and legacy `ErrorController` were removed in prompt 03.

## Rate Limiting

- Active implementation: native ASP.NET Core rate limiting.
- Policies:
  - `public`
  - `authenticated`
  - `authentication-sensitive`
- Rejections return `429 application/problem+json` with `Retry-After` when native metadata is available.
- `/hc` and OpenAPI/Scalar surfaces remain outside API rate-limit policies.
- Forwarded IP headers are not trusted until explicit trusted proxy configuration exists.

## OpenAPI

- Active generator: `Microsoft.AspNetCore.OpenApi` with `Asp.Versioning.OpenApi`.
- Active UI: Scalar.
- Document routes:
  - `/openapi/v1.json`
  - `/openapi/v2.json`
- UI route:
  - `/scalar/`
- Generated contracts:
  - `docs/openapi/openapi-v1.json`
  - `docs/openapi/openapi-v2.json`
- Baseline Swagger contracts:
  - `docs/openapi/baseline/swagger-v1.json`
  - `docs/openapi/baseline/swagger-v2.json`
- Generation command:

```text
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
```

## Versioning

- Active package line:
  - `Asp.Versioning.Mvc` `10.0.1`
  - `Asp.Versioning.Mvc.ApiExplorer` `10.0.1`
  - `Asp.Versioning.OpenApi` `10.0.1`
- Route versioning remains URL-segment based.
- Active API versions:
  - `v1`
  - `v2`
- Existing controller routes were preserved.

## Authentication

- JWT bearer authentication remains active.
- OpenAPI documents expose a `Bearer` HTTP bearer JWT security scheme.
- Protected operations include operation-level security requirements.
- Scalar UI is configured to persist authentication input.

## Tests

- Current test project: `test/Pedidos.Test/Pedidos.Test.csproj`
- Current validation count: 34 tests.
- Covered surfaces include:
  - Problem Details validation and domain errors
  - `401` authentication challenge
  - `403` authorization failure
  - native rate-limit rejection `429`
  - OpenAPI V1/V2 documents
  - Scalar UI
  - public and protected endpoints
  - health check in isolated host

## Official Commands

```text
dotnet --info
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet list package
dotnet list package --deprecated
dotnet list package --vulnerable
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
git diff --check
```

## Contracts Changed

- Documentation route changed from `/swagger/{version}/swagger.json` to `/openapi/{version}.json`.
- Documentation UI changed from `/swagger` to `/scalar/`.
- OpenAPI version changed from `3.0.1` to `3.0.4`.
- JWT scheme changed from API-key-style bearer header documentation to HTTP bearer JWT.
- Problem Details media documentation was added for error responses.
- `429` is now documented for rate-limited operations.
- Runtime API paths were preserved.

## Debts and Risks

- `dotnet list package --deprecated` reports `xunit` 2.9.3 as deprecated in the test project; Phase 3 should evaluate xUnit v3 migration.
- Native OpenAPI output no longer documents `404` for two `PUT` operations even though runtime behavior is preserved; refine response metadata in a future contract-quality pass.
- Full real `/hc` validation depends on external SQL Server availability; isolated host tests keep `/hc` registered.
- HealthChecks UI web `/hc-ui` remains disabled because package line 9 is not runtime-compatible with EF Core 10 in this solution.
- Existing HealthChecks packages are still on major version 9 because no compatible major 10 line was available during Phase 2.
- Process-based smoke may be blocked by local shell policy; WebApplicationFactory smoke/regression is the reliable local path.

## Phase 3

Planned next branch:

```text
phase/3-quality-and-safety
```

Planned next issues:

- `#9`
- `#10`
- `#11`
- `#12`
- `#13`
