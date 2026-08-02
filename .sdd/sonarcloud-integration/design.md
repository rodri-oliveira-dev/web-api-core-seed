# Design - SonarCloud Integration

## Planned Flow

```text
Checkout with full history
        ↓
Setup .NET SDK
        ↓
Cache NuGet and Sonar
        ↓
Install SonarScanner
        ↓
Restore
        ↓
SonarScanner begin
        ↓
Build
        ↓
Unit tests + OpenCover + TRX
        ↓
Integration tests + OpenCover + TRX
        ↓
SonarScanner end
        ↓
Wait for Quality Gate
        ↓
Remaining existing validations
```

The Quality Gate wait should be configured as a scanner property so `SonarScanner end` blocks until the server returns the gate result or the timeout expires.

## SonarCloud Identity

Expected organization:

```text
rodri-oliveira-dev
```

Expected project key:

```text
rodri-oliveira-dev_web-api-core-seed
```

The project key is not a secret, but it must be confirmed in the SonarCloud UI before the implementation commit.

SonarCloud URL:

```text
https://sonarcloud.io
```

Secret name:

```text
SONAR_TOKEN
```

The secret value must be configured only in GitHub repository or organization secrets. It must not be written to files, command output or documentation.

## Workflow Events And Branch Strategy

Keep existing CI coverage for:

```yaml
pull_request:
  branches: ["main"]
push:
  branches: ["main"]
```

Add:

```yaml
workflow_dispatch:
```

For modernization branch analysis, prefer adding the active branch explicitly only if SonarCloud branch analysis is available:

```yaml
push:
  branches: ["main", "phase/4-architecture-modernization"]
```

If the SonarCloud plan does not support branch analysis, keep Sonar analysis focused on `main` and PR analysis.

## Pull Request Strategy

Use the normal GitHub Actions environment so SonarCloud can infer PR metadata when the project is imported from GitHub.

The workflow should expose:

```yaml
env:
  GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
  SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
```

Recommended workflow permissions:

```yaml
permissions:
  contents: read
  pull-requests: read
```

`pull-requests: read` supports PR metadata access while keeping write permissions out of the workflow.

## Scanner Version Strategy

Use SonarScanner for .NET as an explicit .NET tool version.

Current verified package version:

```text
dotnet-sonarscanner 11.2.1
```

Recommended install approach for CI:

```text
dotnet tool install --global dotnet-sonarscanner --version 11.2.1
```

Alternative: create a versioned local tool manifest in a later prompt if the repository wants scanner installation to be reproducible outside CI as well. This stage does not create that manifest.

## Cache Strategy

Keep the existing NuGet package cache:

```text
~/.nuget/packages
```

Add a separate Sonar cache:

```text
~/.sonar/cache
```

The Sonar cache key should include runner OS and scanner version, for example:

```text
sonar-${{ runner.os }}-dotnet-sonarscanner-11.2.1
```

Do not cache:

- `.scannerwork/`
- `TestResults/`
- token values
- environment files

## Restore And Build Strategy

Restore remains before scanner `begin`:

```text
dotnet restore "$SOLUTION"
```

Build must move inside the scanner cycle:

```text
dotnet sonarscanner begin ...
dotnet build "$SOLUTION" --configuration Release --no-restore
dotnet test ...
dotnet sonarscanner end ...
```

This preserves the existing build command while satisfying SonarScanner for .NET requirements.

## Test And Coverage Strategy

Keep separate unit and integration test commands.

Keep TRX log file names:

```text
unit-tests.trx
integration-tests.trx
```

Keep result directories:

```text
TestResults/Unit
TestResults/Integration
```

Configure Coverlet Collector to emit OpenCover:

```text
--collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
```

If the existing `coverage-results` artifact must keep Cobertura, use both formats:

```text
--collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover
```

Filter coverage collection to production assemblies and generated source exclusions:

```text
DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*
DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs
```

The workflow prepares `TestResults/Unit` and `TestResults/Integration` immediately before test execution so stale local or self-hosted runner artifacts cannot be imported or uploaded.

Planned TRX scanner path:

```text
TestResults/Unit/unit-tests.trx
TestResults/Integration/integration-tests.trx
```

Planned OpenCover scanner path:

```text
TestResults/Unit/*/coverage.opencover.xml
TestResults/Integration/*/coverage.opencover.xml
```

## Scanner Properties

Planned `begin` properties:

```text
/k:"rodri-oliveira-dev_web-api-core-seed"
/o:"rodri-oliveira-dev"
/d:sonar.host.url="https://sonarcloud.io"
/d:sonar.token="${{ secrets.SONAR_TOKEN }}"
/d:sonar.cs.vstest.reportsPaths="TestResults/Unit/unit-tests.trx,TestResults/Integration/integration-tests.trx"
/d:sonar.cs.opencover.reportsPaths="TestResults/Unit/*/coverage.opencover.xml,TestResults/Integration/*/coverage.opencover.xml"
/d:sonar.qualitygate.wait=true
/d:sonar.qualitygate.timeout=300
/d:sonar.coverage.exclusions="tests/**,tools/**,**/Migrations/**,**/*ModelSnapshot.cs,**/*.Designer.cs"
/d:sonar.cpd.exclusions="**/Migrations/**,**/*ModelSnapshot.cs,**/*.Designer.cs,docs/openapi/**/*.json,**/packages.lock.json"
```

Planned `end` property:

```text
/d:sonar.token="${{ secrets.SONAR_TOKEN }}"
```

## Quality Gate Timeout

Initial timeout:

```text
300 seconds
```

Rationale: this matches the commonly documented default wait window and keeps the CI timeout within the current 30-minute job budget.

If the project regularly times out waiting for analysis processing, increase to `600` seconds in a follow-up commit with evidence from GitHub Actions logs.

## Exclusion Policy

Use narrow exclusions and prefer coverage/duplication exclusions over broad source exclusions.

| Path | Property | Justification |
| --- | --- | --- |
| `tests/**` | `sonar.coverage.exclusions` | Test projects are not production coverage targets. |
| `tools/**` | `sonar.coverage.exclusions` | OpenAPI generator is an auxiliary build tool, not production API behavior. |
| `**/Migrations/**` | `sonar.coverage.exclusions`, `sonar.cpd.exclusions` | EF migrations are generated-like schema history and create duplication noise. |
| `**/*ModelSnapshot.cs` | `sonar.coverage.exclusions`, `sonar.cpd.exclusions` | EF model snapshots are generated metadata. |
| `**/*.Designer.cs` | `sonar.coverage.exclusions`, `sonar.cpd.exclusions` | Designer files are generated or generated-like code. |
| `docs/openapi/**/*.json` | `sonar.cpd.exclusions` | Generated API contracts can create duplication noise and are validated separately. |
| `**/packages.lock.json` | `sonar.cpd.exclusions` | Lock files are dependency metadata, not authored source. |

Avoid excluding hand-written production code from static analysis unless the next prompt finds a concrete false positive pattern.

## Preserving Existing Validations

Keep these current validations:

- restore
- build
- unit tests
- integration tests
- OpenAPI generation
- OpenAPI JSON parse validation
- OpenAPI synchronization diff
- vulnerable package audit
- deprecated package report
- test result artifact upload
- coverage artifact upload
- OpenAPI contract artifact upload

The implementation should place Sonar analysis around restore/build/test and leave OpenAPI/package checks in the same job after `SonarScanner end`, following the requested planned flow.

If the Quality Gate fails, later validations may not run because the job should fail closed. Artifact upload steps must remain `if: always()` so diagnostics survive test or analysis failures.

## External Configuration Documentation

The implementation prompt should also document, outside the workflow, how to:

- import the project in SonarCloud;
- disable SonarCloud automatic analysis;
- confirm project key `rodri-oliveira-dev_web-api-core-seed`;
- create `SONAR_TOKEN` with the minimum required scope;
- store `SONAR_TOKEN` as a GitHub secret;
- confirm PR decoration is enabled;
- set New Code definition;
- configure Quality Gate policy;
- protect `main` with required status checks.
