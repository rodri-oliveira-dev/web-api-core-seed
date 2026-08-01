# Handoff - Phase 4

## Estado final do prompt 02

- Branch atual: `phase/4-architecture-modernization`.
- Branch-base: `phase/3-quality-and-safety`.
- Commit-base da fase: `18af517adab5d21ae58ac9674da411244a5379b9`.
- Prompt atual: `02 - Separacao do dominio de exemplo` concluido.
- Commit esperado: `refactor: separate sample domain from reusable seed`.
- Push: nao realizado.
- PR: nao realizado.

## Estrutura atual

- `WebApiCoreSeed.sln`
- `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`
- `src/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj`
- `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj`
- `test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj`
- `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`
- `src/SampleRestaurant/Modules/SampleRestaurant/Domain/Models`
- `src/SampleRestaurant/Modules/SampleRestaurant/Application/Contracts/Pagination`
- `src/SampleRestaurant/Modules/SampleRestaurant/Application/Notifications`
- `src/SampleRestaurant/Modules/SampleRestaurant/Application/Ports/Inbound`
- `src/SampleRestaurant/Modules/SampleRestaurant/Application/Ports/Outbound`
- `src/SampleRestaurant/Modules/SampleRestaurant/Application/UseCases`
- `src/SampleRestaurant.Infrastructure/Modules/SampleRestaurant/Infrastructure/Persistence`

## Modulos

- `SampleRestaurant`: dominio demonstrativo, contendo pratos, mesas, pedidos, itens de pedido, atendentes e log legado.
- `Identity`: capacidade registrada no catalogo, ainda imatura e hospedada na API por dependencia direta do ASP.NET Core Identity.

## Dependencias

- `WebApiCoreSeed.Api` referencia `WebApiCoreSeed.SampleRestaurant` e `WebApiCoreSeed.SampleRestaurant.Infrastructure` para composicao.
- `WebApiCoreSeed.SampleRestaurant.Infrastructure` referencia `WebApiCoreSeed.SampleRestaurant` para implementar portas de saida.
- `WebApiCoreSeed.SampleRestaurant` nao referencia API, infraestrutura, ASP.NET Core, EF Core, Redis nem logging.

## Nomes anteriores e novos

| Anterior | Novo |
| --- | --- |
| `RestauranteAPI.sln` | `WebApiCoreSeed.sln` |
| `Restaurante.IO.Api` | `WebApiCoreSeed.Api` |
| `Restaurante.IO.Business` | `WebApiCoreSeed.SampleRestaurant` |
| `Restaurante.IO.Data` | `WebApiCoreSeed.SampleRestaurant.Infrastructure` |
| `src/DevIO.Api` | `src/WebApiCoreSeed.Api` |
| `src/DevIO.Business` | `src/SampleRestaurant` |
| `src/DevIO.Data` | `src/SampleRestaurant.Infrastructure` |
| `Modules/Restaurant` | `Modules/SampleRestaurant` |
| `MeuDbContext` | `SampleRestaurantDbContext` |
| `Pedidos.Test` | `WebApiCoreSeed.Tests` |
| `PedidosApi` | `SampleRestaurantDb` |

## Contratos preservados

- Rotas, payloads, status codes, autenticacao, autorizacao, Problem Details, rate limiting e health checks preservados.
- Rotas do sample, como `/api/v{version}/Pratos` e `/api/v{version}/Mesas`, permanecem com vocabulário do exemplo.
- OpenAPI regenerado por `tools/OpenApiGenerator`; titulo atualizado para `Sample Restaurant API`.

## Debitos temporarios

- Repositorio generico permanece como porta de saida temporaria ate o Prompt 3.
- Unit of Work implicito permanece ate o Prompt 4.
- CancellationToken ainda nao foi propagado de ponta a ponta; Prompt 5.
- Migrations de Identity ainda ficam na API; Prompt 6.
- Migrations do sample permaneceram no projeto de infraestrutura com ajuste de namespace/tipo; mover ownership definitivo fica para o Prompt 6.
- Paginacao ainda e a implementacao legada; Prompt 7.

## Validacao final

- `dotnet restore WebApiCoreSeed.sln`: passou.
- `dotnet build WebApiCoreSeed.sln --configuration Release --no-restore`: passou com 34 warnings de analyzers ja existentes.
- `dotnet test WebApiCoreSeed.sln --configuration Release --no-build`: passou, 47 testes em `WebApiCoreSeed.Tests` e 26 em `WebApiCoreSeed.IntegrationTests`.
- `dotnet test test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj --configuration Release --no-build --filter Architecture`: passou, 6 testes.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration`: passou, 26 testes.
- `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build`: passou e regenerou `docs/openapi/openapi-v1.json` e `docs/openapi/openapi-v2.json`.
- Smoke/regressao HTTP: cobertos pelas suites `WebApiCoreSeed.Tests`, `WebApiCoreSeed.IntegrationTests` e pela geracao OpenAPI.
- `git grep -n -i -E "Datasul|MeuDbContext"`: apenas referencias historicas em `LEGACY.md` e SDD antigo.

## Proxima issue

- Proxima issue/prompt: `#14`, iniciando o Prompt 3 da Fase 4.

## Proximos prompts restantes

- `03 - Portas de persistencia`: pendente.
- `04 - Unit of Work`: pendente.
- `05 - CancellationToken`: pendente.
- `06 - Migrations na infraestrutura`: pendente.
- `07 - Paginacao deterministica`: pendente.

## Observacoes

- A solucao ativa permanece `WebApiCoreSeed.sln`.
- Os projetos ativos miram `net10.0`.
- As skills DDD citadas no prompt nao estavam instaladas nesta sessao; foram usadas as skills locais aplicaveis de SDD, mudanca .NET, integracao .NET e refatoracao .NET.
