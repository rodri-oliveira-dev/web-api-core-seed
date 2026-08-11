# Discovery - 04 OpenTelemetry

## Baseline Commands

Initial state:

- `git status --short`: clean.
- `git branch --show-current`: `phase/3-quality-and-safety`.
- `git log -3 --oneline`:
  - `c9c4641 fix: harden API security defaults`
  - `e21a215 test: add API and infrastructure integration tests`
  - `d8730d3 test: strengthen existing unit test suite`
- `dotnet build --configuration Release`: passed with existing analyzer warnings.
- `dotnet test --configuration Release --no-build`: passed, 36 tests in `Pedidos.Test` and 26 tests in `WebApiCoreSeed.IntegrationTests`.

## Grep Results

- `Serilog`: active in `Program.cs`, `HostingConfig.cs`, `SerilogLoggingActionFilter.cs`, `SerilogMiddleware.cs`, appsettings and package references.
- `Seq`: active via Serilog sink, health check and settings. Legacy section name was product-specific.
- Legacy product-specific Seq name: active in settings, appsettings, integration tests and OpenAPI generator before this prompt.
- `Activity`: only used in `ApiProblemDetails` for trace id response extension.
- `Meter`: no active custom meters.
- `OpenTelemetry`: only roadmap/SDD mentions before this prompt.
- `AddHealthChecks`: active in `HostingConfig.cs`.
- `AddHttpClient`: no active registration found.
- `StackExchangeRedis`: active through `Microsoft.Extensions.Caching.StackExchangeRedis`.
- `DbContext`: active `ApplicationDbContext` and `MeuDbContext`, both SQL Server in production and replaced in tests where appropriate.

## Existing Logs

- Bootstrap Serilog logger in `Program.cs`.
- Host Serilog configuration in `HostingConfig.UseApiSerilog`.
- Serilog request logging middleware.
- Custom `SerilogMiddleware` with whitelisted headers for error context.
- MVC action filter enriches route/action/model-state metadata.
- Security prompt already removed full query string and raw target logging.

## Current Sinks

- Console.
- Debug.
- File.
- Seq package installed and preserved as optional sink.

Before this prompt, Seq was always registered as a sink even when the health check flag was disabled. This was changed so Seq is only added when `SeqSettings:Enabled=true`.

## Dependencies

- SQL Server through EF Core for identity and domain contexts.
- Redis through distributed cache and health check when enabled.
- Optional Seq URL.
- No explicit outbound `HttpClient` registrations, but the standard instrumentation is useful for future typed/named clients and any direct `HttpClient` usage.

## Sensitive Data Risks

- Query strings may contain tokens, passwords or personal data.
- Authorization headers and cookies must stay out of logs and telemetry attributes.
- EF Core command text and parameters may contain sensitive data and must not be added manually.
- Metric labels must avoid user, order, request or raw URL identifiers.

## Package Discovery

- Stable OpenTelemetry packages were available for Hosting, OTLP exporter, ASP.NET Core, HttpClient and Runtime at `1.17.0`.
- EF Core instrumentation had no stable package; the current official package was `1.17.0-beta.1`.
- Process instrumentation had no stable package and was not required because runtime metrics cover the baseline.
- StackExchange.Redis instrumentation was beta/pre-release and requires an `IConnectionMultiplexer` instance. The active cache uses `AddStackExchangeRedisCache`, which does not expose the multiplexer in the app DI contract.

## External Sources

- Microsoft Learn: .NET observability with OpenTelemetry.
- OpenTelemetry .NET docs: instrumentation libraries and best practices.
- NuGet/OpenTelemetry contrib package docs for EF Core and StackExchange.Redis maturity.
