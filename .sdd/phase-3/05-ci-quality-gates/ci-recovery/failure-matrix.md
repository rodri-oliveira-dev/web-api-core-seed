# Failure Matrix - CI Recovery

| Case | Evidence | Root cause | Impact | Required behavior |
| --- | --- | --- | --- | --- |
| PR `#24` run `31529967515` | Sonar begin succeeded; reports uploaded; Quality Gate failed at scanner end | Real SonarCloud Quality Gate failure | CI failed after build, tests, OpenAPI and package checks completed | Keep failing; do not hide with `continue-on-error` |
| PR `#25` | Dependabot PR; `SONAR_TOKEN` empty; scanner begin failed | Dependabot pull requests do not receive repository Actions secrets | Build, tests, OpenAPI and package checks were skipped | Skip SonarCloud safely, still run independent gates |
| PR `#26` | Dependabot PR; `SONAR_TOKEN` empty; scanner begin failed | Same as PR `#25` | Build, tests, OpenAPI and package checks were skipped | Skip SonarCloud safely, still run independent gates |
| PR `#27` run `32029331475` | `Secret source: Dependabot`; `SONAR_TOKEN:` empty; invalid `sonar.token=` | Same as PR `#25`; confirmed by logs | Build, tests, OpenAPI and package checks skipped; uploads failed for missing TRX/coverage | Skip SonarCloud safely and guard artifact uploads |
| Trusted PR without token | Inferred from current single-job design | External GitHub secret missing or unavailable | Scanner begin would fail before independent gates | Independent gates must still run; Sonar job must fail clearly |
| Trusted push with Quality Gate red | SonarCloud API current project status `ERROR` | Coverage below configured Quality Gate threshold | Sonar check should fail | Keep SonarCloud mandatory and failing |
| Upload after skipped tests | PRs `#25`, `#26`, `#27` | `if: always()` plus `if-no-files-found: error` with no generated files | Secondary failures obscure primary cause | Upload only when matching files exist |

## SonarCloud Execution Matrix

| Event | Context | Token expected | Sonar behavior | Failure policy |
| --- | --- | --- | --- | --- |
| `push` to `main` | trusted | yes | run scanner and wait for Quality Gate | fail on missing token, scanner error or red gate |
| `push` to configured internal branch | trusted | yes | run scanner and wait for Quality Gate | fail on missing token, scanner error or red gate |
| `pull_request` from same repository by maintainer | trusted | yes | run scanner and wait for Quality Gate | fail on missing token, scanner error or red gate |
| `pull_request` from Dependabot | untrusted secret context | no | skip SonarCloud with explicit notice | do not fail due missing secret |
| `pull_request` from fork | untrusted secret context | no | skip SonarCloud with explicit notice | do not use repository secrets or `pull_request_target` |

## Remote Quality Gate Metrics

| Scope | Status | Coverage threshold | Coverage actual | Duplication threshold | Duplication actual |
| --- | --- | --- | --- | --- | --- |
| PR `#24` | `ERROR` | `80` | `72.2` | `3` | `0.9` |
| Project current | `ERROR` | `80` | `75.0` | `3` | `0.0` |
