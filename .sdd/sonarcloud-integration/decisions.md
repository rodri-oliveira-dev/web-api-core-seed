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

## D012 - Keep Secrets Scoped To Scanner Steps

Context: The workflow needs `SONAR_TOKEN` for `dotnet sonarscanner begin` and `dotnet sonarscanner end`, but secrets should not be global workflow values.

Decision: Reference `secrets.SONAR_TOKEN` only as step-level environment data on the two scanner steps, and pass it to the scanner through `$SONAR_TOKEN`.

Alternatives considered: Store the token in global workflow `env`; write the token directly in scanner command arguments with GitHub expression interpolation.

Consequences: The scanner receives the required token while the workflow keeps non-secret Sonar identity values separate from secret material.

Risks: The workflow still depends on the external GitHub secret being configured before the first successful CI analysis.

Mitigation: Keep `SONAR_TOKEN` documented as an external setup item and never write its value to versioned files or logs.

## D013 - Include The Active Modernization Branch In Push Analysis

Context: The SDD context registered `phase/4-architecture-modernization` as the active modernization branch to consider, and the implementation prompt asked to configure the pushes defined in the SDD without broadening to all branches.

Decision: Configure push analysis for `main` and `phase/4-architecture-modernization`, while keeping pull request analysis limited to PRs targeting `main`.

Alternatives considered: Keep pushes restricted to `main`; broaden pushes to all branches.

Consequences: The active modernization branch can run the same CI and SonarCloud analysis when branch analysis is available.

Risks: SonarCloud branch analysis for non-main branches can depend on the SonarCloud plan and project configuration.

Mitigation: Register this as an external limitation and keep the trigger limited to the one active modernization branch instead of all branches.

## D014 - Filter Test Assemblies During Coverage Collection

Context: Local validation of OpenCover reports found stale test assemblies (`Pedidos.Test` and `WebApiCoreSeed.Tests`) in the unit coverage XML because old DLLs remained in the test output directory. Test assemblies are not production coverage targets and can inflate or distort metrics.

Decision: Configure Coverlet Collector with `DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*`.

Alternatives considered: Exclude test projects only in SonarCloud; clean only the local workspace; switch to another coverage strategy.

Consequences: Coverage reports sent to SonarCloud contain production assemblies only.

Risks: A production assembly with a name matching one of these test suffixes would be excluded from coverage collection.

Mitigation: Current production assemblies do not use these suffixes. Static analysis still covers production code, and this exclusion targets assembly names rather than production paths.

## D015 - Exclude Generated Source Files During Coverage Collection

Context: Local XML validation found `OpenApiXmlCommentSupport.generated.cs` from `Microsoft.AspNetCore.OpenApi.SourceGenerators` in OpenCover file paths. Generated source should not influence product coverage metrics.

Decision: Configure Coverlet Collector with `DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs`.

Alternatives considered: Leave generated source in reports and rely only on SonarCloud exclusions; add broad `obj/**` exclusions.

Consequences: Generated source files are removed before coverage import and coverage XML path validation no longer reports missing generated files.

Risks: Hand-written files intentionally named `*.generated.cs` would be excluded from coverage.

Mitigation: The repository treats this naming pattern as generated-source convention. Static analysis exclusions remain narrow and do not exclude hand-written production directories.

## D016 - Use Suite-Scoped Report Paths

Context: VSTest can copy coverage attachments under run-specific `In/...` folders in addition to the direct Coverlet output folder. A broad `TestResults/**/coverage.opencover.xml` pattern can import duplicate OpenCover files.

Decision: Use suite-scoped report paths:

```text
TestResults/Unit/*/coverage.opencover.xml
TestResults/Integration/*/coverage.opencover.xml
```

TRX import uses the stable file names:

```text
TestResults/Unit/unit-tests.trx
TestResults/Integration/integration-tests.trx
```

Alternatives considered: Keep broad recursive globs; manually post-process reports; add another coverage tool.

Consequences: SonarCloud imports the direct Coverlet reports without depending on concrete GUID directory names and without matching VSTest attachment copies.

Risks: Multiple stale direct coverage directories could still match on reused runners.

Mitigation: The workflow now recreates `TestResults/Unit` and `TestResults/Integration` immediately before tests, so stale artifacts are removed before report generation.

## D017 - Keep SonarCloud Exclusions Narrow And Evidence-Based

Context: Prompt 3 required reviewing SonarCloud exclusions without shrinking analysis scope artificially.

Decision: Keep `sonar.exclusions`, `sonar.test.inclusions` and `sonar.test.exclusions` unset. Keep only coverage and duplication exclusions for generated, generated-like, test or auxiliary files.

Accepted exclusions:

| Pattern | Type | Justification | Expected impact | Risk of masking problems |
| --- | --- | --- | --- | --- |
| `tests/**` | `sonar.coverage.exclusions` | Test projects are not production coverage targets. | Prevents test code from changing product coverage. | Low; static analysis still sees files unless Sonar classifies them as tests. |
| `tools/**` | `sonar.coverage.exclusions` | `tools/OpenApiGenerator` is an auxiliary build tool, not API runtime behavior. | Keeps product coverage focused on shipped API/modules. | Medium; coverage debt in tooling is not represented in product coverage. |
| `**/Migrations/**` | `sonar.coverage.exclusions`, `sonar.cpd.exclusions` | EF migrations are generated-like schema history. | Removes low-signal coverage and duplication noise. | Medium; hand edits inside migrations could be less visible in coverage/duplication metrics. |
| `**/*ModelSnapshot.cs` | `sonar.coverage.exclusions`, `sonar.cpd.exclusions` | EF model snapshots are generated metadata. | Removes generated metadata noise. | Low; snapshots should be reviewed through migration diffs. |
| `**/*.Designer.cs` | `sonar.coverage.exclusions`, `sonar.cpd.exclusions` | Designer files are generated or generated-like code. | Removes generated code noise. | Low; no hand-written production logic should use this suffix. |
| `docs/openapi/**/*.json` | `sonar.cpd.exclusions` | OpenAPI contracts are generated and validated separately by diff. | Avoids duplication noise in generated JSON. | Low; JSON sync remains enforced by CI. |
| `**/packages.lock.json` | `sonar.cpd.exclusions` | Lock files are dependency metadata. | Avoids duplication noise in lock metadata. | Low; package audit commands remain in CI. |

Rejected exclusions:

| Pattern or category | Reason |
| --- | --- |
| `sonar.exclusions` for production paths | Would reduce static analysis scope without a concrete false-positive pattern. |
| `sonar.test.inclusions` | Not needed because scanner/test report paths already identify tests. |
| `sonar.test.exclusions` | Not needed; no evidence that tests are being misclassified for analysis. |
| Controllers, handlers, services, repositories, domain models, value objects, infrastructure | These are hand-written product code and must remain visible to coverage and static analysis. |
| Low-coverage or hard-to-test classes | Excluding them would artificially improve metrics. |

## D018 - Keep External SonarCloud And GitHub Administration Manual

Context: Prompt 4 validates local behavior and documents operational setup, but the repository does not have access to SonarCloud administrative state or GitHub repository rulesets from the local workspace.

Decision: Keep SonarCloud import, automatic analysis disablement, token creation, secret creation, Quality Gate association and branch protection as documented manual tasks until evidence exists from GitHub/SonarCloud.

Alternatives considered: Mark external tasks complete based on intended configuration; add placeholder secrets or check names.

Consequences: SDD status remains honest about local validation versus external validation.

Risks: The first GitHub Actions execution can still fail if an administrator has not completed the external setup.

Mitigation: Document setup steps, troubleshooting and the need to select exact status checks only after the first workflow execution.

## D019 - Align Project Key With Existing SonarCloud Project

Context: The maintainer provided the SonarCloud configuration URL `https://sonarcloud.io/project/configuration/AutoScan?id=rodri-oliveira-dev_web-api-core-seed2`, which identifies the imported project key as `rodri-oliveira-dev_web-api-core-seed2`.

Decision: Update the workflow, operational documentation and SDD references to use `rodri-oliveira-dev_web-api-core-seed2`.

Alternatives considered: Rename or reimport the SonarCloud project to the previously expected key; keep repository files pointing to the old expected key.

Consequences: GitHub Actions analysis will target the SonarCloud project that currently exists.

Risks: If the SonarCloud project is later renamed, the workflow key must be updated again.

Mitigation: Keep the key documented in one visible workflow variable and in the SonarCloud operational guide.
