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
| D015 | Pin .NET SDK `10.0.302` in `global.json`. | Accepted | It is the highest installed .NET 10 SDK in this environment and avoids a nonexistent feature band. |
| D016 | Use `net10.0` for all active projects. | Accepted | This completes the first technical migration objective while preserving the legacy branch/tag separately. |
| D017 | Keep `Nullable` disabled globally for prompt 01. | Accepted | The legacy code was not annotated and enabling nullable would create broad warning churn unrelated to the migration. |
| D018 | Keep the legacy startup file and host hook for prompt 01. | Accepted | Hosting modernization belongs to prompt 02 / issue `#5`. |
| D019 | Keep `AspNetCoreRateLimit` temporarily and register `AsyncKeyLockProcessingStrategy`. | Accepted | The package remains a bridge until native rate limiting is implemented in a later prompt. |
| D020 | Keep legacy API Versioning packages temporarily. | Accepted | `Microsoft.AspNetCore.Mvc.Versioning*` is deprecated, but migration to `Asp.Versioning.*` belongs to the OpenAPI/versioning prompt. |
| D021 | Keep Swashbuckle on `6.9.0` temporarily. | Accepted | Swashbuckle 10 changes the `Microsoft.OpenApi` API surface and would anticipate the future OpenAPI modernization. |
| D022 | Disable HealthChecks UI web `/hc-ui` temporarily. | Accepted | Latest available `AspNetCore.HealthChecks.UI` is 9.0.0 and failed at runtime with EF Core 10; `/hc` remains registered with `UIResponseWriter`. |
| D023 | Add direct `KubernetesClient` `19.0.2` as a private transitive override. | Accepted | It removes the vulnerable transitive version pulled by health checks packages without changing application code. |
| D024 | Use `WebApplication.CreateBuilder` as the only active hosting entry point. | Accepted | Modern ASP.NET Core hosting removes obsolete builders and keeps composition in one place. |
| D025 | Remove static configuration access and inject `IConfiguration` into registration extensions. | Accepted | Services should not depend on process-wide static configuration state. |
| D026 | Keep current compatibility packages for rate limiting, Swagger and API Versioning during hosting modernization. | Accepted | Replacing those packages belongs to later Phase 2 issues and would change more behavior than this prompt requires. |
| D027 | Use native ASP.NET Core Problem Details as the only active error contract. | Accepted | It removes custom duplicated error envelopes and aligns the API with the planned modernization direction. |
| D028 | Keep `CustomResult` for success responses during Problem Details migration. | Accepted | The prompt changes error contracts only; changing success contracts would broaden the blast radius. |
| D029 | Keep Domain Notification and map it to Problem Details. | Accepted | The pattern still carries business messages; there is no replacement domain error model yet. |
| D030 | Use minimal HTTP integration tests with `WebApplicationFactory` and isolated fakes. | Accepted | The task changes HTTP error contracts, while full SQL/Testcontainers strategy belongs to a later phase. |
| D031 | Replace `AspNetCoreRateLimit` with native ASP.NET Core rate limiting. | Accepted | .NET 10 includes first-party rate limiting middleware and the legacy package was only a temporary bridge. |
| D032 | Use explicit `public`, `authenticated` and `authentication-sensitive` policies. | Accepted | Different endpoint surfaces have different abuse and usability risks; one implicit global API rule would hide that design. |
| D033 | Partition authenticated traffic by validated user identity. | Accepted | It avoids one authenticated user consuming another user's quota. The selected identity value is hashed before use as a limiter key. |
| D034 | Do not trust forwarded client IP headers in rate limiting. | Accepted | The repository has no trusted proxy or known network configuration, so trusting forwarded headers would allow spoofed partitions. |
| D035 | Keep `/hc` and `/swagger/*` exempt from API rate limiting. | Accepted | They were outside the legacy `*:/api/*` rule and are needed for health and smoke validation. |
| D036 | Replace legacy API Versioning packages with `Asp.Versioning.*`. | Accepted | `Microsoft.AspNetCore.Mvc.Versioning*` is obsolete for this runtime; the supported package line preserves controller versioning and API explorer integration on .NET 10. |
| D037 | Use `Microsoft.AspNetCore.OpenApi` with `Asp.Versioning.OpenApi` for generated contracts. | Accepted | Native ASP.NET Core OpenAPI is the simplest supported generation path for .NET 10 and avoids keeping Swashbuckle only for document generation. |
| D038 | Use Scalar UI at `/scalar/`. | Accepted | Scalar integrates cleanly with native OpenAPI endpoints, supports API key/bearer authentication in the UI, and avoids retaining redundant Swagger UI packages. |
| D039 | Commit generated contracts under `docs/openapi/` and keep previous Swagger contracts under `docs/openapi/baseline/`. | Accepted | A versioned repository artifact allows future CI validation and makes the OpenAPI modernization diff auditable. |
| D040 | Document JWT as HTTP bearer JWT and Problem Details through OpenAPI transformers. | Accepted | The previous API-key-style bearer scheme was only a UI workaround; explicit transformers keep security and error response metadata close to the platform OpenAPI pipeline. |
