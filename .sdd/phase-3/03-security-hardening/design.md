# Design - 03 Security Hardening

## CORS

- Replace hard-coded permissive policies with configuration-backed origins from `Cors:AllowedOrigins`.
- Use explicit methods and headers.
- Reject literal wildcard origin `*` and never enable credentials with wildcard origins.
- If no origins are configured, the policy denies browser origins by omitting CORS allow headers.

## Forwarded Headers

- Add `ForwardedHeaders` configuration with `Enabled`, `KnownProxies`, `KnownNetworks` and `ForwardLimit`.
- Keep middleware disabled by default.
- When enabled, trust only configured IPs or CIDR networks.
- In production, fail startup if forwarded headers are enabled without any trusted proxy or network.

## Headers

- Replace the legacy manual header block with current security headers.
- Keep CSP compatible with Scalar by allowing local scripts/styles and inline UI bootstrap.
- Use `Permissions-Policy` instead of `Feature-Policy`.
- Set no-store headers for authentication and authorization-sensitive responses.

## Logging

- Remove full query string enrichment.
- Log sanitized path only, not raw target.
- Keep custom error header logging restricted to a small safe whitelist.
- Keep production Problem Details generic while preserving internal exception logs.

## Health

- Map `/health/live` for minimal liveness.
- Map `/health/ready` for dependency readiness.
- Keep `/hc` as a legacy minimal alias to avoid exposing internal dependency details publicly.
- Return detailed health check entries only in Development and Testing.
- Keep public health endpoints exempt from rate limiting because they expose only aggregate status and existing regressions require health probes to remain stable.

## Limits

- Configure request timeout and Kestrel body size from `RequestLimits`.
- Defaults: 30 seconds timeout and 10 MB max request body.
- Keep values configurable because final hosting infrastructure is not defined.
