# Requirements - 04 OpenTelemetry

## Objective

Introduce a vendor-neutral OpenTelemetry baseline for the active .NET 10 API without requiring Aspire, a local collector, dashboards or a production observability stack.

## Acceptance Criteria

- ASP.NET Core requests emit server traces.
- Outbound HTTP calls are instrumented through the standard HttpClient instrumentation.
- EF Core operations are instrumented.
- Redis instrumentation is evaluated and used only if supported and useful for the active cache design.
- Essential ASP.NET Core, HTTP client, runtime and EF Core metrics are registered for export.
- Serilog logs include trace and span correlation fields.
- OTLP export is optional and configuration-driven.
- The API starts when OpenTelemetry is disabled.
- The API starts when OTLP is configured but no collector is available.
- Active code and configuration no longer use the legacy product-specific Seq naming.
- Seq remains optional.
- Sensitive values are not added intentionally to spans, baggage, metric labels or logs.
- Tests and smoke validation cover the configuration.

## Out Of Scope

- Aspire or AppHost.
- Collector, Grafana, Prometheus server, Jaeger, Loki or Docker Compose observability stack.
- Production alerting, SLOs or vendor-specific SDKs.
- Custom domain telemetry beyond a small naming baseline.

## Inputs Read

- `AGENTS.md`
- `.sdd/phase-3/README.md`
- `.sdd/phase-3/status.md`
- `.sdd/phase-3/decisions.md`
- `.sdd/phase-3/handoff.md`
- `.sdd/phase-3/03-security-hardening/report.md`
- `.sdd/phase-3/03-security-hardening/logging-data-classification.md`
- Current API hosting, Serilog, Seq, health checks, Redis cache, EF Core, request pipeline and integration tests.

## Skill Notes

- Used `dotnet-service-change`.
- Used `integration-tests-dotnet`.
- `configuring-opentelemetry-dotnet` was requested but is not installed in this Codex session; official Microsoft/OpenTelemetry/NuGet sources were used instead.
