# Architecture Map - Phase 4

## Estado atual

| Area | Projeto | Responsabilidade real |
| --- | --- | --- |
| API | `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | Hosting, controllers, autenticacao, autorizacao, Problem Details, OpenAPI, rate limiting, health checks, cache HTTP/Redis e composition root. |
| SampleRestaurant | `src/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj` | Entidades, validadores, notificacoes, interfaces de repositorio, interfaces de servico e services de aplicacao do dominio demonstrativo. |
| SampleRestaurant Infrastructure | `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | EF Core, `SampleRestaurantDbContext`, mappings, migrations de dominio demonstrativo e repositorios concretos. |
| Unit/API tests | `test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj` | Testes unitarios, testes leves de contrato e configuracao com `WebApplicationFactory`. |
| Integration tests | `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj` | Testes HTTP e infraestrutura real com SQL Server e Redis via Testcontainers. |
| OpenAPI tool | `tools/OpenApiGenerator/OpenApiGenerator.csproj` | Geracao de contratos OpenAPI versionados. |

## Direcao alvo

| Papel Hexagonal | Estado nesta entrega |
| --- | --- |
| Domain | Tipos de dominio do modulo `SampleRestaurant` permanecem no assembly do sample, organizados em subestrutura de modulo. |
| Application | Casos de uso e portas de entrada do modulo `SampleRestaurant` permanecem no assembly do sample, separados fisicamente dos tipos de dominio. |
| Output ports | Interfaces de repositorio continuam no nucleo como portas temporarias ate o Prompt 3. |
| Infrastructure | EF Core e repositorios concretos permanecem no assembly de infraestrutura do sample, como adaptadores de saida. |
| Input adapters | Controllers permanecem no assembly Api e devem depender de casos de uso/portas de entrada, nao de repositorios. |
| Composition root | `Program.cs` e configuracoes de DI permanecem no assembly Api. |

## Limites temporarios

- O modulo `Identity` ainda esta hospedado dentro da API por causa do ASP.NET Core Identity.
- As migrations do Identity ainda ficam no projeto API; a correcao esta reservada para o Prompt 6.
- O repositorio generico e a unidade de trabalho implicita continuam temporarios ate os Prompts 3 e 4.
- A paginacao ainda usa a implementacao legada ate o Prompt 7.

## Estrutura apos o prompt 02

```text
src/
|-- WebApiCoreSeed.Api/
|-- SampleRestaurant/
|   `-- Modules/
|       `-- SampleRestaurant/
|           |-- Domain/
|           |   `-- Models/
|           `-- Application/
|               |-- Contracts/
|               |-- Notifications/
|               |-- Ports/
|               `-- UseCases/
`-- SampleRestaurant.Infrastructure/
    `-- Modules/
        `-- SampleRestaurant/
            `-- Infrastructure/
                `-- Persistence/
```

Namespaces ativos usam `WebApiCoreSeed.Api`, `WebApiCoreSeed.SampleRestaurant` e `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
