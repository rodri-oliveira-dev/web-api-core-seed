# Handoff - Phase 2 Task 00

## Branch

- Current branch: `phase/2-dotnet-10-migration`
- Branch base: `phase/1-preserve-legacy`
- Base SHA: `2799562943ac03926d69bc716617d091d04ecc82`
- Source repository SHA: `9029163f1a795a1bb18f138dd8fa9179f13f544e`
- Bootstrap commit: `chore: bootstrap modernization tooling` (the current HEAD of this branch after task 00 delivery)

## Created Files

- `AGENTS.md`
- `.agents/skills/`
- `.vscode/extensions.json`
- `.vscode/settings.json`
- `.vscode/tasks.json`
- `.vscode/launch.json`
- `web-api-core-seed.code-workspace`
- `.github/`
- `.githooks/`
- `scripts/setup/`
- `.sdd/phase-2/`

## Available Skills

- `repository-governance-sdd`
- `dotnet-service-change`
- `dotnet-refactoring-engineer`
- `integration-tests-dotnet`
- `test-anti-patterns`

## Rules To Carry Forward

- Follow SDD: Specification, Discovery, Design, Development, Validation, Delivery.
- Read `.sdd/phase-2/` before every Phase 2 prompt.
- Update `status.md` and `handoff.md` before each commit.
- Use one semantic commit per prompt.
- Do not push without explicit request.
- Do not move `legacy/netcoreapp3.1` or `v1.0.0-legacy`.
- Do not describe roadmap items as already implemented.

## Active GitHub Elements

- `CODEOWNERS`
- `PULL_REQUEST_TEMPLATE.md`
- `dependabot.yml`
- `workflows/dependency-review.yml`

## Deferred Workflows

Full .NET CI, CodeQL, release, container, script quality, contract, coverage and other source workflows are deferred until matching scripts, project layout and toolchain exist.

## Hooks

- Hook created: `.githooks/pre-push`
- Setup scripts:
  - `scripts/setup/configure-git-hooks.sh`
  - `scripts/setup/configure-git-hooks.ps1`
- Local hook configuration may be enabled with `core.hooksPath=.githooks`.

## Limitations

- No .NET 10 migration was performed in this task.
- No application code, package reference, migration, database script, authentication behavior, HTTP contract or test was intentionally changed.
- Baseline .NET validation may remain blocked by the legacy runtime/cache limitations recorded in Phase 1.

## Next Objective

First technical objective of Phase 2:

```text
#4 - Migrate the solution to .NET 10
```
