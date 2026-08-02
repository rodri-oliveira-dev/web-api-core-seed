# Validation - Prompt 05 SLNX Migration

## Required SLNX Commands

| Command | Result |
| --- | --- |
| `dotnet restore WebApiCoreSeed.slnx` | Passed; all projects up to date for restore. |
| `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore` | Passed; 30 existing `CA*` warnings, 0 errors. |
| `dotnet test WebApiCoreSeed.slnx --configuration Release --no-build` | Passed; 53 unit tests and 42 integration tests. |
| `dotnet sln WebApiCoreSeed.slnx list` | Passed; 7 active projects listed. |

## Additional Test Gates

| Command | Result |
| --- | --- |
| `dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build` | Passed; 53 tests. |
| `dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "Category=Integration"` | Passed; 42 tests. |
| `dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --filter "Architecture=ModularHexagonal"` | Passed; 7 architecture tests. |

## OpenAPI

| Command | Result |
| --- | --- |
| `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build` | Passed; regenerated `docs/openapi/openapi-v1.json` and `docs/openapi/openapi-v2.json`. |
| `Get-ChildItem docs/openapi/openapi-v*.json \| ConvertFrom-Json` | Passed; generated JSON is syntactically valid. |
| `git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json` after staging regenerated contracts | Passed; worktree content matches the staged generated contracts. |

The OpenAPI generator removed obsolete `nullable: true` entries from selected schemas. The generated files were kept in the commit so the CI synchronization gate will not produce a contract diff.

## Hooks And Workflows

| Command | Result |
| --- | --- |
| `scripts/setup/configure-git-hooks.ps1 -Check` | Passed; `core.hooksPath` is configured as `.githooks`. |
| `C:\Program Files\Git\bin\sh.exe .githooks/pre-push` | Passed; restore, build and tests completed using `WebApiCoreSeed.slnx`. |
| `python -c "... yaml.safe_load(...)"` for `.github/workflows/*.yml` | Passed; workflow YAML parsed successfully with PyYAML. |
| `dotnet list WebApiCoreSeed.slnx package --vulnerable` | Passed; no vulnerable packages reported. |
| `dotnet list WebApiCoreSeed.slnx package --deprecated` | Passed; known `xunit` `2.9.3` `Legacy` warning remains in unit and integration tests. |

`sh` was not available directly in the PowerShell PATH, so the hook was executed through Git for Windows at `C:\Program Files\Git\bin\sh.exe`.

## Reference And Diff Checks

| Command | Result |
| --- | --- |
| `git grep -n -E 'WebApiCoreSeed\.sln([^x]\|$)'` | Remaining tracked matches are historical SDD entries from prior prompts. After adding Prompt 05 docs, additional matches document the required discovery/migration commands and are not active operational references. |
| `rg -n "WebApiCoreSeed\.sln([^x]\|$)" .agents .github .githooks .vscode scripts docs AGENTS.md README.md web-api-core-seed.code-workspace src tests tools` | No active matches. |
| JSON validation for `.vscode/settings.json`, `.vscode/tasks.json`, `web-api-core-seed.code-workspace` | Passed. |
| `git diff --check` | Passed; reported only the expected LF normalization warning for `README.md`. |
