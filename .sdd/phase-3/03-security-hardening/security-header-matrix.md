# Security Header Matrix - 03 Security Hardening

| Header | Current | Status | Action | Proposed Value | Environment | Impact |
| --- | --- | --- | --- | --- | --- | --- |
| `Strict-Transport-Security` | Configured globally | Kept | Emit outside Development | `max-age=31536000; includeSubDomains` | Non-development | Encourages HTTPS for repeat clients |
| `X-Content-Type-Options` | Present via manual `Add` | Kept | Emit with indexer | `nosniff` | All | Reduces MIME sniffing |
| `Referrer-Policy` | Missing | Added | Emit globally | `no-referrer` | All | Avoids leaking URLs to external sites |
| `Permissions-Policy` | Missing | Added | Replace obsolete Feature-Policy | `accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()` | All | Disables unused browser capabilities |
| `Content-Security-Policy` | Present | Updated | Keep compatible with Scalar UI | `default-src 'self'; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self'` | All | Protects docs/API pages while preserving current documentation UI |
| `X-Frame-Options` | Present | Kept | Emit with indexer | `DENY` | All | Legacy frame protection; CSP `frame-ancestors` is canonical |
| `Cache-Control` | Not explicit for auth/problem responses | Added | Set no-store for auth, 401 and 403 | `no-store` plus `Pragma: no-cache` | All | Prevents token and auth failure caching |
| `X-XSS-Protection` | Present | Removed | Do not emit | None | All | Obsolete browser feature |
| `Feature-Policy` | Present | Removed | Do not emit | None | All | Obsolete predecessor of Permissions-Policy |
