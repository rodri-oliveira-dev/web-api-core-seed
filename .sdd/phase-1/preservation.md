# Preservation - Phase 1

## Objective

Preserve the unsupported .NET Core 3.1 version of `rodri-oliveira-dev/web-api-core-seed` before any modernization to .NET 10.

This preservation is documentation-only and Git-reference-only. It does not update target frameworks, SDK selection, package versions, source code, migrations, tests, configuration, workflows, or runtime behavior.

## Git References

| Reference | Value | Expected target |
| --- | --- | --- |
| Phase branch | `phase/1-preserve-legacy` | Final Phase 1 commit |
| Legacy branch | `legacy/netcoreapp3.1` | Final Phase 1 commit |
| Legacy tag | `v1.0.0-legacy` | Final Phase 1 commit |

During document editing, the final Phase 1 commit does not exist yet. The actual commit hash must be verified from Git after the commit is created by comparing:

```powershell
git rev-parse HEAD
git rev-parse legacy/netcoreapp3.1
git rev-parse "v1.0.0-legacy^{}"
```

All three commands must resolve to the same commit.

## Verified Criteria

| Criterion | Status | Evidence |
| --- | --- | --- |
| Phase branch is `phase/1-preserve-legacy` | Verified | `git branch --show-current` returned `phase/1-preserve-legacy`. |
| Prompt 01 commit is present | Verified | `git log -5 --oneline` includes `67f6fce docs: record legacy project baseline`. |
| Prompt 02 commit is present | Verified | `git log -5 --oneline` includes `ae63989 docs: document legacy runtime and usage`. |
| Working tree was clean before Prompt 03 edits | Verified | Initial `git status --short` returned no files. |
| Prompt 01 and Prompt 02 are completed in status | Verified | `.sdd/phase-1/status.md` marked both prompts `Completed` before this final update. |
| README contains unsupported runtime notice | Verified | `README.md` begins with a .NET Core 3.1 legacy preservation notice. |
| `LEGACY.md` exists | Verified | `LEGACY.md` documents runtime, commands, migrations, seed state, limitations, and security notes. |
| Repository remains on .NET Core 3.1 | Verified | All project files target `netcoreapp3.1`. |
| No modernization included | Verified | Prompt 03 edits are limited to `.sdd/phase-1/*`; project, code, dependency, migration, test, configuration, workflow, README, and `LEGACY.md` files are unchanged. |
| Legacy branch reference | Pending until after commit | Create or verify `legacy/netcoreapp3.1` after the final Phase 1 commit. |
| Legacy tag reference | Pending until after commit | Create or verify annotated tag `v1.0.0-legacy` after the final Phase 1 commit. |

## Legacy Documentation Files

The preserved legacy context is documented by:

- `README.md`
- `LEGACY.md`
- `.sdd/phase-1/README.md`
- `.sdd/phase-1/baseline.md`
- `.sdd/phase-1/decisions.md`
- `.sdd/phase-1/handoff.md`
- `.sdd/phase-1/preservation.md`
- `.sdd/phase-1/status.md`

## Verification Commands

Precondition and discovery commands:

```powershell
git status
git branch --show-current
git log -5 --oneline
git rev-parse HEAD
git branch --list
git tag --list
git show-ref --verify --quiet refs/heads/legacy/netcoreapp3.1
git show-ref --verify --quiet refs/tags/v1.0.0-legacy
```

Runtime and validation commands:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Diff and scope review commands:

```powershell
git status
git diff --check
git diff
```

Final reference validation commands:

```powershell
git status
git branch --show-current
git rev-parse HEAD
git rev-parse legacy/netcoreapp3.1
git rev-parse "v1.0.0-legacy^{}"
git log -3 --oneline
```

## Current Validation Results

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet restore` | Failed | Blocked by invalid local NuGet metadata for `microsoft.netcore.targets/1.1.0`; also warned that `netcoreapp3.1` is out of support. |
| `dotnet build --no-restore` | Failed | Restore assets are missing for API, Data, and test projects after restore failure; also warned that `netcoreapp3.1` is out of support. |
| `dotnet test --no-build` | Inconclusive | Returned exit code `0` with no output; not considered a meaningful pass because build output was unavailable. |

These results are recorded as legacy preservation facts. They must not be corrected retroactively in Phase 1.

## Reference Movement Policy

- Do not delete existing `legacy/netcoreapp3.1` or `v1.0.0-legacy` references.
- Do not move existing references.
- Do not use `--force`.
- If a reference already exists, resolve and compare its target with the final Phase 1 commit.
- If a reference points to another commit, stop and report the conflict.
- If a reference already points to the final Phase 1 commit, leave it unchanged and record that it already existed.
- For the annotated tag, compare the resolved commit with:

```powershell
git rev-parse "v1.0.0-legacy^{}"
```

## Local Reference Creation

After creating the final Phase 1 commit, capture the commit:

```powershell
$phase_commit_sha = git rev-parse HEAD
```

Create the legacy branch only if it does not already exist:

```powershell
git branch legacy/netcoreapp3.1 "$phase_commit_sha"
```

Create the annotated legacy tag only if it does not already exist:

```powershell
git tag -a v1.0.0-legacy "$phase_commit_sha" -m "Preserve unsupported .NET Core 3.1 version"
```

Remain on:

```text
phase/1-preserve-legacy
```

## Remote Publication Commands

Remote publication is intentionally pending. The following commands must be run later when the Phase 1 references are ready to publish:

```powershell
git push origin phase/1-preserve-legacy
git push origin legacy/netcoreapp3.1
git push origin v1.0.0-legacy
```

These push commands were not executed in Phase 1 Prompt 03.
