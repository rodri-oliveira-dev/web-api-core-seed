# Validation

## Resultado

| Comando | Resultado |
| --- | --- |
| `dotnet restore WebApiCoreSeed.sln` | Passou. Todos os projetos foram restaurados nos novos caminhos. |
| `dotnet build WebApiCoreSeed.sln --configuration Release --no-restore` | Passou com 31 warnings de analyzer ja existentes; 0 erros. |
| `dotnet test WebApiCoreSeed.sln --configuration Release --no-build` | Passou: 53 testes em `WebApiCoreSeed.UnitTests` e 42 em `WebApiCoreSeed.IntegrationTests`. |
| `dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~Arquitetura"` | Passou: 7 testes arquiteturais. |
| `dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~Unitarios"` | Passou: 27 testes unitarios. |
| `dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "Category=Integration"` | Passou: 42 testes de integracao/container. |
| `dotnet ef dbcontext list` | Falhou na raiz com `No project was found`; o comando raiz nao e suficiente porque nao ha `.csproj` no diretorio atual. |
| `dotnet ef migrations list --context SampleRestaurantDbContext` | Falhou na raiz com `No project was found`; requer `--project`. |
| `dotnet ef migrations list --context ApplicationDbContext` | Falhou na raiz com `No project was found`; requer `--project`. |
| `dotnet ef dbcontext list --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build` | Passou; listou `WebApiCoreSeed.SampleRestaurant.Infrastructure.Context.SampleRestaurantDbContext`. |
| `dotnet ef dbcontext list --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build` | Passou; listou `WebApiCoreSeed.Identity.Infrastructure.Context.ApplicationDbContext`. |
| `dotnet ef migrations list --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --configuration Release --no-build --no-connect` | Passou; listou `20200817223231_InitialCreate` e `20260801191447_AddPratosPaginationOrderingIndex`. |
| `dotnet ef migrations list --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --configuration Release --no-build --no-connect` | Passou; listou `20200817223121_InitialCreate`. |
| `dotnet ef migrations has-pending-model-changes ... SampleRestaurantDbContext` | Passou; sem mudancas pendentes no modelo. |
| `dotnet ef migrations has-pending-model-changes ... ApplicationDbContext` | Passou; sem mudancas pendentes no modelo. |
| `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build` | Passou; gerou `docs/openapi/openapi-v1.json` e `docs/openapi/openapi-v2.json`. |
| `git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json` | Passou; OpenAPI sem diff. |
| Smoke local da API em `http://127.0.0.1:5099` | Passou; `/openapi/v1.json` e `/hc` responderam HTTP 200. |

## Buscas de referencias antigas

| Busca | Resultado |
| --- | --- |
| `git grep -n 'WebApiCoreSeed.Tests'` | Somente SDD historico de fases/prompts anteriores e inventario deste prompt. |
| `git grep -n -E 'src/SampleRestaurant|src\\SampleRestaurant'` | Somente SDD historico de fases/prompts anteriores e inventario deste prompt. |
| `git grep -n -E 'src/Identity.Infrastructure|src\\Identity.Infrastructure'` | Somente SDD historico de fases/prompts anteriores. |
| `git grep -n -E 'test/|test\\'` | Somente `LEGACY.md` e SDD historico/inventarios; referencias ativas foram movidas para `tests/`. |

## Observacoes

- Os comandos EF sem `--project` continuam invalidos a partir da raiz da solution; a forma operacional correta esta registrada em `.sdd/phase-4/06-infrastructure-migrations/command-reference.md`.
- Nenhum arquivo OpenAPI ficou alterado apos a geracao.
- Migrations antigas nao foram editadas para renomeacao cosmetica.
