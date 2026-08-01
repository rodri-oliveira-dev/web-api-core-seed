# Status - Phase 1

## Current State

| Field | Value |
| --- | --- |
| Phase | Phase 1 - Legacy Preservation |
| Current delivery | 01 - Registrar baseline |
| Related issue | GitHub `#3` |
| Expected branch | `phase/1-preserve-legacy` |
| Current branch at delivery | `phase/1-preserve-legacy` |
| Source branch before delivery | `main` |
| `legacy_source_sha` | `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d` |
| Latest legacy commit date | `2020-09-01 08:34:26 -0300` |

## Prompt Status

| Prompt | Name | Status | Notes |
| --- | --- | --- | --- |
| 01 | Registrar baseline | Completed | SDD artifacts created and validation recorded. |
| 02 | Documentar a versao legada | Pending | Must read this folder before acting. |
| 03 | Finalizar preservacao e criar referencias Git | Pending | Must create planned tag and branch only in Prompt 03. |

## Validation Status

| Check | Status | Summary |
| --- | --- | --- |
| Initial `git status` | Passed | Working tree was clean before edits. |
| Branch preparation | Passed | `phase/1-preserve-legacy` was created from `main` at `legacy_source_sha`. |
| `dotnet --info` | Passed with limitation | .NET SDKs available, but no .NET Core 3.1 SDK/runtime installed. |
| `dotnet --list-sdks` | Passed with limitation | Available SDKs: 8.0.423, 10.0.110, 10.0.204, 10.0.302. |
| `dotnet restore` | Blocked | Failed with invalid local NuGet metadata for `microsoft.netcore.targets/1.1.0`; also warned that `netcoreapp3.1` is out of support. |
| `dotnet build --no-restore` | Blocked | Failed because asset files were missing for Data, Api, and test projects after restore failure. |
| `dotnet test --no-build` | Inconclusive | Returned exit code `0` with no output; not considered meaningful because build was not available. |
| API run command identification | Blocked after identification | Candidate command identified; `--no-build` run failed because `Restaurante.IO.Api.exe` was not present. |
| `git diff --check` | Passed | Exit code `0`; no whitespace errors reported. |
| Scope review | Passed | Only `.sdd/phase-1/` files are intended for commit. |

## Blockers

| Blocker | Status | Impact |
| --- | --- | --- |
| .NET Core 3.1 SDK/runtime absent from this machine. | Active environment limitation | May block restore/build/test/run for `netcoreapp3.1`. |
| Local NuGet cache has invalid metadata for `microsoft.netcore.targets/1.1.0`. | Active environment limitation | `dotnet list RestauranteAPI.sln package` failed while attempting restore. |

## Next Action

Prompt 02 must read all files in `.sdd/phase-1/` in full, continue documenting the legacy version, and keep preserving the codebase without modernization or functional changes.
