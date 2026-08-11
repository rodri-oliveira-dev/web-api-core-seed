# Design - Bootstrap Tooling

## AGENTS.md

Create a new target-specific `AGENTS.md` instead of copying the source file. It must distinguish:

- current .NET Core 3.1 state;
- planned .NET 10 direction;
- rules already in force;
- features not yet implemented.

## Skills

Include only five adapted skills:

- `repository-governance-sdd`
- `dotnet-service-change`
- `dotnet-refactoring-engineer`
- `integration-tests-dotnet`
- `test-anti-patterns`

The first three support the immediate migration work. `integration-tests-dotnet` is included as planned/conditional because later Phase 2 prompts explicitly target `WebApplicationFactory` and Testcontainers. `test-anti-patterns` supports review of existing tests without changing them.

## VS Code

Create portable JSON files:

- `extensions.json`
- `settings.json`
- `tasks.json`
- `launch.json`

Use `RestauranteAPI.sln` and the real API project. Do not set application URLs because no launch profile or verified port exists.

## Workspace

Create `web-api-core-seed.code-workspace` with the repository root as the only folder and the real solution as the .NET default.

## GitHub

Create:

- `CODEOWNERS`
- `PULL_REQUEST_TEMPLATE.md`
- `dependabot.yml`
- `workflows/dependency-review.yml`

Defer workflows requiring successful .NET build, generated contracts, coverage, release, scripts or future architecture.

## Git Hooks

Create a lightweight POSIX `sh` `pre-push` that:

- resolves the repository root;
- detects documentation-only changes;
- runs .NET restore/build/test only for .NET-impacting changes;
- uses `RestauranteAPI.sln`;
- avoids containers and heavy local gates.

Create setup scripts for `sh` and PowerShell that configure only local `core.hooksPath=.githooks`, support check mode and avoid overwriting an existing setting unless forced.
