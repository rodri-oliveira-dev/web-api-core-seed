# Source Manifest - Bootstrap Tooling

Source repository SHA: `9029163f1a795a1bb18f138dd8fa9179f13f544e`

| Source path | Action | Destination path | Justification | Main changes | Dependencies | License/attribution |
| --- | --- | --- | --- | --- | --- | --- |
| `AGENTS.md` | adapted | `AGENTS.md` | Needed as global governance. | Rewritten for target state, Phase 2 SDD and planned .NET 10 roadmap. | Existing repo docs and projects. | Source repository internal guidance. |
| `.agents/skills/repository-governance-sdd/SKILL.md` | adapted | same path | Directly supports SDD governance. | SDD order aligned to this phase and target paths. | `.sdd/`, `AGENTS.md`. | Source repository internal guidance. |
| `.agents/skills/dotnet-service-change/SKILL.md` | adapted | same path | Supports issue `#4` migration and later technical changes. | Removed source service topology and commands; uses `RestauranteAPI.sln`. | Existing solution and projects. | Source repository internal guidance. |
| `.agents/skills/dotnet-refactoring-engineer/SKILL.md` | adapted | same path | Supports safe modernization/refactoring. | Condensed and aligned to target repository. | Existing solution and tests. | Source repository internal guidance. |
| `.agents/skills/integration-tests-dotnet/SKILL.md` | adapted | same path | Future Phase 2 test strategy. | Marked `WebApplicationFactory` and Testcontainers as conditional. | Future test infrastructure. | Source repository internal guidance. |
| `.agents/skills/test-anti-patterns/SKILL.md` | adapted | same path | Useful for current and future test audits. | Removed source-specific assumptions. | `test/Pedidos.Test`. | MIT from `.agents/skills/THIRD-PARTY-NOTICES.md`. |
| `.agents/skills/THIRD-PARTY-NOTICES.md` | adapted | same path | Required for selected MIT-derived skill. | Kept only relevant notice. | Selected skill with MIT frontmatter. | MIT, .NET Foundation and Contributors. |
| `.agents/skills/THIRD_PARTY_NOTICES.md` | excluded | none | Only needed for deferred DDD skills. | Not copied. | None. | MIT references deferred. |
| `.agents/skills/ci-release-governance/SKILL.md` | deferred | none | Useful after build and CI baseline exist. | Not copied. | Future CI design. | Source repository internal guidance. |
| `.agents/skills/configuring-opentelemetry-dotnet/SKILL.md` | deferred | none | Observability not implemented in this task. | Not copied. | Future observability packages/config. | MIT from source notice. |
| `.agents/skills/coverage-analysis/` | deferred | none | Coverage gates not established yet. | Not copied, including helper scripts. | Future coverage tooling. | MIT from source notice. |
| `.agents/skills/ddd-modeling-vernon/SKILL.md` | deferred | none | Domain modeling is not part of bootstrap or issue `#4`. | Not copied. | Future domain design prompt. | MIT references deferred. |
| `.agents/skills/ddd-implementation-vernon/SKILL.md` | deferred | none | Domain implementation design is not part of bootstrap. | Not copied. | Future modular design prompt. | MIT references deferred. |
| `.agents/skills/docker-compose-container-baseline/SKILL.md` | excluded | none | Container baseline is source-specific and out of scope. | Not copied. | None. | Source repository internal guidance. |
| `.agents/skills/gcp-cli-auth-governance/SKILL.md` | excluded | none | Cloud provider operations are out of scope. | Not copied. | None. | Source repository internal guidance. |
| `.agents/skills/gcp-cloud-run-deployment/SKILL.md` | excluded | none | Deployment target is out of scope. | Not copied. | None. | Source repository internal guidance. |
| `.agents/skills/gcp-cloud-sql-postgres/SKILL.md` | excluded | none | Target direction is SQL Server, not PostgreSQL. | Not copied. | None. | Source repository internal guidance. |
| `.agents/skills/nginx-edge-local/SKILL.md` | excluded | none | No target edge proxy exists. | Not copied. | None. | Source repository internal guidance. |
| `.agents/skills/optimizing-ef-core-queries/SKILL.md` | deferred | none | Query optimization belongs to later EF work. | Not copied. | Future EF Core migration. | MIT from source notice. |
| `.agents/skills/terraform-gcp-iac/SKILL.md` | excluded | none | Infrastructure as code is out of scope. | Not copied. | None. | Source repository internal guidance. |
| `.vscode/extensions.json` | adapted | `.vscode/extensions.json` | Improves developer onboarding. | Removed personal, source-specific and excluded extensions. | VS Code. | Source repository internal guidance. |
| `.vscode/settings.json` | adapted | `.vscode/settings.json` | Portable workspace defaults. | Uses `RestauranteAPI.sln`; removed personal paths and excluded tooling. | VS Code. | Source repository internal guidance. |
| `.vscode/tasks.json` | adapted | `.vscode/tasks.json` | Provides commands that exist today. | Uses restore/build/test/run API and hook setup only. | .NET SDK, setup scripts. | Source repository internal guidance. |
| `.vscode/launch.json` | adapted | `.vscode/launch.json` | Debug API from VS Code. | Uses real API project and no invented port. | .NET SDK. | Source repository internal guidance. |
| `.vscode/rest-client.env.json` | excluded | none | Could contain environment assumptions and no HTTP files exist. | Not copied. | None. | Source repository internal guidance. |
| `poc-arquitetura.code-workspace` | adapted | `web-api-core-seed.code-workspace` | Repository workspace. | Renamed, simplified, target solution. | VS Code. | Source repository internal guidance. |
| `.github/CODEOWNERS` | adapted | `.github/CODEOWNERS` | Ownership for target paths. | Removed source service paths and added current paths. | GitHub. | Source repository internal guidance. |
| `.github/PULL_REQUEST_TEMPLATE.md` | copied | `.github/PULL_REQUEST_TEMPLATE.md` | Created for Phase 2 governance. | Source had no equivalent; target-specific template created. | GitHub. | New target artifact. |
| `.github/dependabot.yml` | copied | `.github/dependabot.yml` | Created for dependency hygiene. | Source had no equivalent; target-specific config created. | GitHub Dependabot. | New target artifact. |
| `.github/workflows/dependency-review.yml` | adapted | same path | Useful and structurally independent of build. | Added target paths-ignore. | GitHub dependency review action. | GitHub Actions. |
| `.github/workflows/codeql.yml` | deferred | none | May require build baseline not available in current legacy environment. | Not copied. | Future modern build. | GitHub Actions. |
| `.github/workflows/dotnet.yml` | excluded | none | Contains Sonar and source-specific scripts/solutions. | Not copied. | None. | GitHub Actions. |
| Other `.github/workflows/*` | excluded/deferred | none | Not executable for target today or tied to unavailable future capabilities. | Not copied. | Future prompts as needed. | GitHub Actions. |
| `.github/actions/*` | deferred | none | Local actions are useful only with deferred workflows. | Not copied. | Future workflows. | GitHub Actions. |
| `.githooks/pre-push` | adapted | `.githooks/pre-push` | Local proportional validation. | Rewritten from scratch for target solution and no external scripts. | Git, .NET SDK. | Source repository internal guidance. |
| `.githooks/commit-msg` | deferred | none | Not required by prompt. | Not copied. | None. | Source repository internal guidance. |
| `.githooks/post-merge` | deferred | none | Not required by prompt. | Not copied. | None. | Source repository internal guidance. |
| `scripts/setup/configure-git-hooks.sh` | adapted | same path | Local hook setup. | Simplified to one hook and POSIX sh. | Git. | Source repository internal guidance. |
| `scripts/setup/configure-git-hooks.ps1` | adapted | same path | Windows hook setup. | Simplified to one hook. | Git, PowerShell. | Source repository internal guidance. |
| `scripts/setup/tests/*` | excluded | none | Python test dependency is unnecessary for this repository now. | Not copied. | None. | Source repository internal guidance. |
