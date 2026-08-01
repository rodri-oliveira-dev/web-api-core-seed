# Design - 04 OpenTelemetry

## Central Registration

OpenTelemetry is registered centrally through:

```csharp
services.AddApiOpenTelemetry(configuration, environment);
```

The extension uses the required conceptual shape:

```csharp
services
    .AddOpenTelemetry()
    .ConfigureResource(...)
    .WithTracing(...)
    .WithMetrics(...);
```

## Configuration

Main section:

```text
OpenTelemetry:Enabled
OpenTelemetry:ServiceName
OpenTelemetry:ServiceNamespace
OpenTelemetry:ServiceVersion
OpenTelemetry:Environment
OpenTelemetry:Tracing:SamplingRatio
OpenTelemetry:Tracing:RecordExceptions
OpenTelemetry:Metrics:RuntimeInstrumentation
OpenTelemetry:Otlp:Enabled
OpenTelemetry:Otlp:Endpoint
OpenTelemetry:Otlp:Protocol
```

Environment compatibility:

- `OTEL_SERVICE_NAME`
- `OTEL_EXPORTER_OTLP_ENDPOINT`
- `OTEL_EXPORTER_OTLP_PROTOCOL`

OTLP export is added only when `OpenTelemetry:Otlp:Enabled=true` or an endpoint is configured.

## Tracing

Enabled instrumentation:

- ASP.NET Core.
- HttpClient.
- EF Core.

The sampler is parent-based with configurable trace-id ratio.

## Metrics

Enabled instrumentation/meters:

- ASP.NET Core instrumentation.
- HttpClient instrumentation.
- Runtime instrumentation.
- `Microsoft.AspNetCore.Hosting`.
- `Microsoft.AspNetCore.Server.Kestrel`.
- `System.Net.Http`.
- `System.Net.NameResolution`.
- `System.Runtime`.
- `Microsoft.EntityFrameworkCore`.

## Redis

Redis tracing was not added.

Rationale:

- The available `OpenTelemetry.Instrumentation.StackExchangeRedis` package is pre-release.
- It requires the `IConnectionMultiplexer` used by calls to be instrumented.
- The active app uses `AddStackExchangeRedisCache`, which hides the multiplexer behind `IDistributedCache`.
- Adding a custom multiplexer/cache abstraction just for tracing would increase design surface without enough value in this baseline.

Redis remains covered by health checks and integration tests.

## Logs

Serilog remains the only application logging pipeline:

- structured logging is preserved;
- console/debug/file sinks remain;
- Seq is optional through `SeqSettings:Enabled`;
- file and console templates include `TraceId` and `SpanId`;
- OpenTelemetry logs provider is not enabled to avoid duplicating all logs.

## Seq Rename

The active settings class and section were renamed to `SeqSettings`.

Health check name is now `Seq Log`.

## Data Safety

- No custom baggage.
- No custom high-cardinality labels.
- No custom SQL statement enrichment.
- Query redaction is forced on for ASP.NET Core and HttpClient instrumentation.
- Request logging continues to avoid full query strings.
