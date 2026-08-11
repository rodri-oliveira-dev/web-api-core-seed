# Reference Inventory - Prompt 05 SLNX Migration

## Updated Active References

| Area | Files |
| --- | --- |
| Repository guidance | `AGENTS.md`, `README.md` |
| VS Code | `.vscode/settings.json`, `.vscode/tasks.json`, `web-api-core-seed.code-workspace` |
| GitHub | `.github/CODEOWNERS`, `.github/PULL_REQUEST_TEMPLATE.md`, `.github/workflows/ci.yml`, `.github/workflows/codeql.yml` |
| Hooks | `.githooks/pre-push`, `.githooks/README.md` |
| Operational docs | `docs/quality-gates.md` |
| Local Codex skills | `.agents/skills/README.md`, `.agents/skills/dotnet-refactoring-engineer/SKILL.md`, `.agents/skills/dotnet-service-change/SKILL.md` |
| EF design-time tooling | `ApplicationDbContextFactory.cs`, `SampleRestaurantDbContextFactory.cs` |

## Historical References Intentionally Retained

The final grep may still show `WebApiCoreSeed.sln` in SDD files that document prior repository states or prior prompt validation commands, especially:

- `.sdd/phase-4/**`
- `.sdd/repository-hardening/01-repository-hygiene/**`
- `.sdd/repository-hardening/02-layout-and-namespaces/**`
- `.sdd/repository-hardening/03-central-package-management/**`
- `.sdd/repository-hardening/04-build-and-code-style/**`

These are not active commands for the current repository state.

## Expected Final State

No active file outside historical SDD should reference `WebApiCoreSeed.sln`.
