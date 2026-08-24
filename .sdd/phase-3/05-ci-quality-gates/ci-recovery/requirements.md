# Requirements - CI Recovery

## Objective

Recover the GitHub Actions quality gates for `rodri-oliveira-dev/web-api-core-seed` so build, tests, coverage, OpenAPI validation, package analysis, CodeQL, Dependency Review and SonarCloud provide reliable and explicit feedback.

## Functional Requirements

- Restore, build, unit tests, integration tests, coverage collection, OpenAPI generation, OpenAPI JSON validation, OpenAPI sync verification and NuGet package audit must run independently from SonarCloud availability.
- A SonarCloud initialization failure must not prevent the independent validation job from running.
- SonarCloud must remain mandatory for trusted pushes and trusted pull requests when `SONAR_TOKEN` is available.
- Trusted pushes or trusted pull requests with a missing `SONAR_TOKEN` must fail the SonarCloud job with a clear diagnostic.
- Dependabot pull requests must not receive repository secrets and must use a documented safe SonarCloud skip path.
- Fork pull requests must not use `pull_request_target` or receive repository secrets.
- A real SonarCloud Quality Gate failure must fail the SonarCloud job.
- Artifact uploads must not fail only because an earlier step did not create the expected files.
- CodeQL and Dependency Review workflows must remain active and least-privilege.

## Non-Functional Requirements

- Keep workflow changes small and directly related to issue `#13`.
- Do not expose tokens, secret values or sensitive configuration.
- Do not use `continue-on-error` to hide SonarCloud failures.
- Do not make SonarCloud permanently optional.
- Preserve reproducibility by keeping explicit SDK, scanner and cache behavior.
- Do not update dependencies unless required by the CI recovery.

## Acceptance Criteria

- `Build, test and quality gates` can pass or fail based on repository-local gates without depending on SonarCloud.
- `SonarCloud Quality Gate` runs for trusted contexts and fails on scanner or Quality Gate failures.
- Dependabot pull requests complete repository-local gates and skip SonarCloud with an explicit reason.
- Test, coverage and OpenAPI artifacts upload only when matching files exist.
- Documentation explains PR, Dependabot and push behavior.
- SDD recovery artifacts record requirements, discovery, failure matrix, design, tasks, validation and report.
