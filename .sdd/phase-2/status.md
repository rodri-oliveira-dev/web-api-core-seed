# Status - Phase 2

## Current State

| Field | Value |
| --- | --- |
| Phase | Phase 2 - Modernization to .NET 10 |
| Current task | `01 - Migração .NET 10` |
| Current branch | `phase/2-dotnet-10-migration` |
| Branch base | `phase/1-preserve-legacy` |
| Base SHA | `2799562943ac03926d69bc716617d091d04ecc82` |
| Initial branch SHA | `2799562943ac03926d69bc716617d091d04ecc82` |
| Source repository SHA | `9029163f1a795a1bb18f138dd8fa9179f13f544e` |
| Related next issue | `#5 - Modernize hosting model` |

## Base Selection

Phase 1 has not been integrated into `main` in this local repository. `git merge-base --is-ancestor phase/1-preserve-legacy main` returned false, so the Phase 2 branch was created from the final commit of `phase/1-preserve-legacy`.

## Selected Artifacts

- `AGENTS.md`
- `.agents/skills/`
- `.vscode/`
- `web-api-core-seed.code-workspace`
- `.github/CODEOWNERS`
- `.github/PULL_REQUEST_TEMPLATE.md`
- `.github/dependabot.yml`
- `.github/workflows/dependency-review.yml`
- `.githooks/pre-push`
- `.githooks/README.md`
- `scripts/setup/configure-git-hooks.sh`
- `scripts/setup/configure-git-hooks.ps1`
- `.sdd/phase-2/`

## Adapted Artifacts

- Repository governance guidance adapted from the source repository.
- Selected skills rewritten for the actual target repository.
- VS Code settings and tasks adapted to `RestauranteAPI.sln`.
- GitHub ownership and dependency review adapted to current paths.
- Git hook rewritten as a simple self-contained hook.
- Hook setup scripts simplified to configure only local repository settings.

## Excluded Artifacts

- Source repository workflows that depend on missing scripts, modern solution files, coverage gates, release automation, container scans, infrastructure, load tests, generated contracts or unavailable services.
- Any Sonar-related automation or settings.
- Skills tied to GCP, Cloud Run, Cloud SQL PostgreSQL, Terraform, Nginx, Kafka, or source-specific business services.

## Prompt Status

| Prompt | Status |
| --- | --- |
| 00 - Bootstrap | concluído |
| 01 - Migração .NET 10 | concluído |
| 02 - Hosting moderno | pendente |
| 03 - Problem Details | pendente |
| 04 - Rate limiting nativo | pendente |
| 05 - OpenAPI e versionamento | pendente |

## Validations

Final validation results for prompt 01 are recorded in `01-dotnet-10-migration/validation.md`.

Summary:

- `dotnet restore` passed with SDK `10.0.302`.
- `dotnet build --configuration Release --no-restore` passed.
- `dotnet test --configuration Release --no-build` passed: 21 tests.
- `dotnet list package --vulnerable` reported no vulnerable packages.
- Smoke test started the API, confirmed Swagger `200` and `/error/404` returning `404`.
- `/hc` executed the SQL health check but did not return before timeout because local SQL Server was unavailable; Redis and Seq were disabled for the smoke attempt.

## Blockers

No build or test blocker remains for the migrated solution.

Runtime limitations:

- Full `/hc` validation still depends on external SQL Server availability and the legacy health response pipeline.
- HealthChecks UI web `/hc-ui` is temporarily disabled because the latest available `AspNetCore.HealthChecks.UI` package is 9.0.0 and is not runtime-compatible with EF Core 10 in this solution.

## Next Step

Run Prompt 2 for:

```text
#5 - Modernize hosting model
```
