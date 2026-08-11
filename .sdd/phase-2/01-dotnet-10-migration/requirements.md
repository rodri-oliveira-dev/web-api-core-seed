# Requirements - 01 .NET 10 Migration

## Objective

Migrate `RestauranteAPI.sln` from the preserved .NET Core 3.1 baseline to .NET 10, restoring package restore, build and the existing unit test execution while preserving observable behavior.

## Related Issue

- `#4 - [Phase 2] Migrate the solution to .NET 10`

## Scope

- Change every project target framework from `netcoreapp3.1` to `net10.0`.
- Add a repository `global.json` pinned to an SDK available in this environment.
- Update NuGet packages to compatible versions for .NET 10.
- Remove package references that are obsolete, duplicated or supplied by the shared framework when they are not required.
- Enable modern compilation settings where safe for an initial migration.
- Make only minimal source adjustments required by removed or changed APIs.
- Validate restore, build, tests, package state, repository searches and a smoke run when possible.
- Persist all shared context under `.sdd/phase-2/`.

## Out Of Scope

- Removing `Startup.cs`.
- Moving to the minimal hosting model.
- Replacing the existing error handling strategy with modern Problem Details.
- Replacing `AspNetCoreRateLimit` with native rate limiting.
- Replacing the current Swagger/API versioning approach with the final OpenAPI strategy.
- Hexagonal architecture, modularization, Aspire, Testcontainers or `WebApplicationFactory`.
- Changing HTTP routes, payloads, status codes, authentication semantics, existing migrations or persistence behavior.

## Acceptance Criteria

- No project targets `netcoreapp3.1`.
- The SDK used by repository commands is .NET 10.
- `dotnet restore` completes.
- `dotnet build --configuration Release --no-restore` completes.
- Existing tests compile and execute.
- Obsolete dependencies are removed, updated or documented.
- No planned work from future issues `#5` to `#8` is intentionally anticipated.
- No intentional HTTP contract changes are introduced.
- One semantic commit is created: `build: migrate solution to .NET 10`.

## Constraints

- Work on branch `phase/2-dotnet-10-migration`.
- Do not work directly on `main`.
- Do not use `reset`, `restore`, `checkout --`, `clean` or automatic stash for preexisting changes.
- Do not alter code before completing Specification, Discovery and Design.
- Keep changes small and reproducible.
- Use one commit for this prompt and do not push.

## Risks

- The .NET Core 3.1 baseline cannot restore in the current machine due to invalid local NuGet metadata for `microsoft.netcore.targets/1.1.0`.
- Some legacy packages have no .NET 10-aligned replacement under the same package ID.
- Swagger, API Versioning, health checks and rate limiting are intentionally retained as compatibility bridges and may need modernization in later prompts.
- Enabling nullable globally on legacy code may create a large warning surface not appropriate for this first migration.
- API startup may require external SQL Server, Redis or Seq services for full health validation.
