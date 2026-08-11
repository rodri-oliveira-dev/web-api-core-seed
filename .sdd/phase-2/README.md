# SDD Phase 2 - Modernization To .NET 10

## Objective

Phase 2 prepares and executes the modernization of `rodri-oliveira-dev/web-api-core-seed` from the preserved .NET Core 3.1 baseline toward .NET 10.

This folder is the permanent context for Phase 2. Every prompt in this phase must read this file, `status.md`, `decisions.md`, `handoff.md`, and the prompt-specific folder before acting.

## Branch

Working branch for the phase:

```text
phase/2-dotnet-10-migration
```

Branch base for this bootstrap task:

```text
phase/1-preserve-legacy
```

Base SHA:

```text
2799562943ac03926d69bc716617d091d04ecc82
```

## Related Issues

- `#4` - Migrate the solution to .NET 10
- `#5` - Adopt the modern ASP.NET Core hosting model

## Prompt Order

```text
00 - Bootstrap de governanca e ferramentas
01 - Migrate the solution to .NET 10
02 - Adopt the modern ASP.NET Core hosting model
```

## Handoff Files

- `handoff.md`
- `status.md`
- `decisions.md`
- prompt-specific report files under `.sdd/phase-2/*/`

## Commit Rules

- Use Conventional Commits.
- Use one semantic commit per prompt.
- Do not push without explicit request.
- Update `status.md` and `handoff.md` before each prompt commit.

## Current Task

`04 - Native rate limiting`

The active solution now targets .NET 10, uses modern ASP.NET Core hosting, returns Problem Details for errors, and uses native ASP.NET Core rate limiting.
