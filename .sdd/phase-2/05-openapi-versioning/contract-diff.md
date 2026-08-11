# Contract Diff - 05 OpenAPI and API Versioning

## Compared Files

Before:

- `docs/openapi/baseline/swagger-v1.json`
- `docs/openapi/baseline/swagger-v2.json`

After:

- `docs/openapi/openapi-v1.json`
- `docs/openapi/openapi-v2.json`

## Summary

| Area | Change | Classification |
| --- | --- | --- |
| Document route | `/swagger/{version}/swagger.json` replaced by `/openapi/{version}.json` | Breaking change for documentation consumers |
| UI route | `/swagger` replaced by `/scalar/` | Breaking change for documentation UI users |
| Paths | No API path additions or removals | Compatible |
| OpenAPI version | `3.0.1` to `3.0.4` | Compatible/document generation change |
| Info version | `1.0`/`2.0` to `v1`/`v2` | Documentation change |
| JWT scheme | `apiKey` header scheme to HTTP `bearer` JWT | Correction documental |
| Problem Details | `application/problem+json` added to error responses | Correction documental |
| `429` | Documented with Problem Details content | Correction documental |

## Path Comparison

V1 paths preserved:

- `/api/v1/nova-conta`
- `/api/v1/entrar`
- `/api/v1/Mesas/{id}`
- `/api/v1/Mesas`
- `/api/v1/Pratos`
- `/api/v1/Pratos/{id}`

V2 paths preserved:

- `/api/v2/entrar`

## Response Differences

V1 generated response differences:

| Path | Method | Added | Removed | Classification |
| --- | --- | --- | --- | --- |
| `/api/v1/nova-conta` | `POST` | `401`, `403` | none | Correction documental; endpoint has `[Authorize]` |
| `/api/v1/Mesas/{id}` | `GET` | `400` | none | Correction documental from common Problem Details convention |
| `/api/v1/Mesas/{id}` | `PUT` | none | `404` | Documentation limitation/regression; runtime still returns 404 when resource is missing |
| `/api/v1/Pratos` | `GET` | `400` | none | Correction documental from common Problem Details convention |
| `/api/v1/Pratos/{id}` | `GET` | `400` | none | Correction documental from common Problem Details convention |
| `/api/v1/Pratos/{id}` | `PUT` | none | `404` | Documentation limitation/regression; runtime still returns 404 when resource is missing |

No path was removed. No runtime route was intentionally changed.

## Security Difference

Before:

- `Bearer` was modeled as `apiKey` in the `Authorization` header.

After:

- `Bearer` is modeled as HTTP bearer with `bearerFormat: JWT`.
- Protected operations contain a `Bearer` security requirement.
- Anonymous operations do not require security in the OpenAPI operation.

## Breaking Changes

- Documentation endpoints changed from Swagger paths to OpenAPI/Scalar paths.
- Consumers hardcoded to `/swagger/v1/swagger.json`, `/swagger/v2/swagger.json`, or `/swagger` must update to `/openapi/v1.json`, `/openapi/v2.json`, and `/scalar/`.

## Known Documentation Debt

- Native OpenAPI generation did not carry the previous `404` documentation for `PUT` operations even though runtime behavior is preserved. This is documented as a contract documentation debt for later refinement.
