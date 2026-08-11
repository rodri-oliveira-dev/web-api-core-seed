# Migration Map

| Origem | Destino | Tipo | Acao | Risco | Validacao |
| --- | --- | --- | --- | --- | --- |
| `src/SampleRestaurant/` | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/` | Projeto | `git mv` e atualizar referencias | Project references quebradas | Restore/build/testes arquiteturais |
| `src/SampleRestaurant.Infrastructure/` | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/` | Projeto EF sample | `git mv` e atualizar referencias | EF CLI nao localizar migrations | `dotnet ef migrations list --context SampleRestaurantDbContext` |
| `src/Identity.Infrastructure/` | `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/` | Projeto EF Identity | `git mv` e atualizar referencias | EF CLI nao localizar migrations | `dotnet ef migrations list --context ApplicationDbContext` |
| `test/WebApiCoreSeed.Tests/` | `tests/WebApiCoreSeed.UnitTests/` | Projeto de testes | `git mv`, renomear `.csproj` e namespaces | Filtros por namespace antigo mudam | Testes unitarios e arquiteturais |
| `test/WebApiCoreSeed.IntegrationTests/` | `tests/WebApiCoreSeed.IntegrationTests/` | Projeto de testes | `git mv` | CI e comandos antigos quebram | Testes de integracao |
| `tools/OpenApiGenerator/` | `tools/OpenApiGenerator/` | Tooling | Manter pasta/projeto | Nenhum ganho em mover | Geracao OpenAPI e diff |

## Migrations

| DbContext | Migration IDs | Projeto antes | Projeto depois | Namespace | Acao |
| --- | --- | --- | --- | --- | --- |
| `SampleRestaurantDbContext` | `20200817223231_InitialCreate`, `20260801191447_AddPratosPaginationOrderingIndex` | `src/SampleRestaurant.Infrastructure` | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure` | `WebApiCoreSeed.SampleRestaurant.Infrastructure.Migrations` | Mover arquivos com o projeto; nao alterar IDs, classes ou operacoes. |
| `ApplicationDbContext` | `20200817223121_InitialCreate` | `src/Identity.Infrastructure` | `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure` | `WebApiCoreSeed.Identity.Infrastructure.Migrations` | Mover arquivos com o projeto; nao alterar IDs, classes ou operacoes. |
