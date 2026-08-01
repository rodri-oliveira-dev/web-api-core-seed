# Performance Assessment - Prompt 07

## Query de pratos

- Usa `AsNoTracking`.
- Aplica ordenacao antes de `Skip` e `Take`.
- Usa projecao para o read model usado pela API, evitando materializar entidade de dominio completa para listagem.
- Usa `CountAsync` porque o contrato retorna `totalItems` e `totalPages`.
- Propaga `CancellationToken` em `ToListAsync` e `CountAsync`.
- Usa indice `IX_Pratos_Titulo_Id` para apoiar a ordenacao.

## Custos aceitos

- `CountAsync` executa uma consulta adicional por request paginada. O custo e aceito porque a metadata total e parte do contrato.
- Offset pagination pode ficar menos eficiente em paginas muito altas. Nao ha requisito atual de cursor pagination.

## Nao implementado

- Cache adicional de paginas.
- Cursor pagination.
- Filtros novos.
