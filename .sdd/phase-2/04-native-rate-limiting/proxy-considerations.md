# Proxy Considerations - 04 Native Rate Limiting

## Current State

The active API does not configure forwarded headers:

```text
git grep -n "ForwardedHeaders"
```

Result: no active findings before this prompt.

The active API also did not previously read `RemoteIpAddress` directly. This prompt introduces direct connection remote address only as an anonymous fallback partition signal.

## Decision

The native limiter does not trust:

- `X-Forwarded-For`
- `X-Real-IP`
- any arbitrary forwarded client IP header

Reason: the repository has no configured trusted proxy, known proxy list or known network list. Trusting those headers now would let external callers spoof quota partitions.

## Behavior Behind A Proxy

If the API runs behind a reverse proxy without `ForwardedHeadersOptions`, the direct remote address observed by ASP.NET Core may be the proxy address. Anonymous callers behind the same proxy can therefore share a fallback quota.

Authenticated requests are less affected because their primary partition key is the validated user identity.

## Future Safe Proxy Setup

Before using forwarded client IP as part of rate limiting, configure and validate:

- `ForwardedHeadersOptions.ForwardedHeaders`
- `KnownProxies` or `KnownNetworks`
- the proxy's exact forwarded header behavior
- infrastructure ownership of the proxy hop

After that, the anonymous partition logic can be revisited to use a trusted forwarded address.

## NAT Risk

Anonymous IP-based fallback can group unrelated users behind the same NAT. This is why the implementation:

- uses user identity for authenticated traffic;
- combines `X-ClientId` with the direct remote address when supplied;
- hashes the composite key;
- avoids logging the raw partition input.
