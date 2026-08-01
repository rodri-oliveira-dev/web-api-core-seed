# Validation - Prompt 02

## Comandos

| Comando | Resultado |
| --- | --- |
| `git status --short` | Inicialmente limpo antes das alteracoes. |
| `git branch --show-current` | `phase/4-architecture-modernization`. |
| `git log -3 --oneline` | `27abd76`, `18af517`, `4b493e2`. |
| `dotnet build --configuration Release` | Passou no baseline com 34 warnings existentes. |
| `dotnet test --configuration Release --no-build` | Passou no baseline: 47 + 26 testes. |
| `dotnet restore WebApiCoreSeed.sln` | Passou. |
| `dotnet build WebApiCoreSeed.sln --configuration Release --no-restore` | Passou com 34 warnings existentes. |
| `dotnet test WebApiCoreSeed.sln --configuration Release --no-build` | Passou: 47 testes em `WebApiCoreSeed.Tests` e 26 em `WebApiCoreSeed.IntegrationTests`. |
| `dotnet test test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj --configuration Release --no-build --filter Architecture` | Passou: 6 testes arquiteturais. |
| `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration` | Passou: 26 testes de integracao/container. |
| `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build` | Passou; `/openapi/v1.json` e `/openapi/v2.json` responderam 200 e os arquivos foram regenerados. |
| `git grep -n -i -E "Datasul\|MeuDbContext"` | Retornou apenas referencias historicas em `LEGACY.md` e SDD antigo. |

## Contaminacao por nomes

- `rg` em codigo ativo, testes, tooling, workflows, workspace e documentos vivos nao encontrou `Restaurante`, `MeuDbContext`, `DevIO`, `Pedidos.Test`, `RestauranteAPI`, `PedidosApi` ou `Modules/Restaurant`.
- `Datasul` permanece apenas em documentacao historica/contextual.

## Smoke e regressao

- Smoke HTTP de OpenAPI executado pela ferramenta `OpenApiGenerator`.
- Regressao de pipeline HTTP, Problem Details, rate limiting, security headers, health checks, SQL Server e Redis coberta pelas suites existentes.
