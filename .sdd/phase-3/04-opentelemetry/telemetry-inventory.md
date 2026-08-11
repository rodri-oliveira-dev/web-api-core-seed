# Telemetry Inventory - 04 OpenTelemetry

## Logs

| Source | Current State | Notes |
| --- | --- | --- |
| Bootstrap Serilog | Active | Startup/fatal logs before host configuration. |
| Serilog host logger | Active | Reads app configuration and writes Debug, Console, File and optional Seq. |
| Serilog request logging | Active | Emits handled request logs without full query string. |
| Custom Serilog middleware | Active | Emits request completion and error context with whitelisted headers only. |
| MVC action filter | Active | Adds route/action/model validation context. |
| Exception handlers | Active | Expected and unexpected exceptions mapped to Problem Details and server logs. |

## Metrics

| Source | Current State | Gap Closed |
| --- | --- | --- |
| ASP.NET Core hosting/server | Not exported before prompt | Registered through OpenTelemetry ASP.NET Core instrumentation and framework meters. |
| HttpClient | Not exported before prompt | Registered through OpenTelemetry HttpClient instrumentation and `System.Net.Http` meters. |
| Runtime | Not exported before prompt | Registered through runtime instrumentation. |
| EF Core | Not exported before prompt | `Microsoft.EntityFrameworkCore` meter registered when available. |
| Custom domain meters | None | Not added in this baseline. |

## Traces

| Source | Current State | Gap Closed |
| --- | --- | --- |
| ASP.NET Core requests | Framework Activity only, no OTel provider | Added ASP.NET Core tracing instrumentation. |
| HttpClient | No OTel provider | Added HttpClient tracing instrumentation. |
| EF Core | No OTel provider | Added EF Core tracing instrumentation. |
| Redis | Not instrumented | Not added; package maturity/design mismatch recorded. |
| Custom ActivitySource | None | Registered baseline source name for future custom spans. |

## Health Checks

| Check | Current State | Notes |
| --- | --- | --- |
| `/health/live` | Active | Minimal status only. |
| `/health/ready` | Active | Detailed only in Development/Testing. |
| `/hc` | Active | Legacy minimal alias. |
| SQL Server | Active readiness check | Names kept descriptive and vendor-neutral. |
| Redis | Active when cache enabled | No OTel Redis tracing added in this prompt. |
| Seq URL | Optional when `SeqSettings:Enabled=true` | Renamed to `Seq Log`. |

## Gaps

- No custom domain spans or meters yet.
- No Redis spans because the supported package is pre-release and the active cache abstraction does not expose the required multiplexer.
- No collector-backed validation because a collector stack is out of scope.
- No logs exported through OpenTelemetry logging provider to avoid duplicating the Serilog pipeline.

## Duplications Removed

- Product-specific Seq settings name removed from active code/configuration.
- Seq sink no longer registers unconditionally.
- Health check UI database artifact and captured log artifact were removed from active API project files.
