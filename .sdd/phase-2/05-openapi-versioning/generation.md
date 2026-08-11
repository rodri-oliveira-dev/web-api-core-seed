# Generation - 05 OpenAPI and API Versioning

## Command

```text
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
```

## Output

- `docs/openapi/openapi-v1.json`
- `docs/openapi/openapi-v2.json`

## Baseline Capture

Before replacing Swagger, the same generator was used with explicit arguments:

```text
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release -- /swagger/v1/swagger.json=docs/openapi/baseline/swagger-v1.json /swagger/v2/swagger.json=docs/openapi/baseline/swagger-v2.json
```

## Validation

The generated JSON was parsed with PowerShell `ConvertFrom-Json`.

Summary:

- `docs/openapi/openapi-v1.json`: OpenAPI `3.0.4`, info version `v1`, paths preserved, `Bearer` scheme present.
- `docs/openapi/openapi-v2.json`: OpenAPI `3.0.4`, info version `v2`, v2 login path preserved, `Bearer` scheme present.
