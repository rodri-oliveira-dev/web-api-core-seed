# Policy Matrix - 04 Native Rate Limiting

| Policy | Surface | Limit | Partition Key | Notes |
| --- | --- | --- | --- | --- |
| `public` | Anonymous public API reads, currently `GET /api/v1/Pratos` | 3 requests / 1 second, no queue | Authenticated user ID when present; otherwise hashed composite of `X-ClientId` when supplied and direct connection remote address | Preserves the effective short legacy API limit for public traffic. |
| `authenticated` | Protected `Pratos` and `Mesas` endpoints | 3 requests / 1 second, no queue | Authenticated user ID claim, hashed before storage; anonymous fallback only for unauthenticated attempts | User partitioning prevents one user from consuming another user's quota. |
| `authentication-sensitive` | Login and account/auth endpoints under V1/V2 auth controllers | 2 requests / 1 second, no queue | Authenticated user ID when present; otherwise hashed composite of `X-ClientId` when supplied and direct connection remote address | Tighter than general API traffic because repeated auth attempts are sensitive. |
| Health | `/hc` | Exempt | Not applicable | Endpoint is not mapped with a rate-limiting policy. |
| Swagger | `/swagger/*` | Exempt | Not applicable | Endpoint is outside API policies and remains available for current smoke checks. |

## Partition Claim Order

For authenticated requests, the partition identity is selected in this order:

1. `ClaimTypes.NameIdentifier`
2. `sub`
3. `ClaimTypes.Email`

The selected value is hashed with SHA-256 before being used as the in-memory partition key.

## Anonymous Partition Fallback

Anonymous requests use:

```text
policy + SHA256("anonymous|client:{X-ClientId}|remote:{Connection.RemoteIpAddress}")
```

When `X-ClientId` is absent:

```text
policy + SHA256("anonymous|remote:{Connection.RemoteIpAddress}")
```

The header is not treated as trusted identity. It is only a partition hint combined with the direct remote address.
