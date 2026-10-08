# Migration guide: .NET Core 3.1 to the maintained .NET 10 application

[Back to README](../README.md) | [Historical baseline](../LEGACY.md)

> This is a migration **comparison and planning guide**, not an automatic code/database conversion tool. No production deployment or data migration is implied.

## Which version should I use?

| Reference | Purpose | Support |
| --- | --- | --- |
| [`v1.0.0-legacy`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/v1.0.0-legacy) | Untouched 2020-era baseline at commit `6ce03d7f` | .NET Core 3.1 — out of support |
| [`legacy/netcoreapp3.1`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/legacy/netcoreapp3.1) | Same legacy code plus unsupported-runtime README notice | Historical only |
| [`main`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/main) | Maintained .NET 10 sample | Active development |

.NET Core 3.1 reached end of support on **December 13, 2022**. Do not use the legacy runtime as a production deployment target.

## Breaking changes and migration map

| Area | Legacy | Current |
| --- | --- | --- |
| Solution | `RestauranteAPI.sln`; `src/DevIO.*` and `test/Pedidos.Test` | `WebApiCoreSeed.slnx`; `src/WebApiCoreSeed.Api`, `src/Modules/*`, `tests/*` |
| Host | ASP.NET Core 3.1 `Startup` / `Program` | .NET 10 `WebApplication`; `AddApiServices` / `UseApiPipeline` |
| API documentation | Swashbuckle / Swagger UI | ASP.NET Core OpenAPI + Scalar, versioned documents |
| Authentication | Legacy Identity/JWT integration | .NET 10 Identity/JWT; v2 authentication endpoint; v1 retained but deprecated |
| Rate limiting | `AspNetCoreRateLimit` | Native ASP.NET Core rate limiting policies |
| Errors | Legacy custom envelopes | Standardized `ProblemDetails` for error paths |
| Domain organization | Combined business/data layers, generic repositories | `SampleRestaurant` core with explicit ports, Infrastructure adapters and UoW |
| Persistence | Legacy EF Core 3.1 context locations | Identity and SampleRestaurant EF Core 10 contexts, separate Infrastructure migrations |
| Local setup | Manual and inconsistent legacy services | `compose.yaml`, secret setup scripts, explicit deterministic `--seed` |
| Observability | Serilog / optional Seq | Serilog, OpenTelemetry traces and metrics, optional OTLP/Seq |
| Testing/CI | Historical limited verification | Separate unit/integration suites, CodeQL, dependency review, quality gates |

## Migration checklist

1. **Inventory contracts.** Capture existing HTTP routes, methods, JWT issuer/audience, response shapes and clients. Compare against `docs/openapi/openapi-v1.json` and `openapi-v2.json` before switching consumers; the authentication contract may differ.
2. **Protect data.** Back up the legacy SQL Server database and inventory custom migrations. Do not rename legacy tables/columns solely for terminology improvements.
3. **Choose a clean checkout.** Use the supported .NET 10 SDK from `global.json`. Configure environment variables/User Secrets; historical `DevIO` paths and launch scripts are not valid in the new tree.
4. **Review schema evolution.** Identity and SampleRestaurant migrations are now in their respective Infrastructure projects. Review generated SQL and integration upgrade tests before any live upgrade; see [migration commands](development/ef-core-migrations.md).
5. **Validate security.** Revisit CORS origins, secrets, claims, JWT issuer/audience, native rate limits, caching and proxy forwarding. Do not silently carry legacy permissive configuration forward.
6. **Compare behavior.** Run unit tests, Testcontainers integration tests and OpenAPI generation. Smoke-test sign-in, representative protected endpoints, pagination and error responses against expected contracts.
7. **Plan deployment separately.** The provided Compose setup is for development only. Perform an independent rollout, rollback, secret rotation and observability review for each real environment.

## Compatibility caveats

- Legacy migration filenames remain useful history. The current solution intentionally preserves compatible SQL naming where required.
- The seed creates local demonstration records; it is **not** a migration of historical customer data.
- The two contexts can use one SQL Server database locally, but their migrations and responsibilities remain separate.
- The future `dotnet new` template distribution [#22](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/22) and release [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23) are not complete as part of the documentation work.
- Legacy restore/build/test outcomes are documented in [LEGACY.md](../LEGACY.md); do not interpret absence of a failure report as verified compatibility.

## Sources of truth

- [Current architecture](architecture.md)
- [Original legacy analysis](../LEGACY.md)
- [Infrastructure migration strategy](../.sdd/phase-4/06-infrastructure-migrations/migration-inventory.md)
- [Quality checks](quality-gates.md)
- [Development setup](development/containerized-local-development.md)