# Report - 04 OpenTelemetry

## Summary

OpenTelemetry observability was added for traces, metrics, log correlation and optional OTLP export without introducing a required external observability stack.

## Packages

- `OpenTelemetry.Exporter.OpenTelemetryProtocol` `1.17.0`
- `OpenTelemetry.Extensions.Hosting` `1.17.0`
- `OpenTelemetry.Instrumentation.AspNetCore` `1.17.0`
- `OpenTelemetry.Instrumentation.EntityFrameworkCore` `1.17.0-beta.1`
- `OpenTelemetry.Instrumentation.Http` `1.17.0`
- `OpenTelemetry.Instrumentation.Runtime` `1.17.0`

## Instrumentation

- ASP.NET Core traces and metrics.
- HttpClient traces and metrics.
- EF Core traces.
- Runtime metrics.
- Framework meters for ASP.NET Core, Kestrel, HTTP, name resolution and EF Core.

Redis instrumentation was evaluated but not added because the available package is pre-release and the current cache abstraction does not expose the required `IConnectionMultiplexer`.

## Exporters

- OTLP exporter for traces and metrics.
- Export is optional through `OpenTelemetry:Otlp:Enabled` or `OpenTelemetry:Otlp:Endpoint`.
- No collector is required for startup.

## Metadata

- `service.name`: `web-api-core-seed-api`
- `service.namespace`: `rodri-oliveira-dev.web-api-core-seed`
- `service.version`: configured value or assembly informational version
- `deployment.environment.name`: configured value or ASP.NET Core environment

## Logs

- Serilog remains the logging pipeline.
- Console and file output templates include `TraceId` and `SpanId`.
- Seq remains optional through `SeqSettings:Enabled`.
- OpenTelemetry log export was not enabled to avoid duplicating the Serilog pipeline.

## Data Safety

- No custom baggage.
- No custom SQL command enrichment.
- Query redaction is forced on for ASP.NET Core and HttpClient instrumentation.
- Request logging continues to avoid full query strings and sensitive headers.
- Tests assert sensitive query/header values are not present in captured Activity tag values.

## Naming Cleanup

- Active code/configuration now uses `SeqSettings`.
- Active health check name is `Seq Log`.
- Stale API project artifacts `healthchecksdb` and `teste.txt` were removed.

Historical references remain only in legacy/older SDD documentation.

## Tests

- Added `ObservabilityConfigurationTests`.
- Existing integration factory now configures `SeqSettings` and OpenTelemetry explicitly.
- Existing OpenAPI/problem-details factories disable OpenTelemetry where it is not under test.

## Limitations

- No collector-backed export validation.
- No Redis spans in this baseline.
- EF Core instrumentation package is beta because no stable version was available.

## Next Issue

Proceed to issue `#13` / Prompt 5 for CI and quality gates.
