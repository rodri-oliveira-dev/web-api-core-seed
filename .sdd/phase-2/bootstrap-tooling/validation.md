# Validation - Bootstrap Tooling

## Git State

| Check | Result |
| --- | --- |
| `git status --short` before edits | Passed; working tree was clean. |
| `git branch --show-current` | `phase/2-dotnet-10-migration`. |
| `git diff --check` | Passed; only Windows LF-to-CRLF warnings were emitted. |
| `git diff --stat` | Reviewed before staging; final staged stat reviewed in Delivery. |

## JSON

Validated with PowerShell `ConvertFrom-Json`:

- `.vscode/extensions.json`
- `.vscode/settings.json`
- `.vscode/tasks.json`
- `.vscode/launch.json`
- `web-api-core-seed.code-workspace`

Result: passed.

## YAML

- `actionlint`: not installed; not installed automatically.
- Python with PyYAML was available and parsed:
  - `.github/dependabot.yml`
  - `.github/workflows/dependency-review.yml`

Result: structural YAML parsing passed.

## Shell

- `sh`: not available in this environment.
- `bash`: command exists through a broken WSL bridge but failed because `/bin/bash` is unavailable.

Result: POSIX syntax validation with `sh -n` was blocked by environment.

## PowerShell

- `pwsh`: not available.
- Windows PowerShell parser validated `scripts/setup/configure-git-hooks.ps1`.

Result: parser validation passed through Windows PowerShell.

## Git Hooks

- `scripts/setup/configure-git-hooks.ps1` configured local `core.hooksPath=.githooks`.
- `scripts/setup/configure-git-hooks.ps1 -Check` passed.
- `git config --local --get core.hooksPath` returned `.githooks`.
- Hook executable mode is validated after staging with `git ls-files --stage .githooks/pre-push`.

## Contamination Checks

Used `rg` before staging because the new files were still untracked. No occurrences were found in active artifacts for the source-specific forbidden terms requested by the prompt.

No occurrences were found in active artifacts for Sonar-related terms. Historical Sonar references exist only in `.sdd/phase-2/` as exclusion records; no Sonar automation or active configuration was imported.

After staging, the equivalent `git grep` checks are run again.

## Skills

Validated:

- every copied `SKILL.md` has frontmatter with `name` and `description`;
- skill names are kebab-case;
- selected skills do not claim future resources are already implemented;
- local paths referenced by the skills exist or are explicitly described as future/conditional;
- required third-party notice for `test-anti-patterns` exists.

## Baseline .NET Commands

### `dotnet restore`

Result: blocked.

Reason:

```text
invalid local NuGet metadata for microsoft.netcore.targets/1.1.0
```

The SDK also warned that `netcoreapp3.1` is unsupported.

### `dotnet build --no-restore`

Result: blocked.

Reason: restore did not generate required `project.assets.json` files for API, Data and test projects. The SDK also warned that `netcoreapp3.1` is unsupported.

### `dotnet test --no-build`

Result: inconclusive.

Reason: command returned exit code `0` with no output after restore/build were blocked.

## Final Scope Checks

- Staged `git diff --cached --check` passed after removing trailing whitespace in the PR template.
- `git diff --cached --stat` reviewed.
- `git diff --cached` reviewed.
- `git grep` over active artifacts found no source-specific forbidden terms requested by the prompt.
- `git grep` over active artifacts found no Sonar-related terms.
- `git ls-files --stage .githooks/pre-push` returned mode `100755`.
- No staged files under `src/` or `test/`.
- No staged `.cs`, `.csproj`, migration, `appsettings`, SQL, `web.config` or solution files.
- Temporary source clone removal confirmed.
- Current branch confirmed as `phase/2-dotnet-10-migration`.
