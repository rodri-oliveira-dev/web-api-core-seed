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

## Validado localmente - 2026-08-02

### Branch and worktree

- `git branch --show-current`: passed.
  - Result: `phase/4-architecture-modernization`.
- `git status --short`: passed before changes.
  - Result: no entries.
- Branch in `.sdd/sonarcloud-integration/context.md`: `phase/4-architecture-modernization`.
- Branch comparison result: matched.
- Branch checkout performed: no.

### Workflow review

- Confirmed `push` events for `main` and `phase/4-architecture-modernization`.
- Confirmed `pull_request` events targeting `main`.
- Confirmed `workflow_dispatch`.
- Confirmed `actions/checkout@v4` with `fetch-depth: 0`.
- Confirmed SDK setup based on `global.json`.
- Confirmed NuGet cache and separate SonarCloud cache.
- Confirmed reproducible SonarScanner installation with explicit version `11.2.1`.
- Confirmed use of `secrets.SONAR_TOKEN` only in scanner steps.
- Confirmed no secret values are versioned in the workflow.
- Confirmed `sonarscanner begin`, build after `begin`, tests before `end`, OpenCover/TRX paths, `sonarscanner end`, Quality Gate wait and explicit timeout.
- Confirmed artifact upload steps use `if: always()`.
- No workflow correction was required in Prompt 4.

### Commands

- `dotnet --info`: passed.
  - SDK selected by `global.json`: `10.0.302`.
  - Host runtime: `10.0.10`.
- `dotnet restore WebApiCoreSeed.slnx`: passed.
  - Result: all projects were up to date for restore.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passed.
  - Result: build succeeded with `30` warnings and `0` errors.
  - Warnings were analyzer warnings in existing application code and were not changed in this prompt.
- Unit tests with TRX and OpenCover: passed.
  - Command used the same Coverlet settings as the workflow.
  - Result: `53` passed, `0` failed, `53` total.
  - TRX: `TestResults\Unit\unit-tests.trx`.
  - OpenCover: `TestResults\Unit\d84f6056-cf8f-44cd-aed6-86f7c86ebe04\coverage.opencover.xml`.
- Integration tests with TRX and OpenCover: passed.
  - Command used the same Coverlet settings as the workflow.
  - Result: `42` passed, `0` failed, `42` total.
  - TRX: `TestResults\Integration\integration-tests.trx`.
  - OpenCover: `TestResults\Integration\fbb6d855-bef7-4b37-ae26-ed0eb8fc197c\coverage.opencover.xml`.
- OpenCover XML inspection: passed.
  - Unit report: `1093730` bytes, `4` modules, `178` classes, `3772` sequence points, `1205` covered sequence points.
  - Integration report: `1093421` bytes, `4` modules, `178` classes, `3772` sequence points, `2731` covered sequence points.
- OpenAPI generation: passed.
  - Command: `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build`.
  - Generated `docs\openapi\openapi-v1.json` and `docs\openapi\openapi-v2.json`.
- OpenAPI JSON validation: passed.
  - `openapi-v1.json`: parsed successfully.
  - `openapi-v2.json`: parsed successfully.
- OpenAPI synchronization: passed.
  - `git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json` returned exit code `0`.
- Vulnerable package audit: passed.
  - `dotnet list WebApiCoreSeed.slnx package --vulnerable` found no vulnerable packages from the configured sources.
- Deprecated package report: passed with findings.
  - `dotnet list WebApiCoreSeed.slnx package --deprecated` found `xunit 2.9.3` marked as `Legacy` in unit and integration test projects.
  - The command returned exit code `0`; package migration is outside this prompt.
- YAML validation with local parser: passed.
  - Command: `python -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml', encoding='utf-8')); print('PYTHON_YAML_OK')"`
  - Result: `PYTHON_YAML_OK`.
- `actionlint .github/workflows/ci.yml`: not executed because `actionlint` is not installed locally.
- `TestResults/` ignore status: passed.
  - `git status --short --ignored=matching TestResults` returned `!! TestResults/`.
- Final diff review: passed.
  - `git diff --check` returned exit code `0`.
  - `git diff -- .github/workflows/ci.yml` produced no output because Prompt 4 made no workflow changes.
  - `git diff -- docs/quality/sonarcloud.md`, `git diff -- README.md` and `git diff -- .sdd/sonarcloud-integration` were reviewed.
- Secret review: passed.
  - Diff search found only secret names and documentation text such as `SONAR_TOKEN`; no token value was added.
- Branch review: passed.
  - Final branch remained `phase/4-architecture-modernization`.

### Local limitations

- A local attempt to remove and recreate `TestResults/` was blocked by the executor policy, so tests were run without forced cleanup. The workflow itself still removes and recreates `TestResults` before tests on Linux.
- SonarScanner was not executed locally because no real `SONAR_TOKEN` should be used in this workspace.

## Dependente de execucao no GitHub

- Runner initialization on `ubuntu-latest`.
- Availability of `secrets.SONAR_TOKEN`.
- NuGet cache restore/save behavior.
- SonarCloud cache restore/save behavior.
- Artifact publication for test results, coverage results and OpenAPI contracts.
- Actual workflow check publication in GitHub.
- Exact check names available for branch protection selection.
- Linux execution of the PowerShell OpenAPI JSON validation step.

## Dependente do SonarCloud

- Project import or repository binding under organization `rodri-oliveira-dev`.
- Analysis receipt by SonarCloud.
- OpenCover coverage import.
- TRX test report import.
- Branch analysis for `phase/4-architecture-modernization`, subject to plan/configuration.
- Pull request analysis.
- Quality Gate processing result.
- Quality Gate failure behavior in a real run.
- Pull request decoration.

## Dependente de configuracao administrativa

- Import the project in SonarCloud.
- Confirm project key `rodri-oliveira-dev_web-api-core-seed`.
- Keep `main` as the SonarCloud main branch.
- Disable SonarCloud automatic analysis.
- Create or rotate the SonarCloud analysis token.
- Create GitHub secret `SONAR_TOKEN`.
- Associate the chosen Quality Gate.
- Configure New Code definition.
- Configure GitHub branch protection rulesets for `main`.
- Select required GitHub status checks after the first workflow execution.

## External Validation Still Pending

- Confirm analysis is visible in SonarCloud.
- Confirm a green Quality Gate lets the workflow pass.
- Confirm a red Quality Gate or Quality Gate timeout fails the job.
- Confirm PR decoration.
- Confirm branch protection blocks merge when required checks or the Quality Gate fail.
