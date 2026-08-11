# Architecture Map - Phase 4

## Estado atual

| Area | Projeto | Responsabilidade real |
| --- | --- | --- |
| API | `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | Hosting, controllers, autenticacao, autorizacao, Problem Details, OpenAPI, rate limiting, health checks, cache HTTP/Redis e composition root. |
| Identity Infrastructure | `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj` | EF Core Identity, `ApplicationDbContext`, migrations de Identity e factory design-time. |
| SampleRestaurant | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj` | Entidades, validadores, notificacoes, contratos de paginacao/query, portas de repositorio, portas de servico e services de aplicacao do dominio demonstrativo. |
| SampleRestaurant Infrastructure | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | EF Core, `SampleRestaurantDbContext`, mappings, migrations de dominio demonstrativo, indices e repositorios concretos. |
| Unit/API tests | `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj` | Testes unitarios, testes leves de contrato e configuracao com `WebApplicationFactory`. |
| Integration tests | `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj` | Testes HTTP e infraestrutura real com SQL Server e Redis via Testcontainers. |
| OpenAPI tool | `tools/OpenApiGenerator/OpenApiGenerator.csproj` | Geracao de contratos OpenAPI versionados. |

## Direcao alvo

| Papel Hexagonal | Estado nesta entrega |
| --- | --- |
| Domain | Tipos de dominio do modulo `SampleRestaurant` permanecem no assembly do sample, organizados em subestrutura de modulo. |
| Application | Casos de uso e portas de entrada do modulo `SampleRestaurant` permanecem no assembly do sample, separados fisicamente dos tipos de dominio. |
| Output ports | Interfaces especificas de repositorio e Unit of Work ficam no nucleo do modulo como portas de saida. |
| Infrastructure | EF Core e repositorios concretos permanecem no assembly de infraestrutura do sample, como adaptadores de saida. |
| Input adapters | Controllers permanecem no assembly Api e devem depender de casos de uso/portas de entrada, nao de repositorios. |
| Composition root | `Program.cs` e configuracoes de DI permanecem no assembly Api. |

## Limites temporarios

- O modulo `Identity` ainda tem endpoints e application flow hospedados na API por causa do ASP.NET Core Identity.
- A persistencia e as migrations de Identity ficam em `WebApiCoreSeed.Identity.Infrastructure`.
- Services e repositories ainda preservam `IDisposable` legado.
- A paginacao ativa de pratos agora e deterministica e limitada por offset pagination.

## Estrutura apos o prompt 02

```text
src/
|-- WebApiCoreSeed.Api/
`-- Modules/
    |-- Identity/
    |   `-- WebApiCoreSeed.Identity.Infrastructure/
    |       |-- Context/
    |       `-- Migrations/
    `-- SampleRestaurant/
        |-- WebApiCoreSeed.SampleRestaurant/
        |   `-- Modules/
        |       `-- SampleRestaurant/
        |           |-- Domain/
        |           |   `-- Models/
        |           `-- Application/
        |               |-- Contracts/
        |               |-- Notifications/
        |               |-- Ports/
        |               `-- UseCases/
        `-- WebApiCoreSeed.SampleRestaurant.Infrastructure/
            `-- Modules/
                `-- SampleRestaurant/
                    `-- Infrastructure/
                        `-- Persistence/
```

Namespaces ativos usam `WebApiCoreSeed.Api`, `WebApiCoreSeed.Identity.Infrastructure`, `WebApiCoreSeed.SampleRestaurant`, `WebApiCoreSeed.SampleRestaurant.Infrastructure`, `WebApiCoreSeed.UnitTests` e `WebApiCoreSeed.IntegrationTests`.
