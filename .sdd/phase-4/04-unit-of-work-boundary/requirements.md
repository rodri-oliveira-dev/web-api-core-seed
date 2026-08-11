# Requirements - Prompt 04

## Objetivo

Introduzir um limite explicito de Unit of Work para o modulo `SampleRestaurant`.

Repositorios devem registrar alteracoes no `SampleRestaurantDbContext`; casos de uso de escrita devem decidir quando confirmar essas alteracoes.

## Requisitos funcionais

- `Adicionar`, `Atualizar` e `Remover` em repositorios nao executam `SaveChanges`.
- Cada caso de uso de escrita confirma uma vez quando a validacao e as operacoes de persistencia terminam com sucesso.
- Casos de uso de consulta nao confirmam alteracoes.
- Falhas antes do commit nao persistem alteracoes.
- Falhas durante o commit propagam excecao ao chamador.
- Operacoes multi-entidade no mesmo `SampleRestaurantDbContext` sao atomicas por um unico `SaveChangesAsync`.
- Controllers permanecem como adaptadores HTTP e nao executam commits.

## Requisitos tecnicos

- Application declara o contrato de Unit of Work como porta de saida.
- Infrastructure implementa o contrato usando `SampleRestaurantDbContext`.
- Domain e Application nao dependem de EF Core nem de `DbContext`.
- DI registra a Unit of Work com escopo de request.
- Nao criar transacoes explicitas quando um unico `SaveChangesAsync` ja fornece atomicidade.
- Nao alterar migrations nem unificar `SampleRestaurantDbContext` e `ApplicationDbContext`.

## Fora de escopo

- Outbox.
- Mensageria.
- Distributed transactions.
- Coordenacao entre multiplos bancos.
- Refatoracao ampla de `CancellationToken`.
- Migracao/movimentacao de migrations.
- Redesenho completo dos aggregates.

