# Decisions - Phase 2

| ID | Decision | Status | Rationale |
| --- | --- | --- | --- |
| D001 | Use `AGENTS.md`, uppercase and plural. | Accepted | Codex and other agents use this file as repository guidance. |
| D002 | Do not copy the source `AGENTS.md` literally. | Accepted | The source repository has different architecture, paths and services. |
| D003 | Do not copy skills without evaluation. | Accepted | Skills must be useful and safe for this repository. |
| D004 | Do not import Sonar. | Accepted | The target governance excludes Sonar automation and active settings. |
| D005 | Do not copy personal VS Code paths. | Accepted | Workspace settings must be portable. |
| D006 | Do not copy workflows that depend on missing resources. | Accepted | Active workflows must not be structurally broken. |
| D007 | Do not copy the original `pre-push` hook literally. | Accepted | It depends on scripts and policies absent from this repository. |
| D008 | Keep hooks lightweight. | Accepted | Local hooks should help without duplicating heavy CI gates. |
| D009 | Keep heavy gates in CI. | Accepted | Future CI can run broader checks after the modern toolchain exists. |
| D010 | Do not implement .NET 10, Hexagonal architecture, Aspire or Testcontainers in task 00. | Accepted | This task only prepares governance and tooling. |
| D011 | Use `phase/2-dotnet-10-migration` as the start of Phase 2. | Accepted | Phase 1 is preserved and Phase 2 work should be isolated. |
| D012 | Create only dependency-review as active workflow now. | Accepted | It has no project build dependency and no secrets. |
| D013 | Defer full .NET CI until the .NET 10 migration prompt. | Accepted | The current .NET Core 3.1 environment is already known to be blocked locally. |
| D014 | Preserve legacy application files during bootstrap. | Accepted | This prompt must not alter C#, packages, migrations, HTTP contracts or tests. |
