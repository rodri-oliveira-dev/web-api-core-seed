# Endpoint Inventory - Prompt 07

| Endpoint | Query | Paginacao atual | Ordenacao atual | Padrao | Maximo | Metadata | Estrategia proposta | Risco |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `GET /api/v{version}/Pratos` | `IPratoRepository.ListarPagina(PaginationParameter)` | Offset com `Skip((PageNumber - 1) * PageSize)` e `Take(PageSize)` | Nenhuma | `PageNumber=1`, `PageSize=10` | `PageSize=50` por truncamento silencioso | `PageNumber`, `TotalPages`, `TotalItens`, `Data` | Offset validado, limitado, `OrderBy(Titulo).ThenBy(Id)`, metadata `items/page/pageSize/totalItems/totalPages/hasNextPage/hasPreviousPage` | Mudanca de formato do response e rejeicao de `PageSize > 50` |

## Decisao Offset versus Cursor

`GET /api/v{version}/Pratos` permanece com offset pagination.

Justificativa:

- O endpoint representa um catalogo de exemplo, de volume moderado.
- Navegacao por paginas e mais simples para consumidores atuais.
- Nao ha requisito de consistencia forte entre paginas sob alta escrita concorrente.
- A ordenacao estavel por `Titulo` e `Id` reduz instabilidade sem introduzir cursor.

Cursor pagination nao foi implementada porque adicionaria complexidade contratual sem necessidade demonstrada no estado atual.
