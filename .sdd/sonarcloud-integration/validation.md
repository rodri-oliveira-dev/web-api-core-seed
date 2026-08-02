# Validation - SonarCloud Integration

## Specification Stage Validations

- Confirmed the active branch with `git branch --show-current`.
- Confirmed the initial worktree state with `git status --short`.
- Confirmed SDK version from `global.json`.
- Confirmed local SDK version with `dotnet --version`.
- Inspected current CI workflow commands, events, permissions, cache and artifacts.
- Inspected CodeQL and dependency review workflows to understand existing quality gates.
- Inspected solution structure through `WebApiCoreSeed.slnx`.
- Inspected production, test and tool project files.
- Inspected central package version files.
- Confirmed test framework and coverage packages.
- Confirmed current TRX and Cobertura output locations from the workflow.
- Confirmed `.sdd/sonarcloud-integration/` did not exist before the specification stage.
- Confirmed current `dotnet-sonarscanner` package version with `dotnet tool search`.
- Reviewed `.gitignore` for generated coverage, test result and scanner outputs.
- Ran `git diff --check`; no whitespace errors were reported.

## Implementation Stage Validations - 2026-08-02

### Branch And Worktree

- `git branch --show-current`: passed.
  - Result: `phase/4-architecture-modernization`.
- `git status --short`: passed before changes.
  - Result: no entries.
- Branch in `.sdd/sonarcloud-integration/context.md`: `phase/4-architecture-modernization`.
- Branch comparison result: matched.
- Branch checkout performed: no.

### Workflow Syntax

- `actionlint .github/workflows/ci.yml`: not executed because `actionlint` is not installed in the local environment.
- Python YAML parser validation: passed.
  - Command used: `python -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml', encoding='utf-8')); print('PYTHON_YAML_OK')"`
  - Result: `PYTHON_YAML_OK`.

### Required Commands

- `git diff --check`: passed.
- `dotnet --info`: passed.
  - SDK selected by `global.json`: `10.0.302`.
  - Host runtime: `10.0.10`.
- `dotnet restore WebApiCoreSeed.slnx`: passed.
  - Result: all projects were up to date for restore.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passed.
  - Result: build succeeded with `30` warnings and `0` errors.
  - Warnings were pre-existing analyzer warnings in application projects and were not changed in this prompt.

## Not Executed Locally

- SonarScanner was not executed locally because no real `SONAR_TOKEN` should be used in the local environment for this implementation prompt.
- Unit and integration tests were not executed locally in this prompt because the mandatory validation set requested restore and build; CI is configured to run both test suites with TRX and OpenCover after `sonarscanner begin`.
- OpenCover and TRX discovery were not confirmed locally; they must be confirmed in GitHub Actions after the workflow runs with the configured secret.
- Quality Gate success, Quality Gate failure behavior, PR decoration and analysis visibility were not confirmed locally because they depend on SonarCloud and GitHub external configuration.

## External Validation Still Pending

- Confirm SonarCloud project import or binding for `rodri-oliveira-dev/web-api-core-seed`.
- Confirm project key `rodri-oliveira-dev_web-api-core-seed`.
- Configure GitHub secret `SONAR_TOKEN`.
- Disable SonarCloud automatic analysis.
- Run the GitHub Actions workflow on `main`, `phase/4-architecture-modernization` or a pull request targeting `main`.
- Confirm `TestResults/**/*.trx` and `TestResults/**/coverage.opencover.xml` are produced in CI.
- Confirm `dotnet sonarscanner begin` accepts the configured properties.
- Confirm `dotnet sonarscanner end` uploads analysis and waits for the Quality Gate.
- Confirm a red Quality Gate or Quality Gate timeout fails the job.
- Confirm existing OpenAPI and package validations still run after a successful Quality Gate.
- Configure branch protection and required status checks after the first successful workflow run.

## Prompt 3 Coverage Validation - 2026-08-02

### Branch And Worktree

- `git branch --show-current`: passed.
  - Result: `phase/4-architecture-modernization`.
- `git status --short`: passed before changes.
  - Result: no entries.
- Branch in `.sdd/sonarcloud-integration/context.md`: `phase/4-architecture-modernization`.
- Branch comparison result: matched.
- Branch checkout performed: no.

### Test Project Review

- `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj` uses `Microsoft.NET.Test.Sdk`, `coverlet.collector`, `xunit` and `xunit.runner.visualstudio`.
- `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj` uses `Microsoft.NET.Test.Sdk`, `coverlet.collector`, `xunit` and `xunit.runner.visualstudio`.
- Both test projects inherit `TargetFramework=net10.0` from `Directory.Build.props`.
- Package versions are centralized in `tests/Directory.Packages.props`, which imports root `Directory.Packages.props`.
- No duplicate package versions were added to the test projects.

### Commands Executed

```text
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
```

### Test Results

- Unit tests: `53` total, `53` passed, `0` failed, `0` skipped.
- Integration tests: `42` total, `42` passed, `0` failed, `0` skipped.
- Combined result: `95` total, `95` passed, `0` failed.

### Reports Found

Latest direct reports generated after the corrected commands:

```text
TestResults\Unit\unit-tests.trx
TestResults\Integration\integration-tests.trx
TestResults\Unit\4bbb1ee6-7118-4cc9-8f82-7159ccae2e22\coverage.cobertura.xml
TestResults\Unit\4bbb1ee6-7118-4cc9-8f82-7159ccae2e22\coverage.opencover.xml
TestResults\Integration\7a629b8a-bd9d-40c9-8ee6-14a0cdc48a08\coverage.cobertura.xml
TestResults\Integration\7a629b8a-bd9d-40c9-8ee6-14a0cdc48a08\coverage.opencover.xml
```

`TestResults/` also contained older local artifacts because a local cleanup attempt was rejected by the executor policy. The workflow now removes and recreates `TestResults` before test execution to avoid stale report import in CI or reused workspaces.

### OpenCover XML Validation

- Unit OpenCover:
  - File: `TestResults\Unit\4bbb1ee6-7118-4cc9-8f82-7159ccae2e22\coverage.opencover.xml`
  - Size: `1093730` bytes.
  - Modules: `4`.
  - Module names: `WebApiCoreSeed.Api`, `WebApiCoreSeed.Identity.Infrastructure`, `WebApiCoreSeed.SampleRestaurant`, `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
  - Classes: `178`.
  - Sequence points: `3772`.
  - Covered sequence points: `1205`.
  - Uncovered sequence points: `2567`.
  - Missing source paths: `0`.
- Integration OpenCover:
  - File: `TestResults\Integration\7a629b8a-bd9d-40c9-8ee6-14a0cdc48a08\coverage.opencover.xml`
  - Size: `1093421` bytes.
  - Modules: `4`.
  - Module names: `WebApiCoreSeed.Api`, `WebApiCoreSeed.Identity.Infrastructure`, `WebApiCoreSeed.SampleRestaurant`, `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
  - Classes: `178`.
  - Sequence points: `3772`.
  - Covered sequence points: `2731`.
  - Uncovered sequence points: `1041`.
  - Missing source paths: `0`.

### Issues Found And Corrections

- Initial local OpenCover reports were generated successfully but unit coverage included stale test assemblies `Pedidos.Test` and `WebApiCoreSeed.Tests` from the output directory.
- Initial local OpenCover path validation also found generated OpenAPI source paths under `obj` that were not useful coverage targets.
- Corrected the workflow test commands to pass Coverlet Collector filters for test assemblies and `**/*.generated.cs`.
- Corrected `sonar.cs.opencover.reportsPaths` from broad recursive matching to suite-scoped direct report matching.
- Corrected `sonar.cs.vstest.reportsPaths` to the stable TRX file names.
- Corrected artifact upload paths to use the same real unit/integration output structure.
- Added a workflow step to recreate `TestResults/Unit` and `TestResults/Integration` before tests.

### Exclusions Review

- Adopted SonarCloud coverage exclusions remain limited to `tests/**`, `tools/**`, `**/Migrations/**`, `**/*ModelSnapshot.cs` and `**/*.Designer.cs`.
- Adopted SonarCloud duplication exclusions remain limited to `**/Migrations/**`, `**/*ModelSnapshot.cs`, `**/*.Designer.cs`, `docs/openapi/**/*.json` and `**/packages.lock.json`.
- Added Coverlet Collector exclusions for test assemblies and `**/*.generated.cs` at coverage collection time.
- Rejected broad `sonar.exclusions`, `sonar.test.inclusions` and `sonar.test.exclusions` because there was no evidence requiring them.
- Rejected exclusions for controllers, handlers, services, repositories, entities, value objects, infrastructure and low-coverage code.

### Not Executed

- SonarScanner `begin` and `end` were not executed locally because no real `SONAR_TOKEN` should be used in the local environment.
- SonarCloud upload, Quality Gate result, branch analysis and PR decoration remain external validations.

### Final Validation Before Commit

- `git status --short`: showed only `.github/workflows/ci.yml` and SDD files modified; `TestResults/` was not listed.
- `git diff --check`: passed.
- Python YAML parser validation for `.github/workflows/ci.yml`: passed with `PYTHON_YAML_OK`.
- `actionlint .github/workflows/ci.yml`: not executed because `actionlint` is not installed.
- `dotnet restore WebApiCoreSeed.slnx`: passed.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passed with `0` warnings and `0` errors in the final run.
- Final unit test run with TRX and OpenCover: `53` passed, `0` failed.
- Final integration test run with TRX and OpenCover: `42` passed, `0` failed.
- `git status --short --ignored=matching TestResults`: returned `!! TestResults/`, confirming the generated reports are ignored.
