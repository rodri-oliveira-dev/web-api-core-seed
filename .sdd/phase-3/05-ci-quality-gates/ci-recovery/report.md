# Report - CI Recovery

## Summary

The CI workflow was recovered by separating repository-local quality gates from SonarCloud analysis.

`Build, test and quality gates` now runs restore, build, unit tests, integration tests, coverage, OpenAPI validation and package audits without depending on SonarCloud initialization. `SonarCloud Quality Gate` now runs as a separate job for trusted contexts and skips safely for Dependabot and fork pull requests.

## Root Causes

- PR `#24`: real SonarCloud Quality Gate failure caused by coverage below the configured threshold.
- PRs `#25`, `#26` and `#27`: Dependabot pull requests did not receive `SONAR_TOKEN`; the single CI job attempted SonarCloud initialization before build and tests.
- Artifact failures after Dependabot Sonar initialization failure were secondary: test and coverage files did not exist because test steps were skipped.

## Delivery Status

## Files Changed

- `.github/workflows/ci.yml`
- `docs/quality-gates.md`
- `docs/quality/sonarcloud.md`
- `README.md`
- `.sdd/phase-3/decisions.md`
- `.sdd/phase-3/05-ci-quality-gates/ci-recovery/*`
- `.sdd/sonarcloud-integration/decisions.md`

## Strategy

- No `pull_request_target`.
- No `continue-on-error` for SonarCloud.
- Dependabot and fork pull requests skip SonarCloud with an explicit notice because repository secrets are unavailable by design.
- Trusted pushes and trusted same-repository pull requests require `SONAR_TOKEN`; missing token, scanner failure, red Quality Gate or timeout fails `SonarCloud Quality Gate`.
- Artifact uploads are conditional on matching files existing, preventing secondary failures when an earlier step did not produce files.

## Validation

Local validation passed for YAML parsing, restore, build, unit tests, integration tests, OpenAPI generation, JSON validation, OpenAPI sync, vulnerable package audit and `git diff --check`.

Remote validation passed on PR `#29`:

- CI run: `https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/runs/32768850910`
- CodeQL run: `https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/runs/32768850764`
- Dependency Review run: `https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/runs/32768851012`

Checks passed:

- `Build, test and quality gates`
- `SonarCloud Quality Gate`
- `CodeQL analysis`
- `Review dependency changes`
- GitHub code scanning `CodeQL`

The CI run uploaded `test-results`, `coverage-results` and `openapi-contracts`.

## Delivery Status

Pull request open and green:

```text
https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/29
```
