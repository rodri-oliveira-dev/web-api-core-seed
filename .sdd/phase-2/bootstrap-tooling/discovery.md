# Discovery - Bootstrap Tooling

## Destination Inventory

| Item | Result |
| --- | --- |
| Current branch before creation | `phase/1-preserve-legacy` |
| Working branch | `phase/2-dotnet-10-migration` |
| Default branch checked locally | `main` |
| Phase 1 integrated in `main` | No |
| Solution | `RestauranteAPI.sln` |
| API project | `src/DevIO.Api/Restaurante.IO.Api.csproj` |
| Business project | `src/DevIO.Business/Restaurante.IO.Business.csproj` |
| Data project | `src/DevIO.Data/Restaurante.IO.Data.csproj` |
| Test project | `test/Pedidos.Test/Pedidos.Test.csproj` |
| Target framework | `netcoreapp3.1` |
| Existing VS Code files | `.vscode/launch.json`, `.vscode/tasks.json` |
| Existing GitHub directory | Absent |
| Existing `.agents/` | Absent |
| Existing `.githooks/` | Absent |
| Existing setup scripts | Absent |
| Existing development docs | `README.md`, `LEGACY.md`, `src/README.md`, `.sdd/phase-1/` |

## Existing Legacy Notes

Phase 1 recorded that restore/build are blocked in the local environment by missing .NET Core 3.1 runtime/SDK and invalid local NuGet metadata for `microsoft.netcore.targets/1.1.0`.

## Source Inventory

Source repository:

```text
https://github.com/rodri-oliveira-dev/poc-arquitetura
```

Source SHA:

```text
9029163f1a795a1bb18f138dd8fa9179f13f544e
```

Source top commit:

```text
9029163 Merge pull request #91 from rodri-oliveira-dev/docs/c4-likec4-architecture-review
```

## Source Candidate Directories

| Path | Finding |
| --- | --- |
| `AGENTS.md` | Exists, but specific to a different architecture and service topology. |
| `.agents/skills/` | Exists with 17 skills plus third-party notices. |
| `.vscode/` | Exists, but includes personal paths, source solution names and excluded tooling. |
| `*.code-workspace` | `poc-arquitetura.code-workspace` exists and is source-specific. |
| `.github/` | Exists with ownership, local composite actions and many workflows. |
| `.githooks/` | Exists with `commit-msg`, `post-merge` and a complex `pre-push`. |
| `scripts/setup/` | Exists with hook setup scripts and Python tests. |

## Skills Found

- `ci-release-governance`
- `configuring-opentelemetry-dotnet`
- `coverage-analysis`
- `ddd-implementation-vernon`
- `ddd-modeling-vernon`
- `docker-compose-container-baseline`
- `dotnet-refactoring-engineer`
- `dotnet-service-change`
- `gcp-cli-auth-governance`
- `gcp-cloud-run-deployment`
- `gcp-cloud-sql-postgres`
- `integration-tests-dotnet`
- `nginx-edge-local`
- `optimizing-ef-core-queries`
- `repository-governance-sdd`
- `terraform-gcp-iac`
- `test-anti-patterns`

## Skill Selection Matrix

| Skill | Classification | Reason |
| --- | --- | --- |
| `repository-governance-sdd` | Adapt and include | Directly supports this task and later SDD prompts. |
| `dotnet-service-change` | Adapt and include | Useful for the .NET 10 migration prompt after removing source-specific assumptions. |
| `dotnet-refactoring-engineer` | Adapt and include | Useful for safe modernization and code review. |
| `integration-tests-dotnet` | Adapt and include | Planned for later Phase 2 testing prompts; marked conditional. |
| `test-anti-patterns` | Adapt and include | Useful for auditing existing and future tests. |
| `ddd-modeling-vernon` | Defer | Useful later, but not needed for task 00 or issue #4. |
| `ddd-implementation-vernon` | Defer | Useful when modular domain design starts, not for bootstrap. |
| `configuring-opentelemetry-dotnet` | Defer | Relevant only when observability is implemented. |
| `optimizing-ef-core-queries` | Defer | Relevant after EF Core migration or performance work. |
| `coverage-analysis` | Defer | Coverage tooling and gates are not established yet. |
| `ci-release-governance` | Defer | Full CI/release governance needs a modern build baseline. |
| `docker-compose-container-baseline` | Exclude | Source-specific container policy and future local orchestration are out of scope. |
| `gcp-cli-auth-governance` | Exclude | Cloud provider operations are out of scope. |
| `gcp-cloud-run-deployment` | Exclude | Deployment target is out of scope. |
| `gcp-cloud-sql-postgres` | Exclude | Target database direction is SQL Server, not PostgreSQL. |
| `nginx-edge-local` | Exclude | No local edge proxy exists in the target. |
| `terraform-gcp-iac` | Exclude | Infrastructure as code is out of scope. |

## GitHub Workflow Inventory

| Source workflow | Classification |
| --- | --- |
| `dependency-review.yml` | Adapt and include |
| `codeql.yml` | Defer |
| `dotnet.yml` | Defer |
| `script-quality.yml` | Defer |
| `container-baseline.yml` | Exclude |
| `event-contracts.yml` | Exclude |
| `infrastructure-security.yml` | Exclude |
| `loadtests-smoke.yml` | Exclude |
| `mutation-tests.yml` | Exclude |
| `openapi-contracts.yml` | Defer |
| `owasp-zap.yml` | Defer |
| `pages-architecture.yml` | Exclude |
| `pr-advisory-checks.yml` | Defer |
| `pr-owner-assignment.yml` | Defer |
| `publish-shared-nuget.yml` | Exclude |
| `release.yml` | Defer |
| `terraform-validation.yml` | Exclude |
