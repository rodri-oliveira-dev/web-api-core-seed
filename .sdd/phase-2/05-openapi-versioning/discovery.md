# Discovery - 05 OpenAPI and API Versioning

## Commands

Executed before changes:

```text
git status --short
git branch --show-current
git log -3 --oneline
dotnet build --configuration Release
dotnet test --configuration Release --no-build
git grep -n "Swashbuckle"
git grep -n "Swagger"
git grep -n "ApiVersion"
git grep -n "VersionedApiExplorer"
git grep -n "IApiVersionDescriptionProvider"
git grep -n "OpenApi"
git grep -n "ProducesResponseType"
```

## Initial State

- Branch: `phase/2-dotnet-10-migration`.
- Working tree: clean before prompt work.
- Baseline build: passed with existing warnings.
- Baseline tests: passed, 32 tests.

## Current Packages Before Change

- `Swashbuckle.AspNetCore` `6.9.0`.
- `Microsoft.AspNetCore.Mvc.Versioning` `5.1.0`.
- `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` `5.1.0`.

## Current Documents Before Change

Captured with the local generator into:

- `docs/openapi/baseline/swagger-v1.json`
- `docs/openapi/baseline/swagger-v2.json`

Baseline documents:

- `/swagger/v1/swagger.json`: OpenAPI `3.0.1`, info version `1.0`.
- `/swagger/v2/swagger.json`: OpenAPI `3.0.1`, info version `2.0`.

## Routes and Versions

V1 paths:

- `/api/v1/nova-conta`
- `/api/v1/entrar`
- `/api/v1/Mesas/{id}`
- `/api/v1/Mesas`
- `/api/v1/Pratos`
- `/api/v1/Pratos/{id}`

V2 paths:

- `/api/v2/entrar`

Version attributes:

- V1 auth controller: `[ApiVersion("1.0", Deprecated = true)]`.
- V1 `MesasController`: `[ApiVersion("1.0")]`.
- V1 `PratosController`: `[ApiVersion("1.0")]`.
- V2 auth controller: `[ApiVersion("2.0")]`.

## Filters and Security

- Active Swagger operation filter: `SwaggerDefaultValues`.
- Active Swagger options configurator: `ConfigureSwaggerOptions`.
- Active security scheme: `Bearer` as `apiKey` in the `Authorization` header.
- Active Swagger UI route: `/swagger`.

## Documentation Gaps

- API Versioning package family was superseded by `Asp.Versioning.*`.
- Swagger security was documented as `apiKey`, not HTTP bearer.
- Problem Details responses existed at runtime but were not consistently documented as `application/problem+json`.
- `429` was documented broadly, but not consistently with Problem Details media.
- Some response status documentation was incomplete or inconsistent with runtime behavior.
- No committed command existed to generate OpenAPI contracts for future CI validation.

## `.http`

No `.http` or `.rest` files were found.
