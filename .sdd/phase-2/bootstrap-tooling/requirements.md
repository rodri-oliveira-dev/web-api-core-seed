# Requirements - Bootstrap Tooling

## Goal

Prepare governance, automation and development experience for Phase 2 without implementing the technical migration.

## Required Outputs

- `AGENTS.md`
- `.agents/skills/`
- `.vscode/`
- `web-api-core-seed.code-workspace`
- `.github/`
- `.githooks/`
- `scripts/setup/`
- `.sdd/phase-2/`

## Constraints

- Do not change target framework.
- Do not change NuGet packages.
- Do not change C# code.
- Do not change migrations, database, authentication, HTTP contracts or existing tests.
- Do not copy entire source directories indiscriminately.
- Do not import Sonar.
- Do not push, open Pull Request or close issues.
- Create exactly one semantic commit: `chore: bootstrap modernization tooling`.

## SDD Order

1. Specification
2. Discovery
3. Design
4. Development
5. Validation
6. Delivery
