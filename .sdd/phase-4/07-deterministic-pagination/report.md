# Report - Prompt 07

## Resumo

A paginacao ativa de `GET /api/v{version}/Pratos` agora e deterministica, limitada e validada. O endpoint continua usando offset pagination, com ordenacao estavel por `Titulo` e `Id`, page size default `10`, maximo `50` e erro previsivel para valores invalidos.

## Endpoint paginado

- `GET /api/v{version}/Pratos`.

## Contrato

Entrada:

- `PageNumber`: default `1`, minimo `1`.
- `PageSize`: default `10`, minimo `1`, maximo `50`.

Saida:

- `items`
- `page`
- `pageSize`
- `totalItems`
- `totalPages`
- `hasNextPage`
- `hasPreviousPage`

## Implementacao

- `PaginationParameter` usa `Range` e nao trunca valores invalidos.
- `PratosController.ObterLista` retorna Validation Problem Details quando a query string invalida o `ModelState`.
- `PaginationResult<T>` expoe metadata consistente.
- `PratoRepository.ListarPagina` usa `AsNoTracking`, `OrderBy(Titulo)`, `ThenBy(Id)`, `Skip`, `Take`, projecao para `PratoListItem` e `ToListAsync(cancellationToken)`.
- `CountAsync(cancellationToken)` permanece porque o contrato inclui totais.
- Migration `AddPratosPaginationOrderingIndex` adiciona `IX_Pratos_Titulo_Id`.

## OpenAPI

- Query params `PageNumber` e `PageSize` agora documentam `minimum` e `maximum`.
- Schema paginado de pratos troca `data/pageNumber/totalItens` por `items/page/pageSize/totalItems/totalPages/hasNextPage/hasPreviousPage`.

## Breaking changes

- Response de `GET /api/v{version}/Pratos` mudou de formato.
- `PageSize > 50` passou de truncamento silencioso para `400`.
- `PageNumber <= 0` e `PageSize <= 0` retornam `400`.

## Validacao

- Build Release passou.
- Testes leves/unitarios passaram.
- Testes de integracao com SQL Server e Redis reais passaram.
- OpenAPI regenerado.
- Migrations sem pending model changes.

## Delivery

- Commit semantico planejado: `refactor: make pagination deterministic and bounded`.
- Push: nao realizado.
- Fase 4: concluida localmente apos validacao consolidada e commit.
