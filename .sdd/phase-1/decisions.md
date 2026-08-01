# Decisions - Phase 1

## Accepted Decisions

| ID | Decision | Status | Rationale |
| --- | --- | --- | --- |
| D001 | Preserve the legacy repository state without functional changes. | Accepted | Phase 1 exists to capture the current .NET Core 3.1 baseline before modernization. |
| D002 | Modernization will happen only in later phases. | Accepted | This delivery must not update target frameworks, SDKs, packages, source code, migrations, tests, workflows, or application configuration. |
| D003 | Planned legacy tag: `v1.0.0-legacy`. | Accepted | The tag will mark the preserved legacy baseline at the end of Phase 1. |
| D004 | Planned legacy branch: `legacy/netcoreapp3.1`. | Accepted | The branch will preserve the .NET Core 3.1 line at the end of Phase 1. |
| D005 | Phase execution branch: `phase/1-preserve-legacy`. | Accepted | Phase work is isolated from the default branch. |
| D006 | No Phase 1 prompt will push to a remote. | Accepted | Remote publication is intentionally excluded from this phase. |
| D007 | Existing Git references must never be moved with `--force`. | Accepted | Preservation references must remain stable and auditable. |
| D008 | `legacy_source_sha` is `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d`. | Accepted | This was the current HEAD before Prompt 01 created documentation artifacts. |

## Deferred Decisions

| ID | Decision Needed | Owner | Notes |
| --- | --- | --- | --- |
| TBD001 | Exact commands to create `v1.0.0-legacy` and `legacy/netcoreapp3.1`. | Prompt 03 | Must use the preserved baseline and must not move existing refs. |
| TBD002 | Whether to clean or repair local NuGet cache issues. | Later phase or local operator | Prompt 01 only records the issue; it does not modify the environment. |
