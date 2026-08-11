# Report - 03 Security Hardening

## Summary

Security defaults were hardened for CORS, forwarded headers, response headers, sensitive logging, health checks and request limits.

## Changes

- CORS:
  - Removed `AllowAnyOrigin`.
  - Added `CorsSettings`.
  - Production denies browser origins when `Cors:AllowedOrigins` is empty.
  - Literal `*` is rejected.
  - Testing config uses `https://app.example.test` explicitly.
- Forwarded headers:
  - Added `ForwardedHeadersSettings`.
  - Middleware is disabled by default.
  - Trusted proxies and networks are configuration-driven.
  - Production startup fails when forwarded headers are enabled without trust boundaries.
- Headers:
  - Removed active `X-XSS-Protection` and `Feature-Policy`.
  - Added `Referrer-Policy` and `Permissions-Policy`.
  - Kept `X-Frame-Options` and added CSP `frame-ancestors 'none'`.
  - Added no-store for auth and 401/403 responses.
- Logging:
  - Removed full query string enrichment.
  - Replaced raw target logging with sanitized path logging.
  - Kept request header logging restricted to the existing safe whitelist.
- Health:
  - Added `/health/live`.
  - Added `/health/ready`.
  - Kept `/hc` as a minimal legacy alias.
  - Production readiness responses are minimal; Development/Testing expose dependency entries.
- Limits:
  - Added `RequestLimitsSettings`.
  - Configured default request timeout of 30 seconds.
  - Configured default max request body of 10 MB.

## Tests

- Added `SecurityHardeningIntegrationTests`.
- Updated health integration coverage to use `/health/ready` for dependency readiness.
- Final suite passed with 62 total tests across both test projects.

## Risks And Follow-Up

- Production must provide browser origins through `Cors:AllowedOrigins`.
- Production behind a reverse proxy must provide `ForwardedHeaders` trust settings.
- HSTS is configured outside Development, but the integration `TestServer` does not expose it as a reliable header assertion.
- Legacy `docker/SqlServer.dockerfile_` still has a fixed local SQL password and should be handled only if that docker path becomes active again.
