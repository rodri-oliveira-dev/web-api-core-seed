# Design - 04 Native Rate Limiting

## Summary

The active API now uses ASP.NET Core native rate limiting with endpoint metadata.

Implementation points:

- `services.AddNativeRateLimiting(configuration)` registers `AddRateLimiter`.
- `app.UseRateLimiter()` is placed in `UseMvcConfiguration` after:
  - `UseRouting`
  - `UseCookiePolicy`
  - `UseAuthentication`
- `UseRateLimiter` runs before `UseAuthorization` and endpoint execution.

This order lets the middleware see the selected endpoint and lets the policy use `HttpContext.User` after JWT authentication.

## Settings

The legacy `IpRateLimiting` section was replaced by:

```json
"NativeRateLimitingSettings": {
  "Public": {
    "PermitLimit": 3,
    "WindowSeconds": 1,
    "QueueLimit": 0
  },
  "Authenticated": {
    "PermitLimit": 3,
    "WindowSeconds": 1,
    "QueueLimit": 0
  },
  "AuthenticationSensitive": {
    "PermitLimit": 2,
    "WindowSeconds": 1,
    "QueueLimit": 0
  }
}
```

`QueueLimit` is `0` so callers receive a predictable `429` instead of waiting in-process.

## Policies

Policy names live in `NativeRateLimitPolicies`:

- `public`
- `authenticated`
- `authentication-sensitive`

Policies use fixed windows. The previous long windows were linear equivalents of the 3-per-second rule and did not add a stricter effective cap for steady traffic, so the native replacement keeps the short deterministic window and documents the behavior.

## Endpoint Metadata

- V1 `AuthController`: `authentication-sensitive`
- V2 `AuthController`: `authentication-sensitive`
- V1 `PratosController`: `authenticated`
- V1 `PratosController.ObterLista`: `public`
- V1 `MesasController`: `authenticated`

`/hc` and `/swagger/*` are not decorated with a policy and are exempt.

## Problem Details

Rejections use `IProblemDetailsService` and return:

```json
{
  "type": "urn:problem:rate-limit",
  "title": "Limite de requisicoes excedido.",
  "status": 429,
  "detail": "A cota de requisicoes foi excedida. Aguarde antes de tentar novamente.",
  "instance": "/path",
  "traceId": "..."
}
```

When the native limiter exposes retry metadata, the response includes:

```text
Retry-After: <seconds>
```

## Logging

Rejected requests log endpoint display name and request path only. Raw partition keys, user IDs, client IDs and IP addresses are not logged.

## Metrics

No custom telemetry was added. Native ASP.NET Core rate limiting remains compatible with framework diagnostics and future OpenTelemetry work.
