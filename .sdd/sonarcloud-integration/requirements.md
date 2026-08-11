# Requirements - SonarCloud Integration

## Functional Requirements

FR-01. Import or bind the repository `rodri-oliveira-dev/web-api-core-seed` in SonarCloud before the workflow depends on the project.

FR-02. Execute SonarCloud analysis on pushes to the repository's main branch.

FR-03. Execute SonarCloud analysis on pull requests targeting the main branch.

FR-04. Analyze the main branch as the primary branch in SonarCloud.

FR-05. Allow analysis of `phase/4-architecture-modernization` or another active modernization branch when the SonarCloud plan supports branch analysis.

FR-06. Send static analysis results to SonarCloud, including bugs, vulnerabilities, code smells and security hotspots.

FR-07. Send unit test coverage to SonarCloud.

FR-08. Send integration test coverage to SonarCloud.

FR-09. Send TRX test execution reports for unit tests and integration tests.

FR-10. Wait synchronously for Quality Gate processing.

FR-11. Fail the GitHub Actions job when the Quality Gate fails or the Quality Gate wait times out.

FR-12. Preserve the existing CI checks: restore, build, unit tests, integration tests, OpenAPI generation, OpenAPI sync verification, vulnerable package audit, deprecated package report and artifact upload.

FR-13. Add manual execution through `workflow_dispatch`.

FR-14. Never print, commit, echo or otherwise expose the `SONAR_TOKEN` value.

FR-15. Keep pull request analysis able to decorate PRs when the SonarCloud GitHub integration is configured.

## Non-Functional Requirements

NFR-01. The workflow must be reproducible from versioned configuration and the repository-pinned SDK.

NFR-02. SonarScanner for .NET must use an explicit version.

NFR-03. Git checkout must fetch complete history with `fetch-depth: 0`.

NFR-04. Cache usage must be safe and must not cache secrets.

NFR-05. Workflow duplication should be minimized through existing environment variables and clear paths.

NFR-06. The workflow must remain readable for maintainers familiar with the current CI.

NFR-07. The workflow must remain compatible with Linux GitHub-hosted runners.

NFR-08. The workflow must not depend on absolute local paths.

NFR-09. No credentials, tokens or secret values may be versioned.

NFR-10. Generated files should not distort coverage or duplication metrics.

NFR-11. The scanner cache should be isolated from NuGet cache and safe to restore across runs.

NFR-12. The integration should fail closed for mandatory gates while still uploading test and coverage artifacts with `if: always()`.

## Acceptance Criteria

AC-01. `.github/workflows/ci.yml` remains valid YAML after the implementation stage.

AC-02. `dotnet restore "$SOLUTION"` runs successfully in CI.

AC-03. `dotnet build "$SOLUTION" --configuration Release --no-restore` runs inside the SonarScanner analysis cycle, after `begin` and before `end`.

AC-04. Unit tests pass in CI.

AC-05. Integration tests pass in CI.

AC-06. TRX files are found at `TestResults/**/*.trx`.

AC-07. OpenCover reports are found at `TestResults/**/coverage.opencover.xml`.

AC-08. SonarScanner `end` completes successfully when the analysis is accepted and the Quality Gate is green.

AC-09. The analysis is visible in SonarCloud under organization `rodri-oliveira-dev`.

AC-10. The workflow fails when the SonarCloud Quality Gate is red.

AC-11. The workflow fails when Quality Gate processing does not complete within the configured timeout.

AC-12. Pull request analysis is associated with the correct PR in SonarCloud.

AC-13. Existing artifacts for test results are still uploaded.

AC-14. Existing coverage artifacts are still uploaded, either by preserving Cobertura output or by updating the artifact contract deliberately.

AC-15. Existing OpenAPI contract validation still runs.

AC-16. Existing vulnerable package and deprecated package commands still run unless an earlier mandatory gate has already failed.

AC-17. External setup documentation states how to create the SonarCloud project, configure `SONAR_TOKEN`, confirm the project key and protect the main branch with required status checks.

AC-18. No workflow log contains the secret value behind `SONAR_TOKEN`.

AC-19. No branch checkout or branch creation is needed to implement the integration.

AC-20. The implementation commit contains only SonarCloud integration files or explicitly related documentation.
