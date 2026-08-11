# Report - 05 OpenAPI and API Versioning

## Changes

- Replaced legacy API Versioning with `Asp.Versioning.*`.
- Replaced Swashbuckle generation with ASP.NET Core OpenAPI.
- Added Scalar UI at `/scalar/`.
- Added versioned OpenAPI documents:
  - `/openapi/v1.json`
  - `/openapi/v2.json`
- Added JWT HTTP bearer OpenAPI scheme.
- Added operation-level security requirements for protected endpoints.
- Added Problem Details media documentation for error responses.
- Added reproducible contract generator at `tools/OpenApiGenerator`.
- Generated active contracts under `docs/openapi/`.
- Captured previous Swagger contracts under `docs/openapi/baseline/`.
- Updated tests for OpenAPI, Scalar, JWT, Problem Details, rate limiting and authz/authn regressions.

## Packages Added

- `Asp.Versioning.Mvc` `10.0.1`
- `Asp.Versioning.Mvc.ApiExplorer` `10.0.1`
- `Asp.Versioning.OpenApi` `10.0.1`
- `Microsoft.AspNetCore.OpenApi` `10.0.10`
- `Scalar.AspNetCore` `2.16.17`

## Packages Removed

- `Microsoft.AspNetCore.Mvc.Versioning` `5.1.0`
- `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` `5.1.0`
- `Swashbuckle.AspNetCore` `6.9.0`

## Contract

Active generated contracts:

- `docs/openapi/openapi-v1.json`
- `docs/openapi/openapi-v2.json`

Contract differences are recorded in `contract-diff.md`.

## Validation

Validation passed:

- `dotnet restore`
- `dotnet build --configuration Release --no-restore`: 0 warnings, 0 errors.
- `dotnet test --configuration Release --no-build`: 34 tests.
- OpenAPI generation command.
- JSON parse validation.
- `dotnet list package --deprecated`: only existing `xunit` 2.9.3 test-project deprecation.
- `dotnet list package --vulnerable`: no vulnerable packages.
- `git diff --check`: passed with line-ending normalization warnings only.

## Known Debts

- Existing analyzer warnings remain from previous prompts.
- `xunit` 2.9.3 is deprecated and should be evaluated for xUnit v3 migration in Phase 3.
- Native OpenAPI output no longer documents `404` for two `PUT` operations, although runtime behavior is preserved. This should be refined in a future contract-quality pass.
- `/hc` full real process validation still depends on external SQL Server availability outside the isolated test host.

## Next

Phase 2 is complete locally. Prepare `phase/3-quality-and-safety` for issues `#9` through `#13`.
