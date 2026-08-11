# Report - Prompt 04

## Resumo

O Prompt 04 introduziu um limite explicito de Unit of Work para o modulo `SampleRestaurant`.

Repositorios concretos agora registram alteracoes no `SampleRestaurantDbContext`; services/casos de uso de escrita confirmam uma vez por fluxo valido usando `ISampleRestaurantUnitOfWork`.

## Contrato

- `ISampleRestaurantUnitOfWork`.
- Metodo: `CommitAsync(CancellationToken cancellationToken = default)`.
- Propriedade: Application do modulo `SampleRestaurant`.
- Racional: explicitar o momento do commit sem expor EF Core ou `DbContext` para Application.

## Implementacao

- `SampleRestaurantUnitOfWork`.
- Propriedade: Infrastructure do modulo `SampleRestaurant`.
- Comportamento: delega para `SampleRestaurantDbContext.SaveChangesAsync`.
- DI: scoped.

## Repositorios migrados

- `AtendenteRepository`.
- `MesaRepository`.
- `PedidoRepository`.
- `PedidoPratoRepository`.
- `PratoRepository`.
- `LogginRepository`.

Total removido: 16 chamadas diretas a `SaveChangesAsync` dentro de repositories.

## Casos de uso migrados

- `AtendenteService`: adicionar, atualizar, remover.
- `MesaService`: adicionar, atualizar, remover.
- `PedidoService`: adicionar, atualizar, remover.
- `PedidoPratoService`: adicionar, atualizar, remover.
- `PratoService`: adicionar, atualizar, remover.
- `LogginService`: adicionar.

## Transacoes

Nenhuma transacao explicita foi adicionada.

O desenho usa um unico `SaveChangesAsync` por caso de uso porque isso ja fornece atomicidade local para alteracoes rastreadas no mesmo `SampleRestaurantDbContext`.

## Multiplos DbContexts

- `SampleRestaurantDbContext`: coberto por `ISampleRestaurantUnitOfWork`.
- `ApplicationDbContext`: permanece separado no limite de Identity.

Nao ha fluxo atual que exija coordenacao transacional entre os dois contextos.

## Domain events

Nao ha domain events, interceptors ou outbox no codigo ativo. Nada foi introduzido neste prompt.

## Testes

- Unitarios/leves:
  - commit unico no sucesso;
  - ausencia de commit quando validacao falha;
  - propagacao de excecao de commit.
- Integracao SQL Server real:
  - criacao com Unit of Work;
  - atualizacao com Unit of Work;
  - falha antes do commit nao persiste;
  - falha durante commit propaga excecao;
  - duas alteracoes no mesmo commit nao persistem parcialmente quando uma delas viola constraint.
- Smoke HTTP:
  - `POST /api/v1/Mesas` persiste usando a Unit of Work.

## Validacao

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou.
- `dotnet test --configuration Release --no-build`: passou, 49 + 31 testes.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration`: passou, 31 testes.
- Grep final de `SaveChanges` em `src`: somente override autorizado do `SampleRestaurantDbContext`.
- OpenAPI regenerado sem diff de contrato.

## Delivery

- Commit semantico planejado: `refactor: define explicit unit of work boundary`.
- Push: nao realizado.
- Proximo prompt/issue: `#16`, Prompt 5 - CancellationToken.
