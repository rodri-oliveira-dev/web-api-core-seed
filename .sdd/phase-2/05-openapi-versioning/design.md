# Design - 05 OpenAPI and API Versioning

## Documents

- Document names: `v1`, `v2`.
- Document routes:
  - `/openapi/v1.json`
  - `/openapi/v2.json`
- UI route:
  - `/scalar/`

## Environment Behavior

OpenAPI JSON and Scalar UI are available in all environments for this educational seed. Future production hardening can restrict these endpoints if the template is used for a deployed production service.

## Versioning

- Use `Asp.Versioning.Mvc`.
- Preserve controller attributes.
- Preserve URL segment versioning with `api/v{version:apiVersion}`.
- Preserve default version `1.0` and `AssumeDefaultVersionWhenUnspecified`.
- Preserve reported API versions.
- Use `GroupNameFormat = "'v'VVV"` and `SubstituteApiVersionInUrl = true`.

## JWT

- OpenAPI security scheme name: `Bearer`.
- Type: HTTP.
- Scheme: `bearer`.
- Bearer format: `JWT`.
- Protected operations receive an operation-level security requirement.

## Problem Details

- Problem responses are documented with `application/problem+json`.
- The generated schema uses `ProblemDetails`.
- Runtime Problem Details behavior from prompt 03 is preserved.

## Responses

- `400` and `429` are added through an OpenAPI operation transformer.
- `401` and `403` are added for operations that require authorization.
- Existing controller `ProducesResponseType` metadata remains the primary source for success and domain responses.

## Generation Command

```text
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
```

Output:

- `docs/openapi/openapi-v1.json`
- `docs/openapi/openapi-v2.json`

The generator uses `WebApplicationFactory`, disables Redis and Seq, replaces SQL contexts with EF InMemory, clears health check registrations, and fetches documents through the real ASP.NET Core pipeline.
