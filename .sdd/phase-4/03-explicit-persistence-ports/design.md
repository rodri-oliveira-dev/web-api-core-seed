# Design - Prompt 03

## Decisao principal

Substituir o repositorio generico por portas especificas no namespace existente `WebApiCoreSeed.SampleRestaurant.Interfaces.Repository`, preservando o limite Application -> Infrastructure ja estabelecido nos prompts anteriores.

## Portas

- `IPratoRepository`: porta de persistencia de `Prato`, com comandos de escrita, consulta por id e consulta paginada usada pelo controller.
- `IMesaRepository`: porta de persistencia de `Mesa`, com comandos de escrita e consulta por id.
- `IAtendenteRepository`: porta de persistencia de `Atendente`, com comandos de escrita usados pelo service.
- `IPedidoRepository`: porta de persistencia de `Pedido`, com comandos de escrita usados pelo service.
- `IPedidoPratoRepository`: porta de persistencia de `PedidoPrato`, com comandos de escrita usados pelo service.
- `ILogginRepository`: porta para persistir `LogginEntity`.

## Implementacao

- Cada repository concreto usa `SampleRestaurantDbContext` diretamente.
- `Adicionar`, `Atualizar` e `Remover` continuam chamando `SaveChangesAsync` uma vez por metodo, mantendo o comportamento legado ate o Prompt 4.
- Nao ha adaptador temporario para o repositorio generico legado.
- Excecoes de EF Core propagam naturalmente para os handlers da API.
- Persistencia nao usa `Console.WriteLine`.

## Consultas

- Consultas de `Prato` permanecem como entidades para preservar o mapper e contrato HTTP.
- A consulta paginada usa `AsNoTracking`, `Skip` e `Take`, preservando comportamento legado.
- Ordenacao deterministica de pagina fica registrada para o Prompt 7.
- Nenhum metodo expoe `IQueryable` ou recebe `Expression<Func<...>>`.

## Aggregates

- Como a modelagem ainda e legado em camadas, as portas acompanham as entidades atualmente persistidas pelos services.
- Nao foram criados repositorios para value objects.
- A definicao final de aggregate roots fica para refinamento DDD posterior quando invariantes/transacoes forem redesenhadas.
