# Decisions - SonarCloud Integration

## D001 - Use CI-Based Analysis

Context: The repository already has GitHub Actions as the source of restore, build, test and quality validation.

Decision: SonarCloud analysis will run from GitHub Actions.

Alternatives considered: Local-only scanner execution; SonarCloud automatic analysis.

Consequences: Analysis uses the same SDK, solution and test commands as CI.

Risks: CI becomes slower and depends on SonarCloud availability.

Mitigation: Cache NuGet and Sonar packages, set a Quality Gate timeout and keep the current job timeout visible.

## D002 - Disable SonarCloud Automatic Analysis

Context: Automatic analysis cannot run the repository's .NET build and tests or import generated TRX/OpenCover reports.

Decision: Disable SonarCloud automatic analysis for this project after importing it.

Alternatives considered: Keep automatic analysis enabled together with CI analysis.

Consequences: Avoids duplicate analyses and inconsistent results.

Risks: If CI is misconfigured, no analysis is published.

Mitigation: Protect `main` with required workflow status checks and document external setup.

## D003 - Use SonarScanner For .NET

Context: The project is a .NET solution built with `dotnet build` and `WebApiCoreSeed.slnx`.

Decision: Use `dotnet-sonarscanner` as the scanner.

Alternatives considered: Generic Sonar scanner action; standalone scanner CLI.

Consequences: The scanner can integrate with MSBuild and C# analysis.

Risks: Scanner behavior can change across versions.

Mitigation: Pin an explicit scanner version, initially `11.2.1`, and update deliberately.

## D004 - Generate OpenCover Coverage

Context: Current CI uses Coverlet Collector and produces Cobertura coverage. SonarCloud .NET coverage import should use OpenCover paths.

Decision: Configure Coverlet Collector to generate OpenCover reports for SonarCloud.

Alternatives considered: Keep only Cobertura; use another coverage tool.

Consequences: SonarCloud receives coverage through `sonar.cs.opencover.reportsPaths`.

Risks: Changing coverage output could break the existing coverage artifact upload.

Mitigation: Prefer emitting both `cobertura` and `opencover` formats or update artifacts explicitly.

## D005 - Import TRX Test Reports

Context: Current CI already writes TRX files for unit and integration tests.

Decision: Import `TestResults/**/*.trx` through `sonar.cs.vstest.reportsPaths`.

Alternatives considered: Do not import test execution reports; convert to another test report format.

Consequences: SonarCloud can show test execution data in addition to coverage.

Risks: Missing files or changed paths would silently reduce analysis value.

Mitigation: Keep stable TRX names and add validation checks for report discovery.

## D006 - Use `fetch-depth: 0`

Context: The current checkout uses the default shallow fetch.

Decision: Configure checkout with full Git history.

Alternatives considered: Keep default checkout depth.

Consequences: SonarCloud can compute blame, branch and PR analysis more accurately.

Risks: Checkout can become slower.

Mitigation: Accept the small cost for a small repository and monitor CI duration.

## D007 - Wait Synchronously For Quality Gate

Context: The requirement is to fail the workflow when the Quality Gate is rejected.

Decision: Set `sonar.qualitygate.wait=true` and an explicit timeout.

Alternatives considered: Use an asynchronous dashboard-only gate; use a separate quality gate action.

Consequences: `SonarScanner end` can fail the job based on the Quality Gate result.

Risks: SonarCloud processing delays can fail otherwise healthy builds.

Mitigation: Start with `300` seconds and increase only with evidence.

## D008 - Preserve Existing CI Workflow

Context: `.github/workflows/ci.yml` already restores, builds, tests, validates OpenAPI, checks vulnerable packages and uploads artifacts.

Decision: Integrate SonarCloud into the existing `build-test-quality` job instead of replacing the CI.

Alternatives considered: Create a separate Sonar workflow; rewrite the CI job.

Consequences: Existing validations remain visible under the same job.

Risks: The job becomes larger and failures can stop later validations.

Mitigation: Keep artifact uploads under `if: always()` and avoid unrelated workflow refactors.

## D009 - Use Narrow Exclusions

Context: The repository contains EF migrations, generated OpenAPI JSON, lock files and excluded demo assets.

Decision: Prefer coverage and duplication exclusions for generated or generated-like files rather than broad source exclusions.

Alternatives considered: Analyze everything without exclusions; exclude entire directories broadly.

Consequences: Metrics focus more on hand-written production code while static analysis remains broad.

Risks: Over-exclusion could hide real issues.

Mitigation: Require a justification for each exclusion and avoid excluding hand-written production code from static analysis.

## D010 - Use New Code Metrics

Context: The repository is a modernization seed with legacy history and ongoing architecture work.

Decision: Configure SonarCloud Quality Gate expectations around New Code first.

Alternatives considered: Enforce all-code thresholds immediately.

Consequences: The project can improve incrementally without being blocked by historical debt.

Risks: Existing debt can remain invisible if not tracked.

Mitigation: Keep overall metrics visible in SonarCloud and create separate remediation tasks for legacy hotspots.

## D011 - Protect The Main Branch With Status Checks

Context: Quality Gate failure should block changes from reaching `main`.

Decision: Require the CI job and SonarCloud Quality Gate status checks on `main` after the workflow has run successfully at least once.

Alternatives considered: Rely on reviewer discipline; keep branch protection unchanged.

Consequences: Regressions are blocked consistently.

Risks: Required checks can block emergency fixes if SonarCloud is unavailable.

Mitigation: Document administrative override expectations and keep checks limited to meaningful gates.
