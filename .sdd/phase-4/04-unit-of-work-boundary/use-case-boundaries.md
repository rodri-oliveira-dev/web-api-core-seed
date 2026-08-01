# Use Case Boundaries - Prompt 04

## `PratoService.Adicionar`

- Operacoes: validar prato, consultar existencia por id, registrar `Prato`.
- Aggregates: `Prato`.
- Repositorios: `IPratoRepository`.
- Commit esperado: uma chamada a `ISampleRestaurantUnitOfWork.CommitAsync` apos `Adicionar`.
- Atomicidade: consulta nao altera estado; inclusao e atomica no commit unico.
- Falhas: validacao ou id duplicado retornam `false` sem commit; falha no commit propaga excecao.
- Compensacao: nenhuma.

## `PratoService.Atualizar`

- Operacoes: validar prato, registrar atualizacao.
- Aggregates: `Prato`.
- Repositorios: `IPratoRepository`.
- Commit esperado: uma chamada apos `Atualizar`.
- Atomicidade: alteracao confirmada pelo commit unico.
- Falhas: validacao retorna `false` sem commit; falha no commit propaga excecao.
- Compensacao: nenhuma.

## `PratoService.Remover`

- Operacoes: registrar remocao por id.
- Aggregates: `Prato`.
- Repositorios: `IPratoRepository`.
- Commit esperado: uma chamada apos `RemoverPorId`.
- Atomicidade: remocao confirmada pelo commit unico.
- Falhas: falha no commit propaga excecao.
- Compensacao: nenhuma.

## `MesaService.Adicionar`, `Atualizar`, `Remover`

- Operacoes: validar quando ha entidade, registrar alteracao/remocao.
- Aggregates: `Mesa`.
- Repositorios: `IMesaRepository`.
- Commit esperado: uma chamada por metodo de escrita valido.
- Atomicidade: cada alteracao e confirmada por commit unico.
- Falhas: validacao retorna `false` sem commit; falha no commit propaga excecao.
- Compensacao: nenhuma.

## `AtendenteService.Adicionar`, `Atualizar`, `Remover`

- Operacoes: validar quando ha entidade, registrar alteracao/remocao.
- Aggregates: `Atendente`.
- Repositorios: `IAtendenteRepository`.
- Commit esperado: uma chamada por metodo de escrita valido.
- Atomicidade: cada alteracao e confirmada por commit unico.
- Falhas: validacao retorna `false` sem commit; falha no commit propaga excecao.
- Compensacao: nenhuma.

## `PedidoService.Adicionar`, `Atualizar`, `Remover`

- Operacoes: validar quando ha entidade, registrar alteracao/remocao.
- Aggregates: `Pedido`.
- Repositorios: `IPedidoRepository`.
- Commit esperado: uma chamada por metodo de escrita valido.
- Atomicidade: alteracoes futuras envolvendo pedido e itens devem compartilhar a mesma Unit of Work quando estiverem no mesmo `SampleRestaurantDbContext`.
- Falhas: validacao retorna `false` sem commit; constraint ou falha no commit propaga excecao.
- Compensacao: nenhuma.

## `PedidoPratoService.Adicionar`, `Atualizar`, `Remover`

- Operacoes: validar quando ha entidade, registrar alteracao/remocao.
- Aggregates: `PedidoPrato`.
- Repositorios: `IPedidoPratoRepository`.
- Commit esperado: uma chamada por metodo de escrita valido.
- Atomicidade: alteracao confirmada pelo commit unico.
- Falhas: validacao retorna `false` sem commit; falha no commit propaga excecao.
- Compensacao: nenhuma.

## `LogginService.Adicionar`

- Operacoes: validar log, registrar `LogginEntity`.
- Aggregates: `LogginEntity` legado.
- Repositorios: `ILogginRepository`.
- Commit esperado: uma chamada apos `Registrar`.
- Atomicidade: registro confirmado pelo commit unico.
- Falhas: validacao retorna `false` sem commit; falha no commit propaga excecao.
- Compensacao: nenhuma.

