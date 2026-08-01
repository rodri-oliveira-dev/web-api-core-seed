# Discovery - Prompt 02

## Baseline

- Branch inicial: `phase/4-architecture-modernization`.
- Working tree inicial: limpa.
- Ultimos commits:
  - `27abd76 refactor: adopt modular hexagonal architecture`
  - `18af517 ci: add quality and security workflows`
  - `4b493e2 feat: add OpenTelemetry observability`
- `dotnet build --configuration Release`: passou com 34 warnings de analyzers ja existentes.
- `dotnet test --configuration Release --no-build`: passou com 47 testes em `WebApiCoreSeed.Tests` e 26 em `WebApiCoreSeed.IntegrationTests`.

## Projetos ativos

| Projeto atual | Responsabilidade real |
| --- | --- |
| `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | Composition root, hosting, configuracao reutilizavel, Identity, OpenAPI, controllers e view models do exemplo. |
| `src/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj` | Dominio e aplicacao do exemplo de restaurante. |
| `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | Infraestrutura EF Core do exemplo de restaurante, incluindo DbContext, mappings, repositorios e migrations. |
| `test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj` | Testes unitarios do exemplo e testes leves de configuracao/contrato. |
| `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj` | Testes HTTP e infraestrutura real com SQL Server/Redis via Testcontainers. |
| `tools/OpenApiGenerator/OpenApiGenerator.csproj` | Geracao dos contratos OpenAPI versionados. |

## Nomes encontrados

| Busca | Resultado |
| --- | --- |
| `WebApiCoreSeed` | Encontrado em solution, projetos, namespaces, migrations, testes, OpenAPI e docs historicas. |
| `Datasul` | Apenas `LEGACY.md` e SDD antigo; nenhuma ocorrencia ativa. |
| `SampleRestaurantDbContext` | Encontrado em API DI/hosting, Data, migrations, testes e OpenAPI tool. |
| `Restaurant` | Encontrado como modulo fisico `Modules/SampleRestaurant` e testes arquiteturais. |
| `Sample` | Apenas uso tecnico de OpenTelemetry sampler; nao havia nome de modulo de exemplo. |
| `Seed` | Encontrado corretamente em nomes reutilizaveis `WebApiCoreSeed`, telemetry, docs e testes de integracao. |

## Observacoes

- A palavra `WebApiCoreSeed` nos documentos OpenAPI atuais descreve o contrato do exemplo e nao deve ser neutralizada sem registrar mudanca de contrato/documentacao. Ela sera ajustada para deixar claro que e uma API de exemplo.
- Rotas `/api/v{version}/Pratos` e `/api/v{version}/Mesas` pertencem ao sample e devem permanecer.
- O nome `SampleRestaurantDb` na connection string e um nome de banco legado do exemplo; sera renomeado para `SampleRestaurantDb` em configuracao ativa e testes.
- `DevIO` aparece como nome de diretorio ativo e deve sair do caminho de projetos ativos para remover traco de POC anterior.
- Migrations antigas usam nomes de tipos em snapshot/designer; os ajustes de namespace e DbContext sao inevitaveis para manter compilacao e design-time coerentes.
