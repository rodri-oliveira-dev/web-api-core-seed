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
| D009 | Keep the historical README content and prepend only a visible legacy notice. | Accepted | The original README is part of the legacy record and should not be rewritten during preservation. |
| D010 | Centralize detailed legacy execution, migration, seed, limitation, and security notes in `LEGACY.md`. | Accepted | A dedicated document keeps the README notice short while preserving actionable legacy usage details. |
| D011 | Do not correct application, dependency, migration, configuration, or validation failures in Prompt 02. | Accepted | Prompt 02 is documentation-only and must preserve the unsupported .NET Core 3.1 state. |
| D012 | Document confirmed limitations explicitly instead of hiding or normalizing them. | Accepted | Later modernization needs to distinguish historical facts from environment blockers and unverified behavior. |
| D013 | Record that no seed process was identified. | Accepted | Repository search found no seed implementation, `HasData`, automatic migration/initialization, or SQL inserts. |

## Deferred Decisions

| ID | Decision Needed | Owner | Notes |
| --- | --- | --- | --- |
| TBD001 | Exact commands to create `v1.0.0-legacy` and `legacy/netcoreapp3.1`. | Prompt 03 | Must use the preserved baseline and must not move existing refs. |
| TBD002 | Whether to clean or repair local NuGet cache issues. | Later phase or local operator | Prompt 01 only records the issue; it does not modify the environment. |
| TBD003 | Whether modernization should introduce an explicit seed mechanism. | Later modernization phase | Prompt 02 records that the legacy repository does not expose a seed command. |
