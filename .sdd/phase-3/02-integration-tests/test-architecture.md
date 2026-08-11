# Test Architecture - 02 Integration Tests

## Estrutura

- `ApiFactory`: deriva de `WebApplicationFactory<Program>` e implementa `IAsyncLifetime`.
- `DatabaseReset`: limpeza deterministica das tabelas SQL entre testes.
- `AuthenticationHelper`: geracao de JWT para cenarios `401` e `403`.
- `JsonAssertions`: leitura de JSON e Problem Details.
- `TestData`: entidades deterministicas para seeds pequenos.
- `ApiIntegrationFixtureDefinition`: collection fixture compartilhada.

## Ciclo de inicializacao

1. Inicia SQL Server e Redis por Testcontainers.
2. Define overrides de configuracao do ambiente `Testing`.
3. Aguarda o SQL Server aceitar conexoes TCP reais.
4. Inicializa o host da API pelo `WebApplicationFactory`.
5. Aplica migrations de Identity e dominio com `Database.MigrateAsync()`.
6. Reseta SQL e Redis antes da primeira execucao.

## Categorias

Todos os testes novos usam:

```text
Category=Integration
Category=Container
```

## Responsabilidades

- Testes HTTP validam pipeline, serializacao, auth, Problem Details, rate limiting, health e OpenAPI.
- Testes SQL validam migrations, constraints, indice, transacao e funcao nativa do SQL Server.
- Testes Redis validam escrita/leitura, chave ausente e expiracao.
