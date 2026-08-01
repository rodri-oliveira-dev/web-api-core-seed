# Package Assessment - 05 OpenAPI and API Versioning

## Sources Checked

- Microsoft Learn: ASP.NET Core OpenAPI in .NET 10.
- Microsoft .NET Blog: combining API versioning with OpenAPI in .NET 10.
- `dotnet/aspnet-api-versioning` wiki: OpenAPI and Scalar integration.
- NuGet search through `dotnet package search`.

## Options

### `Microsoft.AspNetCore.OpenApi`

Pros:

- First-party OpenAPI generation for ASP.NET Core.
- Supports controller-based apps through API Explorer.
- Provides runtime OpenAPI endpoint and document transformers.
- Fits .NET 10 direction and future CI validation.

Cons:

- Does not ship a UI.
- Requires explicit transformers for security and response customization.

### Current API Versioning Packages

`Microsoft.AspNetCore.Mvc.Versioning*` are obsolete for the active .NET 10 direction.

Decision: remove.

### `Asp.Versioning.*`

Pros:

- Supported package family for API versioning.
- `Asp.Versioning.Mvc` preserves controller attributes.
- `Asp.Versioning.Mvc.ApiExplorer` preserves versioned API Explorer behavior.
- `Asp.Versioning.OpenApi` integrates versioning with `Microsoft.AspNetCore.OpenApi` and supports `WithDocumentPerVersion()`.

Decision: use.

### Swashbuckle

Pros:

- Existing implementation already used it.
- Full Swagger UI and OpenAPI generation are familiar.

Cons:

- Keeping full Swashbuckle alongside `Microsoft.AspNetCore.OpenApi` would be redundant.
- Existing Swashbuckle config carried legacy filters and obsolete versioning integration.
- The prompt explicitly asks not to keep redundant packages.

Decision: remove full Swashbuckle generation and UI package.

### Scalar

Pros:

- Modern UI for OpenAPI documents.
- Integrates directly with ASP.NET Core OpenAPI endpoints.
- Versioning wiki provides a direct `MapScalarApiReference` pattern.
- No Node.js or global tool dependency.

Cons:

- Adds one UI package.
- Authentication UI depends on the generated security scheme in the document.

Decision: use Scalar for UI at `/scalar/`.

### Swagger UI

Pros:

- Familiar UI.
- Could be kept via `Swashbuckle.AspNetCore.SwaggerUI` only.

Cons:

- Less simple than Scalar for the chosen native OpenAPI path.
- Keeping a Swashbuckle UI package while removing generation would still carry Swagger-specific surface.

Decision: do not use for active UI.

## Selected Solution

- `Asp.Versioning.Mvc` `10.0.1`
- `Asp.Versioning.Mvc.ApiExplorer` `10.0.1`
- `Asp.Versioning.OpenApi` `10.0.1`
- `Microsoft.AspNetCore.OpenApi` `10.0.10`
- `Scalar.AspNetCore` `2.16.17`

Removed:

- `Microsoft.AspNetCore.Mvc.Versioning` `5.1.0`
- `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` `5.1.0`
- `Swashbuckle.AspNetCore` `6.9.0`
