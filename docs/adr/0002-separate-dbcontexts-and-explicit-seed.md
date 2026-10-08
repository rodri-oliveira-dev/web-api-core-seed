# ADR 0002 — Separate DbContexts, migrations and explicit development seed

- **Status:** Accepted (implemented)
- **Context:** The sample uses Identity and restaurant persistence with historical EF schema constraints. Development bootstrap must be repeatable without inadvertently seeding production or replacing historical migrations.

## Decision

Keep `ApplicationDbContext` and its migrations in `Identity.Infrastructure`; keep `SampleRestaurantDbContext` and its migrations in `SampleRestaurant.Infrastructure`. The Compose one-shot `migrations` service applies both. The `--seed` application command explicitly calls `MigrateAsync` and upserts deterministic Identity/restaurant records; it is blocked in Production and never runs on normal API startup. Secrets for this operation are injected from local User Secrets or environment variables.

## Consequences

- Each module owns its schema and migration review, though local development can use one SQL Server.
- Container startup waits for required schema initialization.
- Rerunning seed should be idempotent; integration regression tests verify this.
- Production data migration and deployment are separate procedures. Do not use `EnsureCreated` or assume seed is a production migration.

## Evidence

- [EF Core command reference](../development/ef-core-migrations.md)
- `src/WebApiCoreSeed.Api/DevelopmentSeed/DevelopmentSeedRunner.cs`
- `compose.yaml` and `scripts/docker/apply-migrations.sh`