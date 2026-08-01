# Requirements - 02 Modern Hosting

## Specification

Adopt the modern ASP.NET Core hosting model for the active .NET 10 API while preserving observable HTTP behavior.

## Goals

- Initialize the API through a single `WebApplication`.
- Keep `Program.cs` as the concise composition root.
- Remove the legacy startup class file.
- Remove duplicate host builders.
- Remove static configuration access.
- Consolidate service registration and middleware setup into cohesive API extensions.
- Preserve JSON, CORS, authentication, authorization, response compression, health checks, Swagger, current rate limiting, static files, HSTS and error handling behavior.

## Out Of Scope

- Final Problem Details implementation.
- Native rate limiting.
- Final OpenAPI/versioning modernization.
- Hexagonal architecture.
- Modular monolith restructuring.
- Aspire.
- Functional endpoint contract changes.

## Acceptance Criteria

- The application starts from one `WebApplication`.
- The legacy startup class file is removed.
- Services receive `IConfiguration` explicitly where needed.
- Development and Production exception/CORS branches remain.
- Middleware order is explicit and documented.
- Build, tests and smoke validation pass or limitations are recorded.
- HTTP contracts remain compatible.
