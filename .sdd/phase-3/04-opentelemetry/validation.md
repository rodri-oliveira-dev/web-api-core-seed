# Validation - 04 OpenTelemetry

## Initial Baseline

```text
dotnet build --configuration Release
dotnet test --configuration Release --no-build
```

Result:

- Build passed with existing analyzer warnings.
- Tests passed: 36 in `Pedidos.Test`, 26 in `WebApiCoreSeed.IntegrationTests`.

## Focused Validation During Development

```text
dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --filter ObservabilityConfigurationTests
```

Result:

- Passed: 5 tests.

Covered:

- API starts with OpenTelemetry disabled.
- API starts with OTLP configured and no collector available.
- ASP.NET Core request creates a server Activity.
- Serilog file logs include `TraceId` and `SpanId`.
- Captured Activity tag values do not contain sensitive query/header values.

## Final Validation

```text
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet list package
git grep -n -i "Datasul"
git grep -n "OpenTelemetry"
git grep -n "TraceId"
git grep -n "SpanId"
```

Results:

- `dotnet restore`: passed.
- `dotnet build --configuration Release --no-restore`: passed with 21 existing analyzer warnings and 0 errors.
- `dotnet test --configuration Release --no-build`: passed.
  - `Pedidos.Test`: 41 passed.
  - `WebApiCoreSeed.IntegrationTests`: 26 passed.
- `dotnet list package`: passed and listed the OpenTelemetry packages documented in `report.md`.
- `git grep -n -i "Datasul"`: returned only historical references in `LEGACY.md` and older SDD files; no active code or active configuration references remain.
- `git grep -n "OpenTelemetry"`: returned expected active configuration, package, code, tests and SDD references.
- `git grep -n "TraceId"`: returned expected Serilog template, Problem Details trace id and SDD/README references.
- `git grep -n "SpanId"`: returned expected Serilog template and SDD/README references.

## Smoke Coverage

Covered by automated tests:

- API without OTLP.
- API with OTLP exporter configured to a local endpoint where no collector is available.
- HTTP request pipeline.
- Controlled error and unexpected error behavior from existing Problem Details tests.
- SQL Server and Redis readiness through existing container integration tests.

Limitations:

- No collector-backed export assertion, because adding a collector stack is out of scope.
- Redis tracing is not enabled; Redis health/readiness and integration behavior remain validated.
