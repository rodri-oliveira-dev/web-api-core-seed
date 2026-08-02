# Tasks - Prompt 05 SLNX Migration

- [x] Confirm branch `phase/4-architecture-modernization`.
- [x] Confirm clean worktree before changes.
- [x] Run `dotnet sln WebApiCoreSeed.sln list`.
- [x] Run `git grep -n -E 'WebApiCoreSeed\.sln([^x]|$)'`.
- [x] Run `dotnet sln WebApiCoreSeed.sln migrate`.
- [x] Compare `.sln` and `.slnx` project lists.
- [x] Remove `WebApiCoreSeed.sln` with `git rm`.
- [x] Update active solution references to `WebApiCoreSeed.slnx`.
- [x] Validate restore, build and tests with `WebApiCoreSeed.slnx`.
- [x] Validate unit, integration, architecture and OpenAPI generator commands.
- [x] Validate hook commands and workflow syntax.
- [x] Run final reference grep.
- [x] Update status, decisions, handoff and report.
- [x] Review diff and create semantic commit.
