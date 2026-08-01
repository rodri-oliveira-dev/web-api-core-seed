# Report - Prompt 02

## Resumo

O prompt 02 separou nominal e fisicamente o dominio demonstrativo dos componentes reutilizaveis do seed. A composition root e os building blocks tecnicos agora usam `WebApiCoreSeed.Api`, enquanto o exemplo de restaurante fica em `WebApiCoreSeed.SampleRestaurant` e `WebApiCoreSeed.SampleRestaurant.Infrastructure`.

## Componentes reutilizaveis

- `WebApiCoreSeed.Api`: hosting, configuracao da API, Problem Details, OpenAPI, rate limiting, observabilidade, cache, health checks, filtros, middleware, Identity hospedado e composition root.
- `OpenApiGenerator`: ferramenta de geracao dos contratos versionados.
- Workflows, githooks, workspace e docs operacionais apontam para `WebApiCoreSeed.sln`.

## Componentes do sample

- `WebApiCoreSeed.SampleRestaurant`: entidades, enums, validadores, notificacoes, portas temporarias e services do exemplo.
- `WebApiCoreSeed.SampleRestaurant.Infrastructure`: `SampleRestaurantDbContext`, mappings, repositorios e migrations do exemplo.
- Controllers e view models de `Pratos`, `Mesas`, `Pedidos` e `Atendentes` permanecem como contratos do sample dentro da API.

## Nomes removidos de codigo ativo

- `Restaurante`
- `Datasul`
- `MeuDbContext`
- `DevIO`
- `Pedidos.Test`
- `RestauranteAPI`
- `PedidosApi`

## Compatibilidade

- Rotas e payloads HTTP foram preservados.
- Migrations antigas nao foram movidas; apenas namespace, assembly e tipo de DbContext foram ajustados.
- O titulo OpenAPI foi atualizado para `Sample Restaurant API` para declarar explicitamente o dominio demonstrativo.

## Validacao

- Restore, build, testes completos, testes arquiteturais, testes de integracao/container e geracao OpenAPI passaram.
- Smoke e regressao HTTP ficaram cobertos pelas suites existentes e pela geracao OpenAPI.

## Delivery

- Commit semantico planejado: `refactor: separate sample domain from reusable seed`.
- Push: nao realizado.
- Proximo prompt/issue: `#14`, Prompt 3 - Portas de persistencia.
