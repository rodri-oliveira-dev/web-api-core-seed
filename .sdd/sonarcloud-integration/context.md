# Context - SonarCloud Integration

## Objective

Define the SDD specification for integrating `rodri-oliveira-dev/web-api-core-seed` with SonarCloud through GitHub Actions.

This stage is documentation-only. It must not change runtime code, test code, workflow behavior, tokens, secrets or branch state.

## Repository And Branch

| Field | Value |
| --- | --- |
| Repository | `rodri-oliveira-dev/web-api-core-seed` |
| Current branch at start | `phase/4-architecture-modernization` |
| Branch checkout performed | no |
| Main branch expected by current CI | `main` |
| Modernization branch to consider | `phase/4-architecture-modernization` |

Initial required commands executed:

```text
git branch --show-current
git status --short
```

Observed output:

```text
phase/4-architecture-modernization
```

`git status --short` produced no entries at the start of this stage.

## Files Inspected

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

## Current Project State

The active solution is `WebApiCoreSeed.slnx`.

The SDK is pinned by `global.json`:

```text
10.0.302
```

The local SDK command returned:

```text
dotnet --version
10.0.302
```

`Directory.Build.props` sets:

- `TargetFramework`: `net10.0`
- `Nullable`: `enable`
- `ImplicitUsings`: `enable`
- `AnalysisLevel`: `latest-recommended`
- `EnforceCodeStyleInBuild`: `true`
- `Deterministic`: `true`
- `GenerateDocumentationFile`: `true`

Central package management is enabled in `Directory.Packages.props`:

- `ManagePackageVersionsCentrally`: `true`
- `RestorePackagesWithLockFile`: `true`
- `RestoreUseStaticGraphEvaluation`: `true`
- `MicrosoftDotNetPackageVersion`: `10.0.10`
- `OpenTelemetryPackageVersion`: `1.17.0`

## Solution Structure

Production projects in `WebApiCoreSeed.slnx`:

- `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`
- `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj`
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj`

Test projects:

- `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`
- `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`

Auxiliary tool project:

- `tools/OpenApiGenerator/OpenApiGenerator.csproj`

## Test Framework And Coverage

The test projects use:

- `xunit` `2.9.3`
- `xunit.runner.visualstudio` `3.1.5`
- `Microsoft.NET.Test.Sdk` `18.8.1`
- `coverlet.collector` `10.0.1`

The unit test project also uses:

- `Bogus` `35.6.5`
- `Moq` `4.20.72`
- `Microsoft.AspNetCore.Mvc.Testing`
- `Microsoft.EntityFrameworkCore.InMemory`

The integration test project also uses:

- `Microsoft.AspNetCore.Mvc.Testing`
- `StackExchange.Redis` `3.1.0`
- `Testcontainers.MsSql` `4.13.0`
- `Testcontainers.Redis` `4.13.0`

Current coverage is collected with:

```text
--collect:"XPlat Code Coverage"
```

Current coverage format is Cobertura by default:

```text
TestResults/**/coverage.cobertura.xml
```

SonarCloud should receive OpenCover reports, so the implementation stage should configure Coverlet Collector to emit OpenCover, preferably while preserving Cobertura artifacts if the existing artifact contract remains useful.

## Current Test Result Paths

Current unit test command writes:

```text
TestResults/Unit/unit-tests.trx
```

Current integration test command writes:

```text
TestResults/Integration/integration-tests.trx
```

Because `coverlet.collector` creates a run-specific subdirectory, current coverage files are expected under:

```text
TestResults/Unit/**/coverage.cobertura.xml
TestResults/Integration/**/coverage.cobertura.xml
```

Planned OpenCover paths for SonarCloud:

```text
TestResults/Unit/**/coverage.opencover.xml
TestResults/Integration/**/coverage.opencover.xml
```

## Current CI Workflow

Current workflow file:

```text
.github/workflows/ci.yml
```

Workflow name:

```text
ci
```

Current events:

- `pull_request` targeting `main`
- `push` targeting `main`

Current workflow has no `workflow_dispatch`.

Current permissions:

```text
contents: read
```

Current runner:

```text
ubuntu-latest
```

Current timeout:

```text
30 minutes
```

Current concurrency cancels previous in-progress runs for the same workflow and PR/ref.

Current cache:

- action: `actions/cache@v4`
- path: `~/.nuget/packages`
- key includes `global.json`, `Directory.Build.props` and `**/*.csproj`
- restore key: `nuget-${{ runner.os }}-`

Current commands:

```text
dotnet restore "$SOLUTION"
dotnet build "$SOLUTION" --configuration Release --no-restore
dotnet test "$UNIT_TEST_PROJECT" --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage"
dotnet test "$INTEGRATION_TEST_PROJECT" --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage"
dotnet run --project "$OPENAPI_GENERATOR_PROJECT" --configuration Release --no-build
git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json
dotnet list "$SOLUTION" package --vulnerable
dotnet list "$SOLUTION" package --deprecated
```

Current artifacts:

- `test-results`: `TestResults/**/*.trx`
- `coverage-results`: `TestResults/**/coverage.cobertura.xml`
- `openapi-contracts`: `docs/openapi/openapi-v*.json`

## Other Current Quality Workflows

`.github/workflows/codeql.yml` runs on:

- pull requests targeting `main`
- pushes to `main`
- weekly schedule

It uses `contents: read` and `security-events: write`.

`.github/workflows/dependency-review.yml` runs on pull requests targeting `main`, with documentation and image path ignores. It uses `contents: read` and `pull-requests: read`.

## Generated, Migration And Auxiliary Files

Potential generated or low-signal files for coverage and duplication:

- `src/**/Migrations/**`
- `src/**/*ModelSnapshot.cs`
- `src/**/*.Designer.cs`
- `docs/openapi/openapi-v*.json`
- `src/WebApiCoreSeed.Api/wwwroot/demo-webapi/**`
- `src/WebApiCoreSeed.Api/wwwroot/imagens/**`
- `tools/OpenApiGenerator/**`
- `**/packages.lock.json`

The API project already removes the demo and image `wwwroot` folders from compilation/content/resource/none items. These files should not become coverage targets.

`.gitignore` already excludes local analysis and generated outputs, including:

- `TestResults/`
- `*.trx`
- `coverage*.xml`
- `OpenCover/`
- `.scannerwork/`

## SonarCloud Context

Expected SonarCloud organization:

```text
rodri-oliveira-dev
```

Expected project key:

```text
rodri-oliveira-dev_web-api-core-seed2
```

This key was aligned after the maintainer provided the SonarCloud configuration URL:

```text
https://sonarcloud.io/project/configuration/AutoScan?id=rodri-oliveira-dev_web-api-core-seed2
```

The project key is not a secret. It was confirmed for this repository state by the maintainer-provided SonarCloud URL and should be rechecked if the SonarCloud project is renamed later.

Expected SonarCloud URL:

```text
https://sonarcloud.io
```

Expected GitHub secret name:

```text
SONAR_TOKEN
```

No token value is known, required or documented in this repository.

## External References Checked

- NuGet `dotnet-sonarscanner`: latest observed version `11.2.1`, compatible with computed `net10.0`.
- SonarQube Cloud documentation for GitHub Actions, CI-based analysis, Quality Gate waiting and .NET coverage import.

## Compatibility Risks

- SonarScanner for .NET must run around the build: `begin`, then build, then tests, then `end`.
- Current CI runs tests after build with `--no-build`; this is compatible with Sonar analysis only if `dotnet build` runs after `begin`.
- Current Coverlet output is Cobertura. SonarCloud .NET coverage import should use OpenCover through `sonar.cs.opencover.reportsPaths`.
- The scanner needs complete Git history for accurate blame, branch and PR analysis; current checkout uses default shallow history.
- Integration tests use Testcontainers with SQL Server and Redis, so Docker availability and test duration remain CI risks.
- Quality Gate waiting can add latency and can fail the workflow when the gate is red or processing times out.
- Branch analysis for non-main branches may depend on the SonarCloud plan.
- Pull request analysis may need `pull-requests: read` permission and the project imported/bound to GitHub.
