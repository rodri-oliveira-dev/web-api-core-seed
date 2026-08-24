# Data Inventory - Development Seed

## Identity

| Dado | Chave | Origem | Estrategia |
| --- | --- | --- | --- |
| Usuario de desenvolvimento | Email normalizado configuravel | `DevelopmentSeed:User:Email` | Criar se ausente; atualizar email/user name confirmado e preservar dados nao gerenciados. |
| Senha de desenvolvimento | Nao persistida em claro | `DevelopmentSeed:User:Password` | Obrigatoria fora do repositorio; aplicar somente via `UserManager`. |
| Claims | Tipo/valor | Definicao versionada | Garantir existencia sem duplicar. |
| Roles | Nome | Definicao versionada | Nao criar role se nao for necessaria para endpoint atual. |

Claims minimas planejadas:

- `Mesas=ObterPorId` para chamada autenticada representativa.
- `Mesas=Adicionar` para escrita manual local quando necessario.
- `Pratos=Adicionar` para escrita manual local quando necessario.

## SampleRestaurant

| Entidade | Chave de seed | Quantidade planejada | Observacao |
| --- | --- | ---: | --- |
| `Prato` | Guid deterministico | 4 | Catalogo pequeno cobrindo comida, bebida e sobremesa. |
| `Mesa` | Guid deterministico | 3 | Numeros legiveis e localizacoes distintas. |
| `Atendente` | Guid deterministico | 1 | Necessario para um pedido demonstrativo. |
| `Pedido` | Guid deterministico | 1 | Demonstra relacionamento entre mesa, atendente e pratos. |
| `PedidoPrato` | Guid deterministico | 2 | Itens do pedido demonstrativo. |

## Preservacao

- Dados com IDs deterministicas do seed podem ser atualizados para refletir a definicao versionada.
- Dados com IDs ou chaves naturais fora da definicao do seed devem ser preservados.
- O seed nao deve usar `AnyAsync()` sobre a tabela inteira como criterio de existencia.
