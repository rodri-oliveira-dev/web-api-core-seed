# ADR 0001 — Explicit ports and SampleRestaurant module boundaries

- **Status:** Accepted (implemented)
- **Context:** The legacy application coupled business and persistence code through generic repositories. The maintained .NET 10 sample should illustrate the dependencies and contracts of a small hexagonal architecture without claiming independent services.

## Decision

Keep domain models, inbound service ports, outbound repository ports and application use-case services in `WebApiCoreSeed.SampleRestaurant`. Keep EF Core repository implementations, mappings, migrations and Unit of Work in `WebApiCoreSeed.SampleRestaurant.Infrastructure`. The HTTP API is the composition root and wires dependencies in `DependencyInjectionConfig.cs`. Repository contracts are explicit (e.g. `IPratoRepository`), not a generic `IRepository<T>`.

## Consequences

- Core application logic can be tested without requiring Entity Framework Core.
- Queries and transactions have explicit ownership; pagination order and query shape can be reviewed.
- Interfaces grow per use case and require deliberate maintenance.
- The Identity application flow and some HTTP view models remain in the API; the boundary is pragmatic, not a claim of fully clean, deployable modules.

## Evidence

- [Module architecture](../architecture.md)
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant` and corresponding `.Infrastructure` project
- `tests/WebApiCoreSeed.UnitTests/Arquitetura/ModularHexagonalArchitectureTest.cs`