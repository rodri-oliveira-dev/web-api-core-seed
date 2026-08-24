# Validation - CI Recovery

## Planned Local Validation

- YAML parser validation for `.github/workflows/*.yml`.
- `actionlint`, if installed locally.
- `dotnet restore WebApiCoreSeed.slnx`.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`.
- Unit tests with TRX and Cobertura/OpenCover coverage.
- Integration tests with Testcontainers, TRX and Cobertura/OpenCover coverage.
- OpenAPI generation.
- JSON validation for `docs/openapi/openapi-v*.json`.
- OpenAPI sync verification with `git diff --exit-code`.
- `dotnet list WebApiCoreSeed.slnx package --vulnerable`.
- `git diff --check`.

## Planned Remote Validation

- Trusted pull request with independent gates green.
- SonarCloud Quality Gate executed for the trusted pull request.
- Dependabot-safe behavior documented and confirmed by workflow conditions.
- CodeQL remains green.
- Dependency Review remains green.
- Artifact uploads do not fail when matching files do not exist.

## Local Results - 2026-08-24

| Validation | Result |
| --- | --- |
| YAML parser for `.github/workflows/*.yml` | passed with `PYTHON_YAML_OK` |
| `actionlint` | not available locally |
| `dotnet restore WebApiCoreSeed.slnx` | passed |
| `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore` | passed with 1 existing analyzer warning `AV0016` |
| Unit tests with TRX and Cobertura/OpenCover | passed: 93 tests |
| Integration tests with Testcontainers, TRX and Cobertura/OpenCover | passed: 45 tests |
| OpenAPI generation | passed |
| OpenAPI JSON validation | passed with `OPENAPI_JSON_OK` |
| OpenAPI sync verification | passed |
| `dotnet list WebApiCoreSeed.slnx package --vulnerable` | passed; no vulnerable packages found from configured sources |
| `dotnet list WebApiCoreSeed.slnx package --deprecated` | passed; `xunit` 2.9.3 remains deprecated/Legacy in test projects |
| `git diff --check` | passed; emitted only the pre-existing CRLF warning for `tools/OpenApiGenerator/packages.lock.json` |
| Secret/dangerous trigger scan | no `pull_request_target`; no `continue-on-error`; only secret name references found |

## Generated Files

- Unit TRX: `TestResults/Unit/unit-tests.trx`.
- Integration TRX: `TestResults/Integration/integration-tests.trx`.
- Unit coverage: `TestResults/Unit/ff70a10c-d8a2-46cf-be76-226e831cf7b5/coverage.cobertura.xml` and `coverage.opencover.xml`.
- Integration coverage: `TestResults/Integration/76529806-c92d-406d-b8a9-3f0c4112587d/coverage.cobertura.xml` and `coverage.opencover.xml`.

## Limitations

- Local recursive cleanup of `TestResults` was blocked by executor policy. Tests still produced fresh TRX and coverage files.
- SonarCloud scanner was not executed locally because no real token should be used in the workspace.

## Remote Results - 2026-08-24

Pull request:

```text
https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/29
```

Workflow runs:

- CI: `https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/runs/32768850910`
- CodeQL: `https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/runs/32768850764`
- Dependency Review: `https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/runs/32768851012`

Remote check results:

| Check | Result |
| --- | --- |
| `Build, test and quality gates` | passed |
| `SonarCloud Quality Gate` | passed |
| `CodeQL analysis` | passed |
| `Review dependency changes` | passed |
| GitHub code scanning `CodeQL` status | passed |

Artifacts in CI run `32768850910`:

- `test-results`: uploaded.
- `coverage-results`: uploaded.
- `openapi-contracts`: uploaded.

SonarCloud PR `#29` Quality Gate API result:

- Status: `OK`.
- `new_reliability_rating`: OK.
- `new_security_rating`: OK.
- `new_maintainability_rating`: OK.
- `new_duplicated_lines_density`: `0.0`, threshold `3`, OK.
- `new_security_hotspots_reviewed`: `100.0`, threshold `100`, OK.
- No `new_coverage` condition was returned for PR `#29` because the pull request changes workflow and documentation files, not C# product code.

Dependabot behavior was not re-triggered remotely because no new Dependabot pull request was created during this recovery. It was validated by prior logs from PRs `#25`, `#26` and `#27`, and by the new workflow context classifier that skips Dependabot pull requests before reading or using `SONAR_TOKEN`.
