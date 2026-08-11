# Handoff - Phase 1

## Preserved State

Phase 1 preserves the legacy .NET Core 3.1 implementation of `rodri-oliveira-dev/web-api-core-seed`.

The preserved state intentionally includes the original runtime target, package versions, project structure, migrations, configuration, tests, and known limitations. No modernization work is included in Phase 1.

## Planned And Local References

| Reference | Purpose | Expected target |
| --- | --- | --- |
| `phase/1-preserve-legacy` | Phase 1 delivery branch | Final Phase 1 commit |
| `legacy/netcoreapp3.1` | Permanent local branch for the unsupported legacy line | Final Phase 1 commit |
| `v1.0.0-legacy` | Annotated local tag for the unsupported legacy version | Final Phase 1 commit |

Remote publication remains pending:

```powershell
git push origin phase/1-preserve-legacy
git push origin legacy/netcoreapp3.1
git push origin v1.0.0-legacy
```

The Phase 1 pull request should include:

```text
Closes #3
```

## Documentation Available

- `README.md`: visible unsupported-runtime notice and link to `LEGACY.md`.
- `LEGACY.md`: legacy runtime, execution, migrations, seed state, validation notes, troubleshooting, and security notice.
- `.sdd/phase-1/README.md`: SDD operating guide for Phase 1.
- `.sdd/phase-1/baseline.md`: repository, solution, dependency, runtime, migration, configuration, and validation baseline.
- `.sdd/phase-1/decisions.md`: accepted and deferred preservation decisions.
- `.sdd/phase-1/preservation.md`: final preservation criteria, Git reference policy, verification commands, and publication commands.
- `.sdd/phase-1/status.md`: final local status and pending publication/PR items.

## Validation Results

| Command | Result | Meaning |
| --- | --- | --- |
| `dotnet restore` | Failed | Blocked by invalid local NuGet metadata for `microsoft.netcore.targets/1.1.0`; .NET SDK also reports `netcoreapp3.1` as unsupported. |
| `dotnet build --no-restore` | Failed | Missing `project.assets.json` files after restore failure; one project may compile only because partial assets exist locally. |
| `dotnet test --no-build` | Inconclusive | Exit code `0` with no output; not considered a real test pass because build output was unavailable. |

These results are preserved as legacy validation facts. They should be re-run in a compatible environment after the local NuGet cache and .NET Core 3.1 tooling limitations are handled outside the legacy preservation commit.

## Known Limitations

- .NET Core 3.1 is unsupported and no longer receives security updates.
- The current validation machine does not have .NET Core 3.1 SDK/runtime installed.
- The active SDK is newer and emits unsupported-target warnings for `netcoreapp3.1`.
- Local NuGet metadata for `microsoft.netcore.targets/1.1.0` is invalid and blocks restore.
- No `global.json` exists, although the historical README references one.
- API startup was not verified because build output was unavailable.
- No seed process or seed command was identified.
- No `launchSettings.json` was identified.
- `HealthChecks-UI` references `https://localhost:44340/hc`, but the runtime port was not verified.
- `sql/restaurante.sql` creates database `restaurante`, while the application connection string targets catalog `PedidosApi`.
- Redis Dockerfile exposes `6379`, while API settings point to `localhost:7001`.
- SQL Server, Redis, Seq, JWT, and logging settings are local and environment-specific.
- Legacy configuration contains secrets and credentials that must not be reused in new work.

## Risks Not To Fix Retroactively In Legacy

- Do not update the target framework from `netcoreapp3.1` in Phase 1.
- Do not update NuGet package versions in the preserved legacy commit.
- Do not add `global.json` retroactively to the legacy baseline.
- Do not repair migrations, seed behavior, connection strings, Docker ports, health check URLs, credentials, or local logging paths in Phase 1.
- Do not refactor application startup, hosting, dependency injection, Identity, EF Core, Redis, Seq, tests, or project layout in Phase 1.
- Do not modify the legacy branch or tag with force operations.

## Phase 2 Guidance

Start Phase 2 only after Phase 1 is integrated through the phase pull request and the local references are published as needed.

Planned Phase 2 branch:

```text
phase/2-dotnet-10-migration
```

Phase 2 should treat the branch `legacy/netcoreapp3.1` and tag `v1.0.0-legacy` as the preserved comparison points for modernization.
