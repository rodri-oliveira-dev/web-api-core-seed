# Discovery - Prompt 07

## Comandos executados

```text
git grep -n "\.Skip("
git grep -n "\.Take("
git grep -n "PageSize"
git grep -n "PageNumber"
git grep -n "Paged"
git grep -n "Pagination"
git grep -n "OrderBy"
git grep -n "CountAsync"
git grep -n "ToListAsync"
```

## Achados

- `Skip` e `Take` aparecem apenas em `PratoRepository.ListarPagina`.
- A query de `Pratos` usava `AsNoTracking`, `Skip`, `Take` e `ToListAsync`, mas nao possuia `OrderBy`.
- `PageSize` possuia default `10` e maximo `50`, mas valores acima do maximo eram truncados silenciosamente.
- `PageNumber` nao tinha validacao contra zero ou valores negativos.
- `PageSize` nao tinha validacao contra zero ou valores negativos.
- `PaginationResult<T>` expunha `PageNumber`, `TotalPages`, `TotalItens` e `Data`.
- O contrato nao expunha `pageSize`, `hasNextPage` ou `hasPreviousPage`.
- `CountAsync` e necessario no endpoint atual porque o contrato retorna totais.
- Nao foram encontrados endpoints ativos que retornam listas sem limite alem da listagem paginada de pratos.

## Riscos identificados

- Ordenar somente por `Titulo` nao e unico; pratos com o mesmo titulo podem trocar de posicao entre consultas.
- Truncar page size acima do maximo oculta erro de cliente e torna o contrato menos previsivel.
- Renomear metadata de resposta e uma mudanca de formato para consumidores existentes.
- A adicao de indice requer migration nova no projeto de infraestrutura do sample.
