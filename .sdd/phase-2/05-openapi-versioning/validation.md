# Validation - 05 OpenAPI and API Versioning

## Development Validation

Executed during development:

```text
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
dotnet list package
```

Results:

- Restore: passed.
- Build: passed.
- Tests: passed, 34 tests.
- Contract generation: passed.
- `dotnet list package`: active API has `Asp.Versioning.*`, `Microsoft.AspNetCore.OpenApi`, and `Scalar.AspNetCore`.

## Contract Checks

- JSON parsed successfully with `ConvertFrom-Json`.
- V1 and V2 documents respond through `WebApplicationFactory`.
- V1 paths are preserved.
- V2 login path is preserved.
- `Bearer` security scheme is present as HTTP bearer JWT.
- Protected operation includes `Bearer` security.
- Public `GET /api/v1/Pratos` has no operation security requirement.
- Problem Details content is present for documented error responses.
- `429` is documented as `application/problem+json`.

## Smoke and Regression

Covered by integration tests:

- OpenAPI document route.
- Scalar UI route.
- Health check.
- Public endpoint.
- Protected endpoint without token returns `401 application/problem+json`.
- Protected endpoint with token but no permission returns `403 application/problem+json`.
- Invalid payload returns validation Problem Details.
- Domain rule returns Problem Details.
- `429` returns Problem Details and `Retry-After`.

## Final Validation

Executed before delivery:

```text
dotnet --info
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet list package --deprecated
dotnet list package --vulnerable
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
ConvertFrom-Json over baseline and generated contracts
git grep -n "Microsoft.AspNetCore.Mvc.Versioning" -- src test README.md
git grep -n "VersionedApiExplorer" -- src test README.md
git diff --check
git status
```

Results:

- `dotnet --info`: SDK `10.0.302`; host runtime `10.0.10`.
- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed with 0 warnings and 0 errors.
- `dotnet test --configuration Release --no-build`: passed, 34 tests.
- `dotnet list package --deprecated`: only `xunit` 2.9.3 in `Pedidos.Test` is reported as deprecated with `xunit.v3` as the suggested alternative.
- `dotnet list package --vulnerable`: no vulnerable packages reported.
- OpenAPI generation: passed and wrote `docs/openapi/openapi-v1.json` and `docs/openapi/openapi-v2.json`.
- JSON parser validation: passed for generated V1/V2 contracts and baseline Swagger V1/V2 contracts.
- Active-code package grep: no findings for `Microsoft.AspNetCore.Mvc.Versioning`.
- Active-code explorer grep: no findings for `VersionedApiExplorer`.
- `git diff --check`: passed; Git reported line-ending normalization warnings only.

## Consolidated Regression

Confirmed by the full test suite and focused WebApplicationFactory integration tests:

- .NET 10 target framework remains active.
- Modern hosting remains active and no `Startup` class is present in active API code.
- Problem Details remains the error contract.
- Native rate limiting remains active.
- OpenAPI V1/V2 documents are valid and reachable.
- Scalar UI is reachable.
- JWT authentication is preserved and documented in OpenAPI.
- Health check endpoint remains registered.
- Public endpoint remains callable.
- Protected endpoint without token returns `401 application/problem+json`.
- Protected endpoint with a valid token but missing permission returns `403 application/problem+json`.
- Rate-limit rejection returns `429 application/problem+json`.
- No Phase 4 modular architecture migration was introduced.
