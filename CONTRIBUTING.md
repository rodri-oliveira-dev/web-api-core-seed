# Contributing

[English README](README.md) | [Leia-me em português](README.pt-BR.md) | [Code of Conduct](CODE_OF_CONDUCT.md)

Thanks for your interest in improving Web API Core Seed. Contributions should keep the **maintained .NET 10 application** reproducible and avoid changing the historical `v1.0.0-legacy` tag or `legacy/netcoreapp3.1` archival code.

## Before starting

1. Search existing issues and pull requests before beginning work. For non-trivial changes, open or reference an issue describing scope, constraints and expected behavior.
2. Read [architecture and dependency boundaries](docs/architecture.md), [ADR index](docs/adr/README.md), [environment setup](README.md#quick-start-with-docker-compose) and [quality gates](docs/quality-gates.md).
3. Never include passwords, connection strings with credentials, personal data or tokens in commits/logs. Follow [SECURITY.md](SECURITY.md) for vulnerabilities, not a public issue.
4. Prefer small, self-contained pull requests. Fixes should come with regression tests where applicable.

## Development workflow

From the repository root, with the SDK in `global.json`:

```bash
git checkout main
git pull --ff-only
git switch -c feat/short-purpose
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build
```

Integration tests require Docker for Testcontainers. Follow [local setup](docs/development/containerized-local-development.md) and [EF Core guidance](docs/development/ef-core-migrations.md) when database changes are involved. Preserve generated OpenAPI contracts and test their compatibility.

Submit a PR to `main` with a clear description, associated issue (e.g. `Closes #123` when the PR fully addresses it), impact/rollback notes, and test evidence. Do not assume a PR is merged because checks passed.

## Definition of Ready (DoR)

- Scope and expected behavior are clear; affected modules and contracts are identified.
- Security, data and backward compatibility impacts are considered.
- Test strategy and validation criteria are recorded.
- Any migration, environment or deployment prerequisites are described.

## Definition of Done (DoD)

- Implementation matches the issue without unrelated refactoring.
- Relevant unit/integration/contract and regression tests pass.
- Build, CodeQL, dependency review and applicable SonarCloud checks are green.
- Documentation, OpenAPI and migration guidance are updated for user-visible changes.
- PR review feedback has been addressed and resolved; no secrets introduced.
- Release implications are recorded in [CHANGELOG.md](CHANGELOG.md) when relevant.

## Design rules

- Keep the SampleRestaurant domain/application assembly independent of EF Core, ASP.NET Core and infrastructure implementations.
- Use explicit persistence ports and the scoped Unit of Work; avoid introducing a generic repository abstraction.
- Put schema changes in the owning Infrastructure project; preserve historical table names and migration compatibility.
- Propagate `CancellationToken` through asynchronous I/O.
- Preserve the distinction between anonymous and authenticated responses and avoid leaking tokens or personal information through logs or cache.
- Keep `README.md` and `README.pt-BR.md` aligned in topic/order whenever onboarding behavior changes; update other localized guides as necessary.
- Treat the code and generated contracts as source of truth rather than old Phase 1/2 planning documents.

## Support and behavior

For usage help, open a public issue with sanitized diagnostics; see [Code of Conduct](CODE_OF_CONDUCT.md). For potential vulnerabilities, follow [private reporting instructions](SECURITY.md). Project maintainers may request changes before accepting a contribution.