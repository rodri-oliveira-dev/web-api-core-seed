# Requirements - 03 Security Hardening

## Specification

- Remove insecure HTTP and operational defaults from the active .NET 10 API.
- Make CORS explicit and restrictive, with no `AllowAnyOrigin` production behavior.
- Do not combine wildcard origins with credentials.
- Keep forwarded headers disabled unless trusted proxies or networks are configured.
- Replace obsolete security headers with current headers.
- Avoid logging complete query strings, credentials, tokens, cookies or API keys.
- Split public liveness from readiness.
- Keep public health responses minimal; expose dependency detail only in non-production diagnostics.
- Make request timeout and size limits explicit where the API host can enforce them.

## Acceptance Criteria

- Allowed browser origins come from `Cors:AllowedOrigins`.
- Empty production CORS configuration fails closed by returning no CORS allow headers.
- `X-XSS-Protection` and `Feature-Policy` are removed from active middleware.
- `Permissions-Policy`, `Referrer-Policy`, `X-Content-Type-Options`, frame protection and CSP are emitted.
- `Authorization`, cookies, API keys, tokens, passwords and client secrets are never captured in request logs.
- Public health endpoints reveal only aggregate status.
- Readiness checks SQL Server and Redis when those dependencies are configured.
- Regression and smoke tests cover CORS, headers, logging, Problem Details, rate limiting, OpenAPI and health.

## Out Of Scope

- New OAuth provider, WAF, API gateway, Nginx, production infrastructure, secret rotation service, ZAP, Sonar, Aspire or a complete authentication replacement.
