# Transaction Inventory - Prompt 04

| Local | DbContext | Metodo | Chamada de commit | Transacao explicita | Caso de uso | Problema | Acao |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `AtendenteRepository` | `SampleRestaurantDbContext` | `Adicionar` | `_context.SaveChangesAsync()` | Nao | `AtendenteService.Adicionar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `AtendenteRepository` | `SampleRestaurantDbContext` | `Atualizar` | `_context.SaveChangesAsync()` | Nao | `AtendenteService.Atualizar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `AtendenteRepository` | `SampleRestaurantDbContext` | `RemoverPorId` | `_context.SaveChangesAsync()` | Nao | `AtendenteService.Remover` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `MesaRepository` | `SampleRestaurantDbContext` | `Adicionar` | `_context.SaveChangesAsync()` | Nao | `MesaService.Adicionar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `MesaRepository` | `SampleRestaurantDbContext` | `Atualizar` | `_context.SaveChangesAsync()` | Nao | `MesaService.Atualizar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `MesaRepository` | `SampleRestaurantDbContext` | `RemoverPorId` | `_context.SaveChangesAsync()` | Nao | `MesaService.Remover` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PedidoRepository` | `SampleRestaurantDbContext` | `Adicionar` | `_context.SaveChangesAsync()` | Nao | `PedidoService.Adicionar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PedidoRepository` | `SampleRestaurantDbContext` | `Atualizar` | `_context.SaveChangesAsync()` | Nao | `PedidoService.Atualizar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PedidoRepository` | `SampleRestaurantDbContext` | `RemoverPorId` | `_context.SaveChangesAsync()` | Nao | `PedidoService.Remover` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PedidoPratoRepository` | `SampleRestaurantDbContext` | `Adicionar` | `_context.SaveChangesAsync()` | Nao | `PedidoPratoService.Adicionar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PedidoPratoRepository` | `SampleRestaurantDbContext` | `Atualizar` | `_context.SaveChangesAsync()` | Nao | `PedidoPratoService.Atualizar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PedidoPratoRepository` | `SampleRestaurantDbContext` | `RemoverPorId` | `_context.SaveChangesAsync()` | Nao | `PedidoPratoService.Remover` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PratoRepository` | `SampleRestaurantDbContext` | `Adicionar` | `_context.SaveChangesAsync()` | Nao | `PratoService.Adicionar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work apos validacao/existencia |
| `PratoRepository` | `SampleRestaurantDbContext` | `Atualizar` | `_context.SaveChangesAsync()` | Nao | `PratoService.Atualizar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `PratoRepository` | `SampleRestaurantDbContext` | `RemoverPorId` | `_context.SaveChangesAsync()` | Nao | `PratoService.Remover` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `LogginRepository` | `SampleRestaurantDbContext` | `Registrar` | `_context.SaveChangesAsync()` | Nao | `LogginService.Adicionar` | Commit dentro do repositorio | Remover commit; service chama Unit of Work |
| `SampleRestaurantDbContext` | `SampleRestaurantDbContext` | `SaveChangesAsync` | `base.SaveChangesAsync()` | Nao | Unit of Work | Local autorizado para aplicar auditoria simples | Manter; chamado pela implementacao de Unit of Work |
| `ApplicationDbContext` | `ApplicationDbContext` | Identity EF stores | Interno ao Identity | Nao encontrado no codigo ativo | Autenticacao/Identity | Contexto separado, sem caso de uso de sample compartilhado | Manter fora da Unit of Work do sample |

