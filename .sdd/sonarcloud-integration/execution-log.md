# Execution Log - SonarCloud Integration Specification

## 2026-08-02

Branch at start:

```text
phase/4-architecture-modernization
```

Branch checkout performed:

```text
no
```

## Files Analyzed

- `global.json`
- `Directory.Build.props`
- `Directory.Packages.props`
- `WebApiCoreSeed.slnx`
- `.github/workflows/ci.yml`
- `.github/workflows/codeql.yml`
- `.github/workflows/dependency-review.yml`
- `docs/quality-gates.md`
- `src/Directory.Packages.props`
- `tests/Directory.Packages.props`
- `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`
- `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj`
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj`
- `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`
- `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`
- `tools/OpenApiGenerator/OpenApiGenerator.csproj`
- `.gitignore`

## Files Created

- `.sdd/sonarcloud-integration/context.md`
- `.sdd/sonarcloud-integration/requirements.md`
- `.sdd/sonarcloud-integration/design.md`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`

## Main Discoveries

- Active branch is `phase/4-architecture-modernization`.
- Initial worktree was clean.
- SDK is pinned to `10.0.302`.
- Active target framework is `net10.0`.
- Active solution is `WebApiCoreSeed.slnx`.
- CI currently runs only on pushes to `main` and pull requests targeting `main`.
- CI has no `workflow_dispatch`.
- CI checkout currently does not set `fetch-depth: 0`.
- CI permissions are currently only `contents: read`.
- Current NuGet cache covers `~/.nuget/packages`.
- No Sonar cache exists yet.
- Current coverage uses Coverlet Collector with `XPlat Code Coverage`.
- Current coverage artifact expects Cobertura files at `TestResults/**/coverage.cobertura.xml`.
- SonarCloud integration should generate OpenCover files at `TestResults/**/coverage.opencover.xml`.
- Current TRX files are written under `TestResults/Unit` and `TestResults/Integration`.
- Integration tests use Testcontainers for SQL Server and Redis.
- EF migrations and model snapshots exist in Identity and SampleRestaurant infrastructure projects.
- Generated OpenAPI contracts are versioned under `docs/openapi/`.
- `.gitignore` already excludes scanner work, test results, TRX and coverage files.
- `dotnet-sonarscanner` latest observed package version is `11.2.1`.

## Risks

- SonarCloud branch analysis for `phase/4-architecture-modernization` may require a paid plan.
- Quality Gate waiting can increase CI duration or fail when SonarCloud processing times out.
- Integration tests depend on Docker/Testcontainers and can dominate CI time.
- Changing coverage format from Cobertura to OpenCover could break existing coverage artifacts unless both formats are generated or artifact paths are updated.
- PR decoration may require the SonarCloud project to be imported from GitHub and the workflow to expose the right read permissions.
- The expected project key must be confirmed in the SonarCloud UI.

## Pending Items

- Import or confirm the project in SonarCloud.
- Disable SonarCloud automatic analysis.
- Configure `SONAR_TOKEN` as a GitHub secret without exposing its value.
- Implement scanner installation and scanner cycle in `.github/workflows/ci.yml`.
- Generate OpenCover coverage reports.
- Validate TRX and OpenCover discovery.
- Confirm analysis visibility and Quality Gate behavior in SonarCloud.
- Document external setup and branch protection.

## Validation Before Commit

- `git diff --check`: passed.
- `git status --short`: only `.sdd/sonarcloud-integration/` was untracked.
- `git diff -- .sdd/sonarcloud-integration`: produced no output before staging because the files were new and untracked.

## Next Prompt

Prompt 2 should implement the GitHub Actions SonarCloud integration according to this specification, without changing branches and without exposing secrets.

## Prompt 2 Implementation - 2026-08-02

Branch at start:

```text
phase/4-architecture-modernization
```

Branch checkout performed:

```text
no
```

Initial worktree status:

```text
clean
```

## Files Re-Inspected

- `.sdd/sonarcloud-integration/context.md`
- `.sdd/sonarcloud-integration/requirements.md`
- `.sdd/sonarcloud-integration/design.md`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`
- `.github/workflows/ci.yml`
- `global.json`
- `Directory.Build.props`
- `Directory.Packages.props`
- `WebApiCoreSeed.slnx`
- `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`
- `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`

## Files Updated

- `.github/workflows/ci.yml`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`

## Workflow Changes

- Added `workflow_dispatch`.
- Added push analysis for `main` and `phase/4-architecture-modernization`.
- Kept pull request analysis restricted to pull requests targeting `main`.
- Added `pull-requests: read` while preserving `contents: read`.
- Configured checkout with `fetch-depth: 0`.
- Preserved SDK setup through `global.json`.
- Preserved existing NuGet cache and added a separate SonarCloud cache at `~/.sonar/cache`.
- Installed `dotnet-sonarscanner` with explicit version `11.2.1`.
- Added non-secret Sonar values:
  - `SONAR_ORGANIZATION=rodri-oliveira-dev`
  - `SONAR_PROJECT_KEY=rodri-oliveira-dev_web-api-core-seed2`
  - `SONAR_HOST_URL=https://sonarcloud.io`
- Scoped `secrets.SONAR_TOKEN` to the scanner `begin` and `end` steps.
- Added `dotnet sonarscanner begin` before the existing build.
- Kept `dotnet build "$SOLUTION" --configuration Release --no-restore` between scanner `begin` and `end`.
- Kept separate unit and integration test commands, TRX file names and result directories.
- Configured Coverlet Collector to emit `cobertura,opencover`.
- Added `dotnet sonarscanner end` with Quality Gate enforcement after tests.
- Preserved OpenAPI generation, OpenAPI JSON validation, OpenAPI synchronization check, vulnerable package audit, deprecated package report and artifact upload steps.
- Updated the coverage artifact to include both Cobertura and OpenCover XML files.

## Scanner Properties Configured

- `sonar.host.url`
- `sonar.token`
- `sonar.qualitygate.wait=true`
- `sonar.qualitygate.timeout=300`
- `sonar.cs.opencover.reportsPaths=TestResults/**/coverage.opencover.xml`
- `sonar.cs.vstest.reportsPaths=TestResults/**/*.trx`
- `sonar.coverage.exclusions=tests/**,tools/**,**/Migrations/**,**/*ModelSnapshot.cs,**/*.Designer.cs`
- `sonar.cpd.exclusions=**/Migrations/**,**/*ModelSnapshot.cs,**/*.Designer.cs,docs/openapi/**/*.json,**/packages.lock.json`

## External Dependencies Still Pending

- Confirm the SonarCloud project exists or import it from GitHub.
- Confirm project key `rodri-oliveira-dev_web-api-core-seed2` in the SonarCloud UI.
- Disable SonarCloud automatic analysis after enabling CI analysis.
- Configure GitHub secret `SONAR_TOKEN` without exposing its value.
- Confirm branch analysis support for `phase/4-architecture-modernization`.
- Execute the workflow in GitHub Actions with the real secret.
- Confirm analysis visibility, Quality Gate behavior and PR decoration in SonarCloud.
- Configure required status checks and branch protection for `main` after the first successful run.

## Implementation Validation Results

- `git diff --check`: passed.
- `dotnet --info`: passed.
  - SDK: `10.0.302`.
  - Host runtime: `10.0.10`.
- `actionlint .github/workflows/ci.yml`: not run because `actionlint` is not installed.
- Python YAML parser validation for `.github/workflows/ci.yml`: passed.
- `dotnet restore WebApiCoreSeed.slnx`: passed.
  - Result: all projects were up to date for restore.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passed.
  - Result: build succeeded with `30` warnings and `0` errors.

## Validation Limitations

- SonarScanner was not executed locally and no real token was used.
- TRX and OpenCover output discovery must be confirmed in GitHub Actions.
- Quality Gate success, failure and timeout behavior must be confirmed after SonarCloud processing is available.
- PR decoration must be confirmed after the SonarCloud project is imported or bound to GitHub.

## Prompt 3 Coverage Validation - 2026-08-02

Branch at start:

```text
phase/4-architecture-modernization
```

Branch checkout performed:

```text
no
```

Initial worktree status:

```text
clean
```

## Files Re-Inspected

- `.sdd/sonarcloud-integration/context.md`
- `.sdd/sonarcloud-integration/requirements.md`
- `.sdd/sonarcloud-integration/design.md`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`
- `.github/workflows/ci.yml`
- `Directory.Build.props`
- `Directory.Packages.props`
- `WebApiCoreSeed.slnx`
- `tests/Directory.Packages.props`
- `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`
- `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`
- `.gitignore`

## Files Updated

- `.github/workflows/ci.yml`
- `.sdd/sonarcloud-integration/design.md`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`

## Commands Executed

```text
git branch --show-current
git status --short
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
```

## Test And Coverage Results

- Unit tests: `53` passed, `0` failed, `53` total.
- Integration tests: `42` passed, `0` failed, `42` total.
- Combined tests: `95` passed, `0` failed, `95` total.
- TRX files:
  - `TestResults\Unit\unit-tests.trx`
  - `TestResults\Integration\integration-tests.trx`
- OpenCover files from the corrected run:
  - `TestResults\Unit\4bbb1ee6-7118-4cc9-8f82-7159ccae2e22\coverage.opencover.xml`
  - `TestResults\Integration\7a629b8a-bd9d-40c9-8ee6-14a0cdc48a08\coverage.opencover.xml`
- Cobertura files from the corrected run:
  - `TestResults\Unit\4bbb1ee6-7118-4cc9-8f82-7159ccae2e22\coverage.cobertura.xml`
  - `TestResults\Integration\7a629b8a-bd9d-40c9-8ee6-14a0cdc48a08\coverage.cobertura.xml`

## Problems Found

- `TestResults/` contained older local files before this prompt's final validation. A cleanup attempt through `Remove-Item` was rejected by the executor policy, so stale files remained local but ignored by Git.
- The initial unit OpenCover report included stale test assemblies `Pedidos.Test` and `WebApiCoreSeed.Tests` from `tests/WebApiCoreSeed.UnitTests/bin/Release/net10.0`.
- The initial OpenCover path validation found generated OpenAPI source generator paths under `obj`.
- The previous broad `TestResults/**/coverage.opencover.xml` pattern could match VSTest attachment copies and duplicate direct Coverlet reports.

## Corrections Applied

- Added Coverlet Collector `Exclude` for test assembly suffixes:

```text
[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*
```

- Added Coverlet Collector `ExcludeByFile`:

```text
**/*.generated.cs
```

- Changed `sonar.cs.opencover.reportsPaths` to:

```text
TestResults/Unit/*/coverage.opencover.xml,TestResults/Integration/*/coverage.opencover.xml
```

- Changed `sonar.cs.vstest.reportsPaths` to:

```text
TestResults/Unit/unit-tests.trx,TestResults/Integration/integration-tests.trx
```

- Added a workflow step to recreate `TestResults/Unit` and `TestResults/Integration` before test execution.
- Updated coverage artifact paths to the real suite-scoped Cobertura and OpenCover report locations.

## XML Validation Results

- Corrected unit OpenCover XML:
  - Modules: `4`.
  - Classes: `178`.
  - Sequence points: `3772`.
  - Covered sequence points: `1205`.
  - Uncovered sequence points: `2567`.
  - Missing source paths: `0`.
- Corrected integration OpenCover XML:
  - Modules: `4`.
  - Classes: `178`.
  - Sequence points: `3772`.
  - Covered sequence points: `2731`.
  - Uncovered sequence points: `1041`.
  - Missing source paths: `0`.

## Exclusion Review

- Kept narrow SonarCloud coverage exclusions for tests, tools, migrations, model snapshots and designer files.
- Kept narrow SonarCloud duplication exclusions for migrations, model snapshots, designer files, generated OpenAPI JSON and lock files.
- Did not configure `sonar.exclusions`.
- Did not configure `sonar.test.inclusions`.
- Did not configure `sonar.test.exclusions`.
- Did not exclude hand-written production code such as controllers, services, repositories, domain models, value objects or infrastructure.

## Remaining External Items

- Confirm the SonarCloud project and project key in the SonarCloud UI.
- Configure `SONAR_TOKEN` in GitHub secrets without exposing its value.
- Run the workflow in GitHub Actions and confirm SonarCloud import, Quality Gate behavior, branch analysis support and PR decoration.

## Final Validation Before Commit

- `git diff --check`: passed.
- Python YAML parser validation for `.github/workflows/ci.yml`: passed.
- `actionlint .github/workflows/ci.yml`: not executed because `actionlint` is not installed.
- `dotnet restore WebApiCoreSeed.slnx`: passed.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passed with `0` warnings and `0` errors.
- Unit tests with TRX and OpenCover: `53` passed, `0` failed.
- Integration tests with TRX and OpenCover: `42` passed, `0` failed.
- `git status --short --ignored=matching TestResults`: returned `!! TestResults/`, confirming generated reports remain ignored.

## Prompt 4 Final Documentation And Quality Gate Protection - 2026-08-02

Branch at start:

```text
phase/4-architecture-modernization
```

Branch checkout performed:

```text
no
```

Initial worktree status:

```text
clean
```

## Files Re-Inspected

- `.sdd/sonarcloud-integration/context.md`
- `.sdd/sonarcloud-integration/requirements.md`
- `.sdd/sonarcloud-integration/design.md`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`
- `.github/workflows/ci.yml`
- `README.md`
- `global.json`
- `Directory.Build.props`
- `Directory.Packages.props`
- `WebApiCoreSeed.slnx`
- `docs/quality-gates.md`

## Technical Review Result

- Confirmed workflow events: `push`, `pull_request` and `workflow_dispatch`.
- Confirmed checkout uses `fetch-depth: 0`.
- Confirmed .NET SDK setup uses `global.json`.
- Confirmed NuGet and SonarCloud caches are separate.
- Confirmed SonarScanner installation is pinned to `11.2.1`.
- Confirmed `SONAR_TOKEN` is referenced only through `secrets.SONAR_TOKEN` on scanner steps.
- Confirmed no secret values were present in the workflow.
- Confirmed scanner begin/build/test/end order.
- Confirmed OpenCover and TRX report paths.
- Confirmed `sonar.qualitygate.wait=true` and explicit timeout `300`.
- Confirmed artifact upload steps use `if: always()`.
- No technical correction to `.github/workflows/ci.yml` was required in this prompt.

## Local Validation Commands

```text
git branch --show-current
git status --short
dotnet --info
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
python -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml', encoding='utf-8')); print('PYTHON_YAML_OK')"
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json
dotnet list WebApiCoreSeed.slnx package --vulnerable
dotnet list WebApiCoreSeed.slnx package --deprecated
```

## Local Validation Results

- `dotnet --info`: SDK `10.0.302`, host runtime `10.0.10`.
- Restore: passed.
- Build: passed with `30` existing analyzer warnings and `0` errors.
- Unit tests: `53` passed, `0` failed.
- Integration tests: `42` passed, `0` failed.
- Unit TRX: `TestResults\Unit\unit-tests.trx`.
- Integration TRX: `TestResults\Integration\integration-tests.trx`.
- Unit OpenCover: `TestResults\Unit\d84f6056-cf8f-44cd-aed6-86f7c86ebe04\coverage.opencover.xml`.
- Integration OpenCover: `TestResults\Integration\fbb6d855-bef7-4b37-ae26-ed0eb8fc197c\coverage.opencover.xml`.
- OpenCover XML inspection: passed for unit and integration reports.
- YAML parser validation: passed with `PYTHON_YAML_OK`.
- `actionlint`: not executed because it is not installed locally.
- OpenAPI generation: passed.
- OpenAPI JSON validation: passed for `openapi-v1.json` and `openapi-v2.json`.
- OpenAPI synchronization diff: passed.
- Vulnerable package audit: no vulnerable packages found.
- Deprecated package report: `xunit 2.9.3` is reported as `Legacy` in unit and integration test projects.

## Files Updated

- `docs/quality/sonarcloud.md`
- `docs/quality-gates.md`
- `README.md`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`

## External Items Still Pending

- Import or confirm the project in SonarCloud.
- Disable SonarCloud automatic analysis.
- Create and configure GitHub secret `SONAR_TOKEN`.
- Run GitHub Actions with the real secret.
- Confirm SonarCloud receives analysis, TRX and OpenCover reports.
- Confirm Quality Gate pass/fail behavior and timeout behavior.
- Confirm pull request decoration.
- Configure branch protection rulesets and required status checks after checks exist.

## Final Review Before Commit

- `git status --short`: showed only Prompt 4 documentation and SDD files.
- `git diff --check`: passed with exit code `0`.
- `git diff -- .github/workflows/ci.yml`: no output; no workflow changes were made in Prompt 4.
- `git diff -- docs/quality/sonarcloud.md`: reviewed after marking the new file with intent-to-add.
- `git diff -- README.md`: reviewed.
- `git diff -- docs/quality-gates.md`: reviewed.
- `git diff -- .sdd/sonarcloud-integration`: reviewed.
- `git status --short --ignored=matching TestResults`: returned `!! TestResults/`, confirming generated reports remain ignored.
- Diff search for token/secret terms found only documentation text and secret names, with no secret value.
- Final branch check remained `phase/4-architecture-modernization`.

## Project Key Alignment - 2026-08-02

The maintainer provided the SonarCloud configuration URL:

```text
https://sonarcloud.io/project/configuration/AutoScan?id=rodri-oliveira-dev_web-api-core-seed2
```

The project key was aligned from the previously expected value to:

```text
rodri-oliveira-dev_web-api-core-seed2
```

Files updated:

- `.github/workflows/ci.yml`
- `docs/quality/sonarcloud.md`
- `.sdd/sonarcloud-integration/context.md`
- `.sdd/sonarcloud-integration/design.md`
- `.sdd/sonarcloud-integration/tasks.md`
- `.sdd/sonarcloud-integration/decisions.md`
- `.sdd/sonarcloud-integration/validation.md`
- `.sdd/sonarcloud-integration/execution-log.md`

Validation:

- Python YAML parser validation for `.github/workflows/ci.yml`: passed with `PYTHON_YAML_OK`.
- `git diff --check`: passed.
- No token value was added.
- Branch checkout performed: no.
