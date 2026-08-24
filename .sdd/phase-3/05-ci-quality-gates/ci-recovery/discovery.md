# Discovery - CI Recovery

## Repository Context

- Current branch created for this work: `fix/ci-quality-gates`.
- Base branch at start: `main`.
- Initial worktree had a pre-existing modification in `tools/OpenApiGenerator/packages.lock.json`.
- That lock-file change updates transitive package resolutions and is not required for this CI recovery.

## Files Read

- `AGENTS.md`
- `.sdd/phase-3/README.md`
- `.sdd/phase-3/status.md`
- `.sdd/phase-3/decisions.md`
- `.sdd/phase-3/handoff.md`
- `.sdd/phase-3/05-ci-quality-gates/`
- `.sdd/sonarcloud-integration/`
- `docs/quality-gates.md`
- `docs/quality/sonarcloud.md`
- `.github/workflows/ci.yml`
- `.github/workflows/codeql.yml`
- `.github/workflows/dependency-review.yml`

## Current Workflow Shape

`ci.yml` has a single job named `Build, test and quality gates`.

The job currently runs:

1. checkout
2. setup .NET
3. NuGet cache
4. SonarCloud cache
5. install SonarScanner
6. restore
7. `sonarscanner begin`
8. build
9. tests with coverage
10. OpenAPI generation and checks
11. NuGet package checks
12. `sonarscanner end`
13. artifact uploads with `if: always()` and `if-no-files-found: error`

This means SonarCloud initialization is on the critical path before build, tests, OpenAPI and package checks.

## Pull Request Contexts

| PR | Author | Head branch | Context | CI result | Other checks |
| --- | --- | --- | --- | --- | --- |
| `#24` | `rodri-oliveira-dev` | `phase/4-architecture-modernization` | trusted same-repository PR | failed at SonarCloud Quality Gate end step | CodeQL and Dependency Review passed |
| `#25` | Dependabot | `dependabot/github_actions/github-actions-e4270ca1f3` | Dependabot PR | failed at SonarCloud begin due empty token | CodeQL and Dependency Review passed |
| `#26` | Dependabot | `dependabot/nuget/dotnet-dependencies-24ce69a2ac` | Dependabot PR | failed at SonarCloud begin due empty token | CodeQL and Dependency Review passed |
| `#27` | Dependabot | `dependabot/nuget/src/dotnet-dependencies-85ef945bd1` | Dependabot PR | failed at SonarCloud begin due empty token | CodeQL and Dependency Review passed |

## Failed Run Evidence

Run `31529967515` for PR `#24`:

- `Begin SonarCloud analysis`: success.
- Build, tests, coverage, OpenAPI and package checks: success.
- `End SonarCloud analysis and enforce Quality Gate`: failure.
- Log reported `QUALITY GATE STATUS: FAILED`.
- SonarCloud API for PR `24` reports `new_coverage=72.2` with threshold `80`; `new_duplicated_lines_density=0.9` with threshold `3`.

Run `32029331475` for PR `#27`:

- Runner reported `Secret source: Dependabot`.
- `SONAR_TOKEN` was empty in the scanner step environment.
- `Begin SonarCloud analysis` failed with invalid `sonar.token=`.
- Build, tests, OpenAPI and package checks were skipped.
- Test and coverage artifact uploads failed because no TRX or coverage files existed.
- OpenAPI artifact upload succeeded because versioned JSON files already existed after checkout.

Runs for PRs `#25` and `#26` show the same Dependabot pattern:

- `Secret source: Dependabot`.
- Empty `SONAR_TOKEN`.
- Scanner begin failure before build and tests.
- Artificial test and coverage upload failures after skipped test steps.

## Remote SonarCloud State

SonarCloud project key:

```text
rodri-oliveira-dev_web-api-core-seed2
```

Quality Gate API on the project reports current status `ERROR`:

- `new_coverage=75.0`, threshold `80`, status `ERROR`.
- `new_duplicated_lines_density=0.0`, threshold `3`, status `OK`.

The remote gate still fails on coverage, not duplication.

## Actions And Dependency Changes

PR `#25` updated GitHub Actions versions. Later Dependabot logs show `actions/checkout@v7`, `actions/setup-dotnet@v6`, `actions/cache@v6` and `actions/upload-artifact@v7`.

The same empty-token failure occurred with both old and updated action versions. The actions update changed runner/action versions but did not cause the SonarCloud authentication failure. Artifact behavior remained consistent with `if-no-files-found: error`: missing TRX and coverage files fail the upload step when tests never ran.

PRs `#26` and `#27` updated NuGet dependencies. They did not cause the SonarCloud begin failure; the failure happened before build and tests due missing token.

## Security And Permission Findings

- `ci.yml` grants `contents: read` and `pull-requests: read` to the whole workflow.
- `pull-requests: read` is only needed by SonarCloud PR analysis, not by the independent validation job.
- `codeql.yml` grants `security-events: write`, which is required for CodeQL upload.
- `dependency-review.yml` grants `contents: read` and `pull-requests: read`, which is appropriate for dependency review.
- No workflow uses `pull_request_target`.
