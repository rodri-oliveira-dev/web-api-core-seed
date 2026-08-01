# Requirements - Prompt 03

## Objetivo

Remover o repositorio generico legado do codigo ativo e substituir seus consumidores por portas de persistencia explicitas do modulo `SampleRestaurant`.

## Escopo

- Application e Domain nao devem depender do repositorio generico legado.
- Repositorios especificos devem expressar intencao por entidade/aggregate root.
- Consultas somente leitura devem ter metodos explicitos, sem `Expression<Func<...>>` arbitrario nem `IQueryable` fora da Infrastructure.
- O comportamento HTTP existente deve ser preservado.
- O commit implicito via repositorio permanece temporariamente ate o Prompt 4.
- Nao introduzir `SaveChanges` adicional.

## Fora de escopo

- Finalizar Unit of Work.
- Propagar `CancellationToken` em toda a aplicacao.
- Mover migrations.
- Redefinir contrato final de paginacao.

## Criterios de aceite

- Grep literal do nome da interface generica vazio para codigo ativo.
- Grep literal do nome da implementacao generica vazio para codigo ativo.
- `Expression<Func<...>>` removido das portas de Application.
- Excecoes de persistencia nao sao engolidas.
- Persistencia nao escreve no console.
- Testes de unidade, integracao e contrato passam.
- Contratos OpenAPI permanecem sem mudanca esperada.
