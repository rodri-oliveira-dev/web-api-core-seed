# Logging Data Classification - 03 Security Hardening

| Data | Classification | Logging Rule |
| --- | --- | --- |
| `Authorization` | Secret | Never log; if diagnostics require presence, log only a boolean. |
| Cookies / `Set-Cookie` | Secret/session | Never log values. |
| Query strings | Potentially sensitive | Do not log complete query strings. Log route path and selected non-sensitive route metadata only. |
| Connection strings | Secret | Never log complete values. Log dependency names only. |
| JWTs / `access_token` | Secret | Never log. |
| Passwords | Secret | Never log. |
| `client_secret` | Secret | Never log. |
| `X-Api-Key` / API keys | Secret | Never log. |
| E-mails | Personal data | Avoid in infrastructure logs; business audit logs must have a defined purpose and retention. |
| Identifiers | Internal/pseudonymous | Log only when needed, preferably hashed for rate limiting and correlation. |
| Payloads | Potentially sensitive | Do not log raw payloads. |
| Stack traces | Internal diagnostic | Log server-side for exceptions; do not expose in production responses. |
