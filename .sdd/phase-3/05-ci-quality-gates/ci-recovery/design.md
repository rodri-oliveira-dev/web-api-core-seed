# Design - CI Recovery

## Decision

Split the main CI workflow into two jobs:

- `build-test-quality`: independent repository-local gates.
- `sonarcloud`: SonarCloud scanner and Quality Gate enforcement.

This keeps build, tests, coverage, OpenAPI and package checks available even when SonarCloud cannot initialize.

## Job 1 - Independent Gates

Check name remains:

```text
Build, test and quality gates
```

The job runs:

1. checkout
2. setup .NET
3. NuGet cache
4. restore
5. build
6. prepare `TestResults`
7. unit tests with Cobertura and OpenCover
8. integration tests with Cobertura and OpenCover
9. OpenAPI generation
10. OpenAPI JSON validation
11. OpenAPI sync verification
12. vulnerable package audit
13. deprecated package report
14. artifact uploads only when matching files exist

No SonarCloud step runs in this job.

## Job 2 - SonarCloud Quality Gate

Check name:

```text
SonarCloud Quality Gate
```

The job depends on `build-test-quality`. It first classifies the execution context:

- Dependabot PR: skip SonarCloud safely.
- Fork PR: skip SonarCloud safely.
- Trusted push or trusted same-repository PR: require `SONAR_TOKEN`.

When SonarCloud should run, the job executes:

1. checkout with `fetch-depth: 0`
2. setup .NET
3. NuGet cache
4. SonarCloud cache
5. install pinned `dotnet-sonarscanner`
6. restore
7. `sonarscanner begin`
8. build
9. prepare `TestResults`
10. unit tests with Cobertura and OpenCover
11. integration tests with Cobertura and OpenCover
12. `sonarscanner end`

The scanner still waits for the Quality Gate through:

```text
sonar.qualitygate.wait=true
sonar.qualitygate.timeout=300
```

No `continue-on-error` is used. A red Quality Gate remains a job failure.

## Secret Strategy

`SONAR_TOKEN` is referenced only as step-level environment data in the SonarCloud job.

The preflight step checks whether the secret is present without printing it. If it is missing in a trusted context, the job fails with an error message explaining the required GitHub secret.

Dependabot and fork PRs never receive the token and are not run through `pull_request_target`.

## Permissions

Workflow-level default:

```yaml
permissions:
  contents: read
```

The SonarCloud job gets:

```yaml
permissions:
  contents: read
  pull-requests: read
```

The independent gate job does not need `pull-requests: read`.

## Artifact Uploads

Artifact uploads use `always()` combined with `hashFiles(...) != ''`.

This preserves diagnostics when tests run and fail after creating files, while avoiding artificial upload failures when earlier gates prevent file creation.

## Rejected Alternatives

- Keep one job with conditional scanner steps: rejected because trusted Sonar initialization failures could still stop independent validations or require complex step conditions.
- Use `continue-on-error` for SonarCloud: rejected because it would hide real Quality Gate failures.
- Use `pull_request_target`: rejected because it would expose secrets to untrusted pull request code.
- Make SonarCloud optional for every PR: rejected because trusted contexts must still enforce the Quality Gate.
