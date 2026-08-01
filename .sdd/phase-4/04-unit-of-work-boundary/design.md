# Design - Prompt 04

## Decisao

Criar uma porta de saida especifica do modulo:

```csharp
public interface ISampleRestaurantUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
```

O nome explicita o limite real: uma Unit of Work do `SampleRestaurantDbContext`, nao uma coordenacao generica entre todos os bancos da aplicacao.

## Propriedade

- Contrato: Application do modulo `SampleRestaurant`.
- Implementacao: Infrastructure do modulo `SampleRestaurant`.
- Composition root: API registra contrato e implementacao no DI.

## Implementacao

- `SampleRestaurantUnitOfWork` recebe `SampleRestaurantDbContext`.
- `CommitAsync` chama `SampleRestaurantDbContext.SaveChangesAsync`.
- Repositorios concretos passam a chamar apenas `Add`, `Update` e `Remove` no `DbSet`.
- Metodos de escrita dos repositories retornam `Task`, mantendo assinatura assincrona sem expor linhas afetadas.

## Escopo de DI

- `SampleRestaurantDbContext`: scoped.
- Repositorios: scoped.
- `ISampleRestaurantUnitOfWork`: scoped.
- Services/casos de uso: scoped.

Com o mesmo escopo, varios repositorios do modulo compartilham o mesmo `SampleRestaurantDbContext` e podem ser confirmados por um unico commit no caso de uso.

## Falhas

- Falha antes de `CommitAsync`: nenhuma chamada a `SaveChangesAsync`; alteracoes rastreadas sao descartadas ao fim do escopo.
- Falha durante `CommitAsync`: excecao do EF Core e propagada; o pipeline HTTP existente continua responsavel por mapear erro inesperado/persistencia.
- Rollback explicito nao e necessario para um unico `SaveChangesAsync`; o EF Core executa o save de forma atomica no provider relacional.

## Transacoes explicitas

Nao serao adicionadas transacoes explicitas nesta entrega. Um unico `SaveChangesAsync` por caso de uso e suficiente para atomicidade local no `SampleRestaurantDbContext`.

## Domain events

Nao ha domain events ou interceptors ativos. A Unit of Work nao publicara eventos. Caso eventos sejam introduzidos no futuro, eles devem ser integrados ao commit de forma explicita e provavelmente exigir outbox, que esta fora do escopo.

## Identity

`ApplicationDbContext` permanece separado e pertence ao limite do ASP.NET Core Identity. A Unit of Work do sample nao coordenara Identity. Nao ha caso de uso atual que precise confirmar alteracoes de restaurante e Identity na mesma transacao.

## Multiplos DbContexts

Existem dois DbContexts independentes. A estrategia atual e uma Unit of Work por limite real de persistencia, nao uma Unit of Work generica que prometa transacao distribuida. Caso um fluxo futuro precise de dois DbContexts, ele devera ser desenhado explicitamente como consistencia eventual ou coordenacao especifica, sem distributed transaction implicita.

