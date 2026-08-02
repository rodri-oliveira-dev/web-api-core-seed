# Discovery - Prompt 05 SLNX Migration

## Environment

- Branch: `phase/4-architecture-modernization`.
- Worktree before changes: clean.
- SDK selected by `global.json`: `10.0.302`.
- Host runtime observed by `dotnet --info`: `.NET 10.0.10`, `win-x64`.

## Required Commands

### `dotnet sln WebApiCoreSeed.sln list`

```text
Projetos
--------
src\Modules\Identity\WebApiCoreSeed.Identity.Infrastructure\WebApiCoreSeed.Identity.Infrastructure.csproj
src\Modules\SampleRestaurant\WebApiCoreSeed.SampleRestaurant.Infrastructure\WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj
src\Modules\SampleRestaurant\WebApiCoreSeed.SampleRestaurant\WebApiCoreSeed.SampleRestaurant.csproj
src\WebApiCoreSeed.Api\WebApiCoreSeed.Api.csproj
tests\WebApiCoreSeed.IntegrationTests\WebApiCoreSeed.IntegrationTests.csproj
tests\WebApiCoreSeed.UnitTests\WebApiCoreSeed.UnitTests.csproj
tools\OpenApiGenerator\OpenApiGenerator.csproj
```

### `git grep -n -E 'WebApiCoreSeed\.sln([^x]|$)'`

The initial grep found active references in:

- `.agents/skills/README.md`
- `.agents/skills/dotnet-refactoring-engineer/SKILL.md`
- `.agents/skills/dotnet-service-change/SKILL.md`
- `.githooks/README.md`
- `.githooks/pre-push`
- `.github/CODEOWNERS`
- `.github/PULL_REQUEST_TEMPLATE.md`
- `.github/workflows/ci.yml`
- `.github/workflows/codeql.yml`
- `.vscode/settings.json`
- `.vscode/tasks.json`
- `AGENTS.md`
- `docs/quality-gates.md`
- `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/Context/ApplicationDbContextFactory.cs`
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/Modules/SampleRestaurant/Infrastructure/Persistence/Context/SampleRestaurantDbContextFactory.cs`
- `web-api-core-seed.code-workspace`

The initial grep also found historical SDD references in completed phase and repository-hardening prompt folders. Those references describe earlier states and validation commands and should remain only where explicitly historical.

## Solution Folders

The original `.sln` contained these logical folders:

- `src`
- `src/WebApiCoreSeed.Api`
- `src/Modules`
- `src/Modules/SampleRestaurant`
- `src/Modules/Identity`
- `tests`
- `tools`

## Relevant Configurations

The original `.sln` declared:

- Configurations: `Debug`, `Release`.
- Platforms: `Any CPU`, `x64`, `x86`.
- Project build mappings for every active project to `Any CPU`.

## Test Discovery

- Unit tests: `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`.
- Integration/container tests: `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`, traits `Category=Integration` and `Category=Container`.
- Architecture tests: `tests/WebApiCoreSeed.UnitTests/Arquitetura/ModularHexagonalArchitectureTest.cs`, trait `Architecture=ModularHexagonal`.
