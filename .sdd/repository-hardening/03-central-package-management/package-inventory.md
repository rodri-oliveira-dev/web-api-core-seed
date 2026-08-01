# Package Inventory

## Inventario direto

| Package | Consumidores | Versoes iniciais | Escopo CPM | Conflito | Acao | Justificativa |
| --- | --- | --- | --- | --- | --- | --- |
| AspNetCore.HealthChecks.Redis | Api | 9.0.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Pacote produtivo exclusivo da API. |
| AspNetCore.HealthChecks.SqlServer | Api | 9.0.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Pacote produtivo exclusivo da API. |
| AspNetCore.HealthChecks.UI.Client | Api | 9.0.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Cliente produtivo de health checks. |
| AspNetCore.HealthChecks.Uris | Api | 9.0.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Pacote produtivo exclusivo da API. |
| Asp.Versioning.Mvc | Api | 10.0.1 | src | Nao | Centralizar em `src/Directory.Packages.props` | Versionamento HTTP produtivo. |
| Asp.Versioning.Mvc.ApiExplorer | Api | 10.0.1 | src | Nao | Centralizar em `src/Directory.Packages.props` | OpenAPI/versionamento produtivo. |
| Asp.Versioning.OpenApi | Api | 10.0.1 | src | Nao | Centralizar em `src/Directory.Packages.props` | OpenAPI/versionamento produtivo. |
| AutoMapper | Api | 16.2.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Mapeamento produtivo. |
| FluentValidation | SampleRestaurant, transitive em infraestrutura/API/testes via referencias | 12.1.1 | src | Nao | Centralizar em `src/Directory.Packages.props` | Regra produtiva do modulo. |
| KubernetesClient | Api | 19.0.2 | src | Nao | Centralizar em `src/Directory.Packages.props` e preservar `PrivateAssets` no projeto | Dependencia produtiva privada da API. |
| Microsoft.AspNetCore.Authentication.JwtBearer | Api | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Pacote de plataforma produtivo. |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | Api, Identity.Infrastructure | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Pacote de plataforma produtivo compartilhado em `src`. |
| Microsoft.AspNetCore.Identity.UI | Api | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Pacote de plataforma produtivo. |
| Microsoft.AspNetCore.OpenApi | Api | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Pacote de plataforma produtivo. |
| Microsoft.AspNetCore.Mvc.Testing | UnitTests, IntegrationTests, OpenApiGenerator | 10.0.10 | raiz | Nao | Centralizar no arquivo raiz | Usado em `tests/` e `tools/`. |
| Microsoft.EntityFrameworkCore | Api | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Pacote de plataforma produtivo. |
| Microsoft.EntityFrameworkCore.Design | SampleRestaurant.Infrastructure, Identity.Infrastructure | 10.0.10 | src | Nao | Centralizar e preservar `PrivateAssets`/`IncludeAssets` nos projetos | Tooling de EF em projetos produtivos de infraestrutura. |
| Microsoft.EntityFrameworkCore.InMemory | UnitTests, OpenApiGenerator | 10.0.10 | raiz | Nao | Centralizar no arquivo raiz | Usado em `tests/` e `tools/`. |
| Microsoft.EntityFrameworkCore.SqlServer | SampleRestaurant.Infrastructure, Identity.Infrastructure | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Banco produtivo das infraestruturas. |
| Microsoft.EntityFrameworkCore.Tools | Api | 10.0.10 | src | Nao | Centralizar e preservar `PrivateAssets`/`IncludeAssets` no projeto | Ferramenta EF vinculada ao projeto produtivo da API. |
| Microsoft.Extensions.Caching.StackExchangeRedis | Api | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Cache produtivo. |
| Microsoft.Extensions.Configuration.EnvironmentVariables | SampleRestaurant.Infrastructure, Identity.Infrastructure | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Configuracao produtiva das infraestruturas. |
| Microsoft.Extensions.Configuration.Json | SampleRestaurant.Infrastructure, Identity.Infrastructure | 10.0.10 | src | Nao | Centralizar usando `$(MicrosoftDotNetPackageVersion)` | Configuracao produtiva das infraestruturas. |
| OpenTelemetry.Exporter.OpenTelemetryProtocol | Api | 1.17.0 | src | Nao | Centralizar usando `$(OpenTelemetryPackageVersion)` | Observabilidade produtiva. |
| OpenTelemetry.Extensions.Hosting | Api | 1.17.0 | src | Nao | Centralizar usando `$(OpenTelemetryPackageVersion)` | Observabilidade produtiva. |
| OpenTelemetry.Instrumentation.AspNetCore | Api | 1.17.0 | src | Nao | Centralizar usando `$(OpenTelemetryPackageVersion)` | Observabilidade produtiva. |
| OpenTelemetry.Instrumentation.EntityFrameworkCore | Api | 1.17.0-beta.1 | src | Nao | Manter beta explicitamente em `src/Directory.Packages.props` | Nao houve upgrade silencioso; pacote foi reportado como nao encontrado nas fontes pelo outdated. |
| OpenTelemetry.Instrumentation.Http | Api | 1.17.0 | src | Nao | Centralizar usando `$(OpenTelemetryPackageVersion)` | Observabilidade produtiva. |
| OpenTelemetry.Instrumentation.Runtime | Api | 1.17.0 | src | Nao | Centralizar usando `$(OpenTelemetryPackageVersion)` | Observabilidade produtiva. |
| Scalar.AspNetCore | Api | 2.16.17 | src | Nao | Centralizar em `src/Directory.Packages.props` | UI/OpenAPI produtivo. |
| Serilog.AspNetCore | Api | 10.0.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Logging produtivo. |
| Serilog.Expressions | Api | 5.0.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Logging produtivo. |
| Serilog.Sinks.Console | Api | 6.1.1 | src | Nao | Centralizar em `src/Directory.Packages.props` | Logging produtivo. |
| Serilog.Sinks.Seq | Api | 9.1.0 | src | Nao | Centralizar em `src/Directory.Packages.props` | Logging produtivo. |
| Bogus | UnitTests | 35.6.5 | tests | Nao | Centralizar em `tests/Directory.Packages.props` | Dados de teste. |
| coverlet.collector | UnitTests, IntegrationTests | 10.0.1, 6.0.4 | tests | Sim | Alinhar em `10.0.1` | Resolve divergencia antiga; major em integracao registrado e validado. |
| Microsoft.NET.Test.Sdk | UnitTests, IntegrationTests | 18.8.1, 17.14.1 | tests | Sim | Alinhar em `18.8.1` | Resolve divergencia antiga; major em integracao registrado e validado. |
| Moq | UnitTests | 4.20.72 | tests | Nao | Centralizar em `tests/Directory.Packages.props` | Mocking de teste. |
| StackExchange.Redis | IntegrationTests | 3.1.0 | tests | Nao | Centralizar em `tests/Directory.Packages.props` | Cliente direto apenas nos testes de integracao. |
| Testcontainers.MsSql | IntegrationTests | 4.13.0 | tests | Nao | Centralizar em `tests/Directory.Packages.props` | Infraestrutura de teste. |
| Testcontainers.Redis | IntegrationTests | 4.13.0 | tests | Nao | Centralizar em `tests/Directory.Packages.props` | Infraestrutura de teste. |
| xunit | UnitTests, IntegrationTests | 2.9.3 | tests | Nao | Manter `2.9.3` em `tests/Directory.Packages.props` | Migração para `xunit.v3` e major fora do escopo. |
| xunit.runner.visualstudio | UnitTests, IntegrationTests | 3.1.5, 3.1.4 | tests | Sim | Alinhar em `3.1.5` | Resolve divergencia antiga com patch update. |

## Inventario de saude

- Vulneraveis: nenhum pacote direto vulneravel reportado.
- Deprecated: `xunit` `2.9.3` reportado como `Legacy` em testes unitarios e de integracao.
- Outdated final:
  - nenhum pacote de teste divergente permaneceu outdated;
  - `OpenTelemetry.Instrumentation.EntityFrameworkCore` `1.17.0-beta.1` segue reportado como `Nao encontrado nas fontes`.

## Lock files versionados

Foram gerados `packages.lock.json` para todos os projetos ativos:

- `src/WebApiCoreSeed.Api/packages.lock.json`
- `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/packages.lock.json`
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/packages.lock.json`
- `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/packages.lock.json`
- `tests/WebApiCoreSeed.UnitTests/packages.lock.json`
- `tests/WebApiCoreSeed.IntegrationTests/packages.lock.json`
- `tools/OpenApiGenerator/packages.lock.json`
