# Use Case Query Map - Prompt 03

| Caso de uso | Dados necessarios | Filtro | Ordenacao | Projecao | Porta | Implementacao | Paginacao |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Obter prato por id | Entidade `Prato` completa usada pelo mapper HTTP | `Id == id` via chave primaria | N/A | Entidade | `IPratoRepository.ObterPorId(Guid id)` | `PratoRepository.ObterPorId` com `FindAsync` | N/A |
| Listar pagina de pratos | Entidades `Prato` da pagina solicitada | N/A | Ainda nao definida; Prompt 7 | Entidade | `IPratoRepository.ListarPagina(PaginationParameter paginationParameter)` | `PratoRepository.ListarPagina` com `AsNoTracking`, `Skip`, `Take` | Sim, contrato legado preservado |
| Contar pratos | Total de registros | N/A | N/A | `int` | `IPratoRepository.Contar()` | `PratoRepository.Contar` com `CountAsync` | Usado para metadado HTTP |
| Obter mesa por id | Entidade `Mesa` completa usada pelo mapper HTTP | `Id == id` via chave primaria | N/A | Entidade | `IMesaRepository.ObterPorId(Guid id)` | `MesaRepository.ObterPorId` com `FindAsync` | N/A |

## Consultas removidas

- Consulta generica `Buscar(Expression<Func<TEntity, bool>> predicate)`: sem uso produtivo; removida para nao expor predicado arbitrario.
- Consulta generica `ObterTodos()`: sem uso produtivo; removida.
- `IPedidoRepository.ObterPedidoItens(Guid id)`: sem uso produtivo e nao implementava include de itens; removida.
