Legacy preservation notice
==========================

> **Attention:** this repository preserves a legacy ASP.NET Core Web API originally built for .NET Core 3.1. .NET Core 3.1 reached end of support on December 13, 2022.

The maintained solution targets .NET 10 and uses `WebApiCoreSeed.slnx`. The original .NET Core 3.1 source remains available as a separate, immutable historical snapshot for migration comparisons.

Historical references:

- [`v1.0.0-legacy`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/v1.0.0-legacy) points at the original, unmodified source commit [`6ce03d7f`](https://github.com/rodri-oliveira-dev/web-api-core-seed/commit/6ce03d7f011c6809fbcbad47aa26d490f53ddf3d).
- [`legacy/netcoreapp3.1`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/legacy/netcoreapp3.1) retains the same legacy application code and adds an explicit unsupported-runtime warning to its README.

**The legacy branch and tag must not be used for new development or production deployments.**

See [LEGACY.md](LEGACY.md) for the documented legacy requirements, commands, migrations, seed status, limitations, and validation notes.

What is the Project?
=====================
The objective of this project was to implement the most commonly used technologies, and to share as a base project for WEB API. The original implementation was .NET Core 3.1; the active solution now targets .NET 10.

## Give a Star! :star:
If you liked the project or if project helped you, please give a star ;)

## How to use:
- You will need the latest Visual Studio 2019 and the latest .NET Core SDK.
- ***Please check if you have installed the same runtime version (SDK) described in global.json***
- The latest SDK and tools can be downloaded from https://dot.net/core.

Also you can run the Project in Visual Studio Code (Windows, Linux or MacOS).

To know more about how to setup your enviroment visit the [Microsoft .NET Download Guide](https://www.microsoft.com/net/download)

## Technologies implemented:

- .NET 10
- Preserved .NET Core 3.1 legacy baseline
- ASP.NET WebApi Core with JWT Bearer Authentication
- ASP.NET Identity Core
- Entity Framework Core 10
- .NET Core Native DI
- AutoMapper
- FluentValidation
- OpenAPI with Scalar UI and JWT support
- Health Checks
- Redis
- Native ASP.NET Core Rate Limiting
- OWASP Security
- Serilog
- Seq opcional

## Observability

OpenTelemetry is configured by the `OpenTelemetry` section and is vendor-neutral by default.

- `OpenTelemetry:Enabled`: enables traces and metrics. Default: `true`.
- `OpenTelemetry:ServiceName`: default service name: `web-api-core-seed-api`.
- `OpenTelemetry:ServiceNamespace`: default namespace: `rodri-oliveira-dev.web-api-core-seed`.
- `OpenTelemetry:Otlp:Enabled`: enables OTLP export. Default: `false`.
- `OpenTelemetry:Otlp:Endpoint`: optional collector endpoint, also compatible with `OTEL_EXPORTER_OTLP_ENDPOINT`.
- `OpenTelemetry:Otlp:Protocol`: `Grpc` or `HttpProtobuf`.

Serilog remains the structured logging pipeline. Console and file logs include `TraceId` and `SpanId` when a request activity is active. Seq is optional through `SeqSettings:Enabled`.

## Quality

Quality gates and local CI reproduction are documented in [docs/quality-gates.md](docs/quality-gates.md). Operational notes for SonarCloud setup, Dependabot-safe execution and Quality Gate protection are documented in [docs/quality/sonarcloud.md](docs/quality/sonarcloud.md).

## Development Seed

Development data is applied only by an explicit non-production command. Configure local secrets first, then run:

```bash
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed
```

The command applies EF Core migrations for Identity and SampleRestaurant, creates/updates a local development user, and upserts deterministic sample restaurant data. It is blocked in `Production` and requires `DevelopmentSeed:User:Password` from User Secrets or environment variables. See [containerized local development](docs/development/containerized-local-development.md#development-seed).

## Architecture:

- Full architecture with responsibility separation concerns, SOLID and Clean Code
- Domain Driven Design (Layers and Domain Model Pattern)
- Domain Events
- Domain Notification
- Unit of Work
- Repository and Generic Repository
