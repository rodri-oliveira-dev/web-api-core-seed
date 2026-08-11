# Requirements - Prompt 07

## Objetivo

Tornar a paginacao ativa deterministica, limitada, validada, documentada, eficiente e consistente.

## Escopo

- Endpoint paginado ativo: `GET /api/v{version}/Pratos`.
- Query interna: `IPratoRepository.ListarPagina`.
- Contrato de entrada: `PaginationParameter`.
- Contrato de saida: `PaginationResult<T>`.
- OpenAPI versionado em `docs/openapi/`.
- Testes unitarios/leves e testes de integracao com SQL Server real.

## Requisitos funcionais

- Pagina inicial: `1`.
- Page size padrao: `10`.
- Page size minimo: `1`.
- Page size maximo: `50`.
- Valores invalidos retornam Validation Problem Details `400`.
- Pagina apos o final retorna lista vazia com metadata consistente.
- Colecao vazia retorna lista vazia e totais zerados.
- Metadata deve expor pagina, tamanho, totais e navegacao.

## Requisitos tecnicos

- `Skip` e `Take` somente apos ordenacao estavel.
- Ordenacao de `Pratos`: `Titulo` ascendente e `Id` ascendente como desempate.
- `AsNoTracking` em leitura.
- Filtros devem preceder paginacao quando existirem; nao ha filtros no endpoint atual.
- `CountAsync` usado porque o contrato inclui totais.
- `CancellationToken` propagado.
- Indice relacionado deve apoiar a ordenacao principal.

## Fora de escopo

- Cursor pagination.
- Novos filtros ou mudancas de regra de negocio.
- Cache de paginacao.
- Aspire, template `dotnet new`, Sonar ou empacotamento.
