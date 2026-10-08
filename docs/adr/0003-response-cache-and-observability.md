# ADR 0003 — Isolated shared HTTP cache and opt-in telemetry export

- **Status:** Accepted (implemented)
- **Context:** Redis caching and observability should demonstrate production-minded boundaries without leaking authenticated responses or credentials through shared cache entries and telemetry sinks.

## Decision

Use distributed Redis response caching only for eligible anonymous GET requests with no `Authorization` or `Cookie` request headers. Cache writes occur only after MVC result execution, when the final response status is HTTP 200 and `Set-Cookie` is absent; canceled or faulted actions are not cached. Generate length-delimited, SHA-256-derived cache keys that distinguish repeated query values. Use Serilog for structured logs and OpenTelemetry for ASP.NET Core, HTTP client, EF Core and runtime signals; external OTLP export is disabled until explicitly configured.

## Consequences

- Sensitive and authenticated requests bypass shared cache; public cached responses can still become stale for their configured TTL.
- A cache key is an implementation detail, not an authorization mechanism or substitute for input validation.
- Telemetry collection/external export requires deliberate configuration, environment review and redaction checks.
- Redis, exporters and log stores add operational dependencies when enabled.

## Evidence

- `src/WebApiCoreSeed.Api/Attributes/CachedAttribute.cs`
- `src/WebApiCoreSeed.Api/Configuration/CacheConfig.cs`
- `src/WebApiCoreSeed.Api/Configuration/OpenTelemetryConfig.cs`
- [Architecture and configuration](../architecture.md)