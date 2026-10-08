# Security Policy

[English README](README.md) | [README em português](README.pt-BR.md)

## Supported versions

| Version/ref | Status | Security maintenance |
| --- | --- | --- |
| `main` (.NET 10) | Actively developed | Security fixes considered and tracked on the maintained branch |
| `v1.0.0-legacy` and `legacy/netcoreapp3.1` (.NET Core 3.1) | Historical, unsupported | **No security fixes or backports promised** |
| Future `v2.0.0` | Not yet released | Follow [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23) |

The legacy runtime reached end of support on December 13, 2022. Do not deploy it in production.

## Reporting a vulnerability

**Do not open a public issue, PR, or discussion with exploit details, credentials or sensitive data.** Use GitHub's [private security advisory reporting flow](https://github.com/rodri-oliveira-dev/web-api-core-seed/security/advisories/new) **if it is enabled** for this repository. If that feature is unavailable, contact the repository maintainer privately via an available channel on their [GitHub profile](https://github.com/rodri-oliveira-dev), without publishing an exploit.

Include a concise description, affected ref/commit and endpoint/component, prerequisites, expected versus observed behavior, a minimal redacted reproduction and impact assessment. Share proof-of-concept material only through a private channel.

The maintainers will triage received reports based on impact, reproducibility and available capacity. No fixed response or patch timeframe is promised. Security fixes may be coordinated before any public disclosure.

## Security boundaries

- The repository and sample Compose environment are **development/reference material**, not a hardened production deployment.
- Use separate non-production credentials and replace all defaults/placeholders; do not paste secrets into GitHub issues.
- Do not enable wildcard CORS for credentialed origins or expose SQL Server/Redis directly to untrusted networks.
- JWT secrets, database connections and seed user passwords must be managed via User Secrets or secret managers.
- The explicit development seed is prohibited in Production and must not be used against real customer data.
- Redis response caching applies only to eligible anonymous GET requests, not private, authenticated, error or cookie-setting responses.
- Review threat models and dependencies before production deployment; include updates from CodeQL, Dependabot and the CI quality gates.

For architecture context see [docs/architecture.md](docs/architecture.md) and the [development environment guide](docs/development/containerized-local-development.md).