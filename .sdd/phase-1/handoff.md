# Handoff - Phase 1 Prompt 01

## Completed

- Started Phase 1 using SDD order.
- Confirmed clean worktree before edits.
- Captured the legacy base commit as `legacy_source_sha`.
- Created phase branch `phase/1-preserve-legacy` from `main`.
- Created the shared Phase 1 folder and SDD files.
- Recorded repository, solution, project, dependency, runtime, database, migration, external dependency, and sensitive configuration baseline.

## Files Created

- `.sdd/phase-1/README.md`
- `.sdd/phase-1/status.md`
- `.sdd/phase-1/decisions.md`
- `.sdd/phase-1/baseline.md`
- `.sdd/phase-1/handoff.md`

## Legacy Commit Base

`legacy_source_sha`: `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d`

Source branch before Prompt 01: `main`

Phase branch: `phase/1-preserve-legacy`

Latest legacy commit date: `2020-09-01 08:34:26 -0300`

## Validations Performed

- `git status --short`
- `git branch --show-current`
- `git log -5 --oneline`
- `git rev-parse HEAD`
- `dotnet --info`
- `dotnet --list-sdks`
- Repository file inventory with `rg --files`
- Relevant text search for database, Redis, Seq, migrations, and seed references
- `dotnet list RestauranteAPI.sln package` attempted and failed due local NuGet metadata issue

Additional validation completed in this delivery:

- `dotnet restore`
- `dotnet build --no-restore`
- `dotnet test --no-build`
- API run command check with `dotnet run --project src\DevIO.Api\Restaurante.IO.Api.csproj --no-build`
- `git diff --check`
- `git diff`

## Problems Found

- No `global.json` exists, although README references it.
- .NET Core 3.1 SDK/runtime is not installed in this environment.
- Active SDK is .NET `10.0.302`.
- Local NuGet metadata file is invalid for `microsoft.netcore.targets/1.1.0`.
- `dotnet restore` failed because of the invalid NuGet metadata file.
- `dotnet build --no-restore` failed because `project.assets.json` files were missing after restore failed.
- `dotnet test --no-build` returned exit code `0` with no output; treat as inconclusive, not as a confirmed passing test run.
- API run command was identified but could not start with `--no-build` because the executable was missing.
- `appsettings.json` and `docker/SqlServer.dockerfile_` contain sensitive or environment-specific values.
- SQL script creates database `restaurante`, while application connection string points to catalog `PedidosApi`.
- No seed command or seed implementation was found by text search.

## Decisions Taken

- Preserve the legacy state without functional changes.
- Do not modernize in Phase 1.
- Planned tag remains `v1.0.0-legacy`, to be created only in Prompt 03.
- Planned legacy branch remains `legacy/netcoreapp3.1`, to be created only in Prompt 03.
- Phase branch is `phase/1-preserve-legacy`.
- No prompt in Phase 1 should push.
- Existing Git references must never be moved with `--force`.

## Prompt 02 Should Do

- Read all files in `.sdd/phase-1/` in full before acting.
- Continue documenting the legacy version from the recorded baseline.
- Keep all shared context inside `.sdd/phase-1/`.
- Preserve source code, project files, dependencies, migrations, application configuration, tests, README, and workflows unchanged unless Prompt 02 explicitly says otherwise.
- Treat `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d` as `legacy_source_sha`.

## Prompt 02 Must Not Do

- Do not modernize to .NET 10.
- Do not install SDKs or tools automatically.
- Do not alter `global.json` or create one unless explicitly required by a later prompt.
- Do not update target frameworks or NuGet packages.
- Do not fix code, tests, migrations, appsettings, Dockerfiles, or workflows.
- Do not create `v1.0.0-legacy`.
- Do not create `legacy/netcoreapp3.1`.
- Do not push.
- Do not move existing Git refs with `--force`.
