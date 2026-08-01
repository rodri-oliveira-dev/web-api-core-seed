# Architecture Map - Phase 4

## Estado inicial

| Area | Projeto | Responsabilidade real |
| --- | --- | --- |
| API | `src/DevIO.Api/Restaurante.IO.Api.csproj` | Hosting, controllers, autenticacao, autorizacao, Problem Details, OpenAPI, rate limiting, health checks, cache HTTP/Redis e composition root. |
| Business | `src/DevIO.Business/Restaurante.IO.Business.csproj` | Entidades, validadores, notificacoes, interfaces de repositorio, interfaces de servico e services de aplicacao legados. |
| Data | `src/DevIO.Data/Restaurante.IO.Data.csproj` | EF Core, `MeuDbContext`, mappings, migrations de dominio e repositorios concretos. |
| Unit/API tests | `test/Pedidos.Test/Pedidos.Test.csproj` | Testes unitarios, testes leves de contrato e configuracao com `WebApplicationFactory`. |
| Integration tests | `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj` | Testes HTTP e infraestrutura real com SQL Server e Redis via Testcontainers. |
| OpenAPI tool | `tools/OpenApiGenerator/OpenApiGenerator.csproj` | Geracao de contratos OpenAPI versionados. |

## Direcao alvo

| Papel Hexagonal | Estado nesta entrega |
| --- | --- |
| Domain | Tipos de dominio do modulo `Restaurant` permanecem no assembly Business, organizados em subestrutura de modulo. |
| Application | Casos de uso e portas de entrada do modulo `Restaurant` permanecem no assembly Business, separados fisicamente dos tipos de dominio. |
| Output ports | Interfaces de repositorio continuam no nucleo como portas temporarias ate o Prompt 3. |
| Infrastructure | EF Core e repositorios concretos permanecem no assembly Data, como adaptadores de saida do modulo. |
| Input adapters | Controllers permanecem no assembly Api e devem depender de casos de uso/portas de entrada, nao de repositorios. |
| Composition root | `Program.cs` e configuracoes de DI permanecem no assembly Api. |

## Limites temporarios

- O modulo `Identity` ainda esta hospedado dentro da API por causa do ASP.NET Core Identity.
- As migrations do Identity ainda ficam no projeto API; a correcao esta reservada para o Prompt 6.
- O repositorio generico e a unidade de trabalho implicita continuam temporarios ate os Prompts 3 e 4.
- A paginacao ainda usa a implementacao legada ate o Prompt 7.

## Estrutura apos o prompt 01

```text
src/
├── DevIO.Api/
├── DevIO.Business/
│   └── Modules/
│       └── Restaurant/
│           ├── Domain/
│           │   └── Models/
│           └── Application/
│               ├── Contracts/
│               ├── Notifications/
│               ├── Ports/
│               └── UseCases/
└── DevIO.Data/
    └── Modules/
        └── Restaurant/
            └── Infrastructure/
                └── Persistence/
```

Namespaces legados foram preservados por compatibilidade interna.
