# Report - 02 Integration Tests

## Resumo

Foi criada uma baseline de testes de integracao com `WebApplicationFactory<Program>`, Testcontainers, SQL Server real e Redis real.

## Entrega tecnica

- Projeto criado: `test/WebApiCoreSeed.IntegrationTests`.
- Fixtures/helpers criados: `ApiFactory`, `DatabaseReset`, `AuthenticationHelper`, `JsonAssertions`, `TestData`.
- Containers:
  - SQL Server `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`.
  - Redis `redis:7.4.2-alpine`.
- Migrations aplicadas automaticamente para `ApplicationDbContext` e `MeuDbContext`.
- Isolamento por collection xUnit, com reset SQL/Redis antes de cada teste.
- Traits adotadas: `Category=Integration` e `Category=Container`.

## Cenarios implementados

- API: sucesso HTTP, Problem Details `400/401/403/404/429`, health e OpenAPI.
- SQL Server: migrations, persistencia, FK, indice unico, transacao e funcao nativa.
- Redis: escrita/leitura, chave inexistente e expiracao.

## Ajuste produtivo

`AtendenteMapping` passou a ignorar `Email` e `Telefone` para preservar o schema legado versionado em migrations. SQL Server real expôs que `Telefone` era descoberto como entidade sem chave no modelo EF Core 10.

## Limitacoes

- Health/readiness permanecem no endpoint existente `/hc`; nao foi criado endpoint separado.
- A suite exige Docker funcional para executar os testes de integracao.
- Os 13 testes HTTP antigos em `Pedidos.Test` foram mantidos para evitar reescrita fora do escopo.

## Proximo passo

Executar o Prompt 3 da Fase 3 para a issue `#9`, iniciando a frente de seguranca.
