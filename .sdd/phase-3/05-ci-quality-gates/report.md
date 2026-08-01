# Report - 05 CI Quality Gates

## Summary

CI, CodeQL, dependency review, Dependabot and quality-gate documentation were added for the final Phase 3 delivery.

## Workflows

- `.github/workflows/ci.yml`: restore, build, unit tests, integration/container tests, coverage collection, OpenAPI generation/validation/sync, vulnerable package audit and deprecated package report.
- `.github/workflows/codeql.yml`: C# CodeQL analysis for PR, push to `main` and weekly schedule.
- `.github/workflows/dependency-review.yml`: existing workflow preserved and reduced to read-only PR permission.

No workflow was removed.

## Dependabot

`.github/dependabot.yml` now keeps weekly updates for NuGet and GitHub Actions with explicit day/time/timezone and grouped dependency PRs. No reviewers or assignees were added.

## Artifacts

- `test-results`: TRX files.
- `coverage-results`: Cobertura XML files.
- `openapi-contracts`: generated OpenAPI JSON contracts.

## Coverage

Coverage remains informational with no threshold, following Phase 3 decision D002.

## Security

- CodeQL enabled with `security-events: write` only in the CodeQL workflow.
- Dependency Review enabled for PRs with `fail-on-severity: moderate`.
- NuGet vulnerability audit runs in CI.
- No Sonar workflow was added.
- No secrets were versioned.

## Limitations

- `dotnet format --verify-no-changes` is not a gate because it currently fails on existing whitespace debt.
- `dotnet list package --deprecated` reports xUnit 2.9.3 as legacy in test projects; this remains informational.
- `actionlint` was not available locally.
- No `packages.lock.json` exists; NuGet cache invalidates by manifests but lock-file adoption remains a future reproducibility improvement.

## Delivery

- Validacao consolidada local: passou.
- Commit: `ci: add quality and security workflows` (este commit).
- Push: nao realizado.
