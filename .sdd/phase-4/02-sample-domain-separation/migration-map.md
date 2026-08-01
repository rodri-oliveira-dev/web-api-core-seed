# Migration Map - Prompt 02

| Origem | Destino | Tipo | Observacao |
| --- | --- | --- | --- |
| `WebApiCoreSeed.sln` | `WebApiCoreSeed.sln` | Tooling | Atualizar CI, VS Code, docs e handoff. |
| `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | Projeto | Composition root e componentes reutilizaveis. |
| `src/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj` | `src/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj` | Projeto | Dominio/aplicacao do sample. |
| `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | Projeto | Persistencia EF Core do sample. |
| `test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj` | `test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj` | Projeto | Testes unitarios, arquitetura e integracao leve. |
| `WebApiCoreSeed.Api.*` | `WebApiCoreSeed.Api.*` | Namespace | Inclui API, Identity, settings e recursos reutilizaveis. |
| `WebApiCoreSeed.SampleRestaurant.*` | `WebApiCoreSeed.SampleRestaurant.*` | Namespace | Inclui modelos, services, portas, notificacoes e paginacao atual. |
| `WebApiCoreSeed.SampleRestaurant.Infrastructure.*` | `WebApiCoreSeed.SampleRestaurant.Infrastructure.*` | Namespace | Inclui context, mappings, repositories e migrations. |
| `Modules/SampleRestaurant` | `Modules/SampleRestaurant` | Estrutura fisica | Sem alteracao de rotas. |
| `SampleRestaurantDbContext` | `SampleRestaurantDbContext` | Tipo EF Core | Ajustar DI, testes, tool e migrations metadata. |
| `SampleRestaurantDb` | `SampleRestaurantDb` | Configuracao | Nome de banco default do exemplo. |
