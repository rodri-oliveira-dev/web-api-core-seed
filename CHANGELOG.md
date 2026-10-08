# Changelog

This changelog records notable changes to the **maintained .NET 10 codebase**. It does not imply that a public `v2.0.0` release or NuGet template package has been published. See [issue #23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23) for release preparation.

## [Unreleased]

### Documentation
- Rewrite onboarding guides in English and Brazilian Portuguese, including executable development/test/migration/seed commands.
- Document architecture, configuration, request flow, key ADRs, legacy migration, contribution policy and vulnerability disclosure.

### Modernization already merged into `main`
- Migrate active application and tooling to .NET 10 and `WebApiCoreSeed.slnx`.
- Modernize hosting, Problem Details, API versioning/OpenAPI and native rate limiting.
- Separate sample domain, explicit persistence ports, Unit of Work and EF Core migration ownership.
- Add deterministic development seed with non-production safeguards and containerized local development.
- Add structured logging, OpenTelemetry, security headers, CodeQL, tests and CI quality gates.
- Correct cache isolation, response eligibility and IIS `X-Frame-Options` configuration.

### Template packaging (local preview, not published)
- Add `webapi-seed` template identity, package project, generated project documentation and unique `UserSecretsId`.
- Add isolated NuGet install/generation/restore/build/test/runtime smoke checks in GitHub Actions.
- Add template installation/update/uninstallation instructions in EN/PT-BR. The local package is not published to any feed.

### Planned, not shipped
- Versioned `v2.0.0` GitHub Release and release artifacts ([#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23)).

## Historical snapshot

The tag [`v1.0.0-legacy`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/v1.0.0-legacy) points to the untouched legacy .NET Core 3.1 source commit. It is an **archival tag**, not evidence of a current supported release. See [LEGACY.md](LEGACY.md) for limitations and [migration guidance](docs/migration-from-legacy.md) for compatibility considerations.