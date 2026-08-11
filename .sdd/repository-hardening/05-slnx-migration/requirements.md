# Requirements - Prompt 05 SLNX Migration

## Objective

Migrate the active repository solution from `WebApiCoreSeed.sln` to `WebApiCoreSeed.slnx` using the official .NET SDK command.

## Acceptance Criteria

- Create `WebApiCoreSeed.slnx` with `dotnet sln WebApiCoreSeed.sln migrate`.
- Preserve all active projects.
- Preserve relevant logical solution folders.
- Avoid duplicated projects.
- Remove `WebApiCoreSeed.sln` only after confirming project equivalence.
- Restore, build and test using `WebApiCoreSeed.slnx`.
- Update active VS Code, workflow, hook, script, documentation and repository guidance references to `WebApiCoreSeed.slnx`.
- Keep historical documentation intact when it accurately documents previous repository states.
- End with no active reference to `WebApiCoreSeed.sln`.

## Explicit Non-Goals

- Do not rename the solution extension manually.
- Do not alter target frameworks, package versions or project references.
- Do not fix the historical formatting debt reported by Prompt 04.
- Do not rewrite historical SDD entries that correctly describe earlier prompts.
- Do not push.
