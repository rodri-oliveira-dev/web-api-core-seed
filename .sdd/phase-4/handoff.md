# Handoff - Phase 4

## Estado final do prompt 06

- Branch atual: `phase/4-architecture-modernization`.
- Branch-base: `phase/3-quality-and-safety`.
- Commit-base da fase: `18af517adab5d21ae58ac9674da411244a5379b9`.
- Prompt atual: `06 - Migrations na infraestrutura` concluido.
- Commit esperado: `refactor: move EF Core migrations to infrastructure`.
- Push: nao realizado.
- PR: nao realizado.

## Estrutura atual

- `WebApiCoreSeed.sln`
- `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`
- `src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`
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
- `src/Identity.Infrastructure/Context`
- `src/Identity.Infrastructure/Migrations`

## Modulos

- `SampleRestaurant`: dominio demonstrativo, contendo pratos, mesas, pedidos, itens de pedido, atendentes e log legado.
- `Identity`: capacidade registrada no catalogo, ainda imatura no application layer, com persistencia e migrations em infraestrutura propria.

## Dependencias

- `WebApiCoreSeed.Api` referencia `WebApiCoreSeed.Identity.Infrastructure`, `WebApiCoreSeed.SampleRestaurant` e `WebApiCoreSeed.SampleRestaurant.Infrastructure` para composicao.
- `WebApiCoreSeed.Identity.Infrastructure` nao referencia API.
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

## Resultado do prompt 03

- Repositorio generico legado removido do codigo ativo.
- Implementacao generica legada removida da infraestrutura.
- Portas criadas/explicitadas:
  - `IPratoRepository`
  - `IMesaRepository`
  - `IAtendenteRepository`
  - `IPedidoRepository`
  - `IPedidoPratoRepository`
  - `ILogginRepository`
- Queries criadas/explicitadas:
  - `IPratoRepository.ExisteComId`
  - `IPratoRepository.ListarPagina`
  - `IPratoRepository.Contar`
  - `IPratoRepository.ObterPorId`
  - `IMesaRepository.ObterPorId`
- Query generica por predicado arbitrario removida.
- `IPedidoRepository.ObterPedidoItens` removido por falta de consumidor e por nao incluir itens.
- `Repository.ObterPorId` que engolia excecoes e retornava `null` foi removido.
- `Console.WriteLine` de persistencia removido.
- `PratoService.Adicionar` deixou de bloquear em `.Result` e passou a consultar existencia de forma assincrona.

## Resultado do prompt 04

- Contrato criado: `ISampleRestaurantUnitOfWork`.
- Implementacao criada: `SampleRestaurantUnitOfWork`, delegando para `SampleRestaurantDbContext.SaveChangesAsync`.
- DI registra `ISampleRestaurantUnitOfWork` como scoped.
- Repositorios migrados:
  - `AtendenteRepository`
  - `MesaRepository`
  - `PedidoRepository`
  - `PedidoPratoRepository`
  - `PratoRepository`
  - `LogginRepository`
- Casos de uso migrados:
  - `AtendenteService.Adicionar`, `Atualizar`, `Remover`
  - `MesaService.Adicionar`, `Atualizar`, `Remover`
  - `PedidoService.Adicionar`, `Atualizar`, `Remover`
  - `PedidoPratoService.Adicionar`, `Atualizar`, `Remover`
  - `PratoService.Adicionar`, `Atualizar`, `Remover`
  - `LogginService.Adicionar`
- Commits removidos de repositories: 16 chamadas diretas a `SampleRestaurantDbContext.SaveChangesAsync`.
- Transacoes explicitas adicionadas: nenhuma.
- Justificativa: um unico `SaveChangesAsync` por caso de uso e atomico no `SampleRestaurantDbContext`.
- Multiplos DbContexts: `SampleRestaurantDbContext` e `ApplicationDbContext` continuam separados; a Unit of Work criada cobre somente o sample.
- Domain events/interceptors: nao existem no codigo ativo e nao foram introduzidos.

## Resultado do prompt 05

- Convencao: `CancellationToken cancellationToken` como ultimo parametro.
- Controllers atualizados:
  - `PratosController`: `ObterLista`, `ObterPorId`, `Adicionar`, `Atualizar`, `Excluir`, helpers `ObterPrato` e `ObterPratos`.
  - `MesasController`: `ObterPorId`, `Adicionar`, `Atualizar`, `Excluir`, helper `ObterMesa`.
- Portas de entrada atualizadas:
  - `IPratoService`, `IMesaService`, `IAtendenteService`, `IPedidoService`, `IPedidoPratoService`, `ILogginService`.
- Casos de uso atualizados:
  - `PratoService`, `MesaService`, `AtendenteService`, `PedidoService`, `PedidoPratoService`, `LogginService`.
- Portas de saida atualizadas:
  - `IPratoRepository`, `IMesaRepository`, `IAtendenteRepository`, `IPedidoRepository`, `IPedidoPratoRepository`, `ILogginRepository`.
- Repositories atualizados:
  - `PratoRepository`, `MesaRepository`, `AtendenteRepository`, `PedidoRepository`, `PedidoPratoRepository`, `LogginRepository`.
- EF Core recebe token em:
  - `FindAsync`, `AnyAsync`, `ToListAsync`, `CountAsync`, `SaveChangesAsync`.
- Unit of Work recebe e propaga token:
  - `ISampleRestaurantUnitOfWork.CommitAsync`.
  - `SampleRestaurantUnitOfWork.CommitAsync`.
- Redis recebe token em:
  - `ResponseCacheService.GetCachedResponseAsync`.
  - `ResponseCacheService.CacheResponseAsync`.
- HTTP/tooling:
  - `OpenApiGenerator` usa `CancellationTokenSource` para Ctrl+C e propaga token para `HttpClient.GetAsync` e `CopyToAsync`.
- Cancelamento:
  - Services checam token ja cancelado antes de validar/alterar dependencias.
  - Repositories de tracking checam token ja cancelado antes de alterar `ChangeTracker`.
  - `SerilogMiddleware` nao registra `OperationCanceledException` como erro 500.
  - `UnhandledExceptionHandler` nao transforma `OperationCanceledException` em Problem Details 500.
- APIs sem suporte a token:
  - Metodos usados de `UserManager` e `SignInManager` em Auth nao expõem `CancellationToken`.
  - Upload de arquivo permanece sincrono e fora de escopo.

## Resultado do prompt 06

- Novo projeto: `src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`.
- `ApplicationDbContext` movido para `WebApiCoreSeed.Identity.Infrastructure.Context`.
- Migrations de Identity movidas para `src/Identity.Infrastructure/Migrations`:
  - `20200817223121_InitialCreate.cs`
  - `20200817223121_InitialCreate.Designer.cs`
  - `ApplicationDbContextModelSnapshot.cs`
- Migrations do sample permanecem em `src/SampleRestaurant.Infrastructure/Migrations`:
  - `20200817223231_InitialCreate.cs`
  - `20200817223231_InitialCreate.Designer.cs`
  - `SampleRestaurantDbContextModelSnapshot.cs`
- Assemblies de migrations:
  - `ApplicationDbContext`: `WebApiCoreSeed.Identity.Infrastructure`.
  - `SampleRestaurantDbContext`: `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- Startup project para comandos EF: `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`.
- Factories design-time:
  - `ApplicationDbContextFactory`.
  - `SampleRestaurantDbContextFactory`.
- `ApplicationDbContext` preserva max length 128 em `IdentityUserLogin<string>` e `IdentityUserToken<string>` para evitar alteracao de schema gerada pelos defaults do Identity 10.
- Seed:
  - Nao existe seed runtime, `HasData`, initializer ou comando de seed.
  - Testes criam dados por caso e limpam estado com `DatabaseReset`.
- API:
  - Nao contem arquivos de migration.

## Debitos temporarios

- Paginacao ainda e a implementacao legada; Prompt 7.
- Repositories ainda implementam `IDisposable` e services ainda descartam repositories; isso e legado preservado e pode ser simplificado em refatoracao futura.

## Validacao final

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou com warnings de analyzer preexistentes na API.
- `dotnet test --configuration Release --no-build`: passou, 53 testes em `WebApiCoreSeed.Tests` e 32 em `WebApiCoreSeed.IntegrationTests`.
- `dotnet ef --version`: `10.0.10`.
- `dotnet ef dbcontext list`: validado para `ApplicationDbContext` e `SampleRestaurantDbContext`.
- `dotnet ef migrations list --no-connect`: validado para `ApplicationDbContext` e `SampleRestaurantDbContext`.
- `dotnet ef migrations has-pending-model-changes`: sem alteracoes pendentes para ambos os contextos.
- Scripts idempotentes de migrations foram gerados com sucesso em `%TEMP%`.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~MigrationsQuandoBancoVazioDeveCriarSchema"`: passou; SQL Server Testcontainers aplicou migrations em banco vazio.
- Busca de arquivos de migration dentro da API: sem arquivos.
- Banco local do usuario: nao alterado.

## Proxima issue

- Proxima issue registrada conforme Prompt 6: `#19`.
- Proximo prompt: `07 - Paginacao deterministica`.

## Proximos prompts restantes

- `03 - Portas de persistencia`: concluido.
- `04 - Unit of Work`: concluido.
- `05 - CancellationToken`: concluido.
- `06 - Migrations na infraestrutura`: concluido.
- `07 - Paginacao deterministica`: pendente.

## Observacoes

- A solucao ativa permanece `WebApiCoreSeed.sln`.
- Os projetos ativos miram `net10.0`.
- As skills DDD citadas no prompt nao estavam instaladas nesta sessao; foram usadas as skills locais aplicaveis de SDD, mudanca .NET, integracao .NET e refatoracao .NET.
