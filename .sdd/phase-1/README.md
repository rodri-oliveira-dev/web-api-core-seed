# SDD Phase 1 - Legacy Preservation

## Objective

Phase 1 preserves the current legacy version of `rodri-oliveira-dev/web-api-core-seed` before any modernization work. This phase records the real state of the .NET Core 3.1 solution, validates what can be validated without changing application code, and prepares shared context for later chats.

Related GitHub issue: `#3`.

## Working Branch

Expected phase branch: `phase/1-preserve-legacy`.

The legacy source commit captured before this delivery is recorded as `legacy_source_sha` in `status.md`, `baseline.md`, and `handoff.md`.

## Prompt Order

```text
01 - Registrar baseline
02 - Documentar a versao legada
03 - Finalizar preservacao e criar referencias Git
```

## Shared Files

All context for Phase 1 must be persisted only in this folder:

```text
.sdd/phase-1/
```

Shared files:

- `README.md`: phase rules, scope, and operating guide.
- `status.md`: current phase status, validation status, blockers, and next action.
- `decisions.md`: durable decisions for the preservation phase.
- `baseline.md`: discovered repository, solution, dependency, runtime, and validation baseline.
- `handoff.md`: concise context for the next chat.

## Execution Rules

- Read this entire folder before making changes in any later Phase 1 chat.
- Preserve the legacy project without functional changes.
- Do not modernize, fix, update, refactor, or reformat application code during Phase 1 unless a later prompt explicitly changes the scope.
- Do not discard, overwrite, reset, checkout, clean, or stash unrelated existing worktree changes.
- Do not push from any Phase 1 prompt.
- Do not create `v1.0.0-legacy` or `legacy/netcoreapp3.1` until Prompt 03.
- Create textual, versionable artifacts only.
- Keep all Phase 1 shared context under `.sdd/phase-1/`.

## Completion Criteria

Phase 1 is complete when:

- The legacy baseline is recorded.
- The legacy documentation is complete enough for modernization planning.
- Validation results and environment limitations are documented.
- The planned legacy tag and branch are created only in Prompt 03.
- The working tree is clean after each delivery commit.

## Commit Convention

Prompt 01 commit message:

```text
docs: record legacy project baseline
```

Later prompts should use concise documentation commit messages unless their prompt specifies an exact message.

## Required Notice For Later Chats

Every later chat in Phase 1 must read all files in `.sdd/phase-1/` in full before acting. No later chat should rely on previous conversation history.
