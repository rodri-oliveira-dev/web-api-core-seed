# Naming Conventions - 04 OpenTelemetry

## Resource Attributes

| Attribute | Value |
| --- | --- |
| `service.name` | `web-api-core-seed-api` |
| `service.namespace` | `rodri-oliveira-dev.web-api-core-seed` |
| `service.version` | `OpenTelemetry:ServiceVersion` or assembly informational version |
| `service.instance.id` | Machine name |
| `deployment.environment.name` | `OpenTelemetry:Environment` or ASP.NET Core environment |

`OTEL_SERVICE_NAME` may override `OpenTelemetry:ServiceName`.

## Configuration Names

| Section | Purpose |
| --- | --- |
| `OpenTelemetry` | Traces, metrics, resource metadata and OTLP export. |
| `OpenTelemetry:Otlp` | Optional OTLP exporter settings. |
| `SeqSettings` | Optional Seq sink and health check settings. |

## Activity Sources

| Name | Purpose |
| --- | --- |
| `web-api-core-seed.api` | Reserved for future API custom spans. |
| `Microsoft.AspNetCore` / framework sources | ASP.NET Core request spans. |
| `System.Net.Http` / instrumentation sources | Outbound HTTP spans. |
| EF Core instrumentation source | Database spans emitted by the official instrumentation. |

## Meters

| Name | Purpose |
| --- | --- |
| `web-api-core-seed.api` | Reserved for future API custom metrics. |
| `Microsoft.AspNetCore.Hosting` | Request and hosting metrics. |
| `Microsoft.AspNetCore.Server.Kestrel` | Server metrics. |
| `System.Net.Http` | HTTP client metrics. |
| `System.Net.NameResolution` | DNS/name resolution metrics. |
| `System.Runtime` | Runtime metrics through instrumentation. |
| `Microsoft.EntityFrameworkCore` | EF Core metrics when emitted by the runtime/package. |

## Allowed Tags

- HTTP route template.
- HTTP method.
- HTTP response status code.
- Network protocol/scheme when emitted by official instrumentation.
- Dependency type.
- Result category.
- Exception type for unexpected exceptions.

## Cardinality Rules

Do not add labels/tags for:

- `userId`
- `orderId`
- `requestId`
- email
- raw URL
- raw query string
- token
- password
- exception message
- SQL statement text
- SQL parameter values

Route templates are preferred over concrete paths whenever a route attribute exists.
