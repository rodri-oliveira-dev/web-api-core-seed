# Changelog

This changelog documents the maintained .NET 10 application. See [v2.0.1 release notes](docs/releases/v2.0.1.md) and the [publication checklist](docs/releases/release-checklist.md).

## [Unreleased]

Future changes are tracked here.

## [2.0.1]

### Security and maintenance

- Refresh the generated template's .NET SDK baseline to **10.0.403** and ASP.NET Core Docker runtime image to **10.0.14**, incorporating the October 2026 .NET out-of-band security servicing update.
- Publish a newly versioned `RodriOliveira.WebApiCoreSeed.Templates.2.0.1.nupkg` as a GitHub Release asset, without modifying the v2.0.0 release.
- Validate the installed template's generated `global.json` and `Dockerfile` for a security-serviced .NET baseline.
- Keep release checks restricted to the exact verified commit and published SHA-256 package checksum; no NuGet.org publication.

See [v2.0.1 release notes](docs/releases/v2.0.1.md). Existing application deployments still require updating/rebuilding their runtime environments.

## [2.0.0]

### Runtime and architecture — breaking changes

- Replace the historical .NET Core 3.1 application with a supported **.NET 10** sample using `WebApiCoreSeed.slnx` and SDK pinning via `global.json`.
- Replace generic legacy business/data patterns with explicit SampleRestaurant application/repository ports, infrastructure adapters and Unit of Work.
- Introduce separate EF Core migration ownership for Identity and SampleRestaurant; existing production schemas require deliberate upgrade assessment, **not** automatic conversion.
- Modernize API contracts with versioned OpenAPI and Scalar, native rate limiting, JWT/Identity integration and standardized Problem Details.
- Preserve legacy naming where necessary for database compatibility.

### Developer experience, security and observability

- Add Docker Compose SQL Server/Redis development stack, container migrations, non-production-only idempotent seed and User Secrets configuration scripts.
- Add security headers, safer public-response Redis caching, Serilog and OpenTelemetry with opt-in external telemetry.
- Add unit/integration tests, Testcontainers coverage, OpenAPI governance, CodeQL, dependency review and SonarCloud quality gates.
- Add English/Portuguese onboarding, architecture and migration guides, ADRs, contribution and security policies.
- Package `RodriOliveira.WebApiCoreSeed.Templates` version **2.0.0**, with `dotnet new webapi-seed`, source/namespace substitution, per-instance UserSecretsId and generated EN/PT-BR guides.
- Validate package content, install, generated `SampleApi`, dotted project names, build, unit/integration tests and live HTTP health checks.

### Delivery

- Add controlled release workflow that waits for successful CI, CodeQL and template smoke checks at the **exact released commit**.
- Publish the verified NuGet template file and SHA256 checksum as **GitHub Release assets**, not to NuGet.org.
- Record breaking changes and operator verification in `docs/releases/`.

### Known limitations

- Example is not a production deployment blueprint; SQL Server, Redis, secrets, custom auth and telemetry configurations must be reviewed.
- Generation-time flags for omitting application components are intentionally deferred.
- GitHub repository metadata (old .NET Core 3.1 description/topics) requires administrator access to update.
- **No `LICENSE` file is present yet**; the owner must choose a license and confirm any third-party rights before advertising the code as legally open-source licensed.

## Historical snapshot

The archived tag [`v1.0.0-legacy`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/v1.0.0-legacy) preserves the original .NET Core 3.1 code at commit `6ce03d7f`. It is not a supported release; see [LEGACY.md](LEGACY.md) and [migration guidance](docs/migration-from-legacy.md).
