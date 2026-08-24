# Report - Legacy Upgrade Validation

## Resumo

Foi adicionada uma validacao de upgrade legado para a issue `#18`. O teste sobe SQL Server por Testcontainers, aplica um baseline SQL derivado do commit legado, registra as migrations historicas, insere dados representativos, executa as migrations atuais e comprova preservacao de dados, aplicacao da migration nova e idempotencia.

## Arquivos Principais

- `tests/WebApiCoreSeed.IntegrationTests/Infrastructure/LegacyUpgrade/legacy-schema-baseline.sql`
- `tests/WebApiCoreSeed.IntegrationTests/Infrastructure/LegacyUpgrade/LegacySchemaUpgradeFixture.cs`
- `tests/WebApiCoreSeed.IntegrationTests/Infrastructure/LegacyUpgrade/LegacySchemaUpgradeFixtureDefinition.cs`
- `tests/WebApiCoreSeed.IntegrationTests/Infrastructure/LegacyUpgrade/LegacySchemaUpgradeIntegrationTests.cs`

## Estrategia do Baseline

O baseline SQL foi derivado das migrations legadas do commit `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d`, nao das migrations atuais. Ele cria o schema historico minimo de Identity e SampleRestaurant, cria `dbo.__EFMigrationsHistory` e registra:

- `20200817223121_InitialCreate`
- `20200817223231_InitialCreate`

Hash do baseline versionado:

```text
DB3116099B513AB76C4BEFB37AED1138B6A9493E8A3DAC564C767895BC0B5601
```

## Cenarios Cobertos

- Banco SQL Server descartavel inicializado por Testcontainers.
- Schema legado aplicado por script versionado.
- Historico EF preenchido com migrations historicas.
- Dados legados inseridos antes do upgrade.
- `Database.MigrateAsync` executado nos contextos atuais.
- Migration historica nao reaplicada.
- Migration nova de paginacao aplicada.
- Dados legados preservados e consultados por DbContexts atuais.
- Novo dado persistido pelo modelo atual apos upgrade.
- Indices, tabelas e FKs essenciais validados.
- Segunda execucao de `MigrateAsync` comprova idempotencia.

## Resultado das Migrations

Antes:

- `20200817223121_InitialCreate`
- `20200817223231_InitialCreate`

Depois:

- `20200817223121_InitialCreate`
- `20200817223231_InitialCreate`
- `20260801191447_AddPratosPaginationOrderingIndex`

## Validacao

Restore, build Release, testes unitarios, testes de integracao, teste novo isolado duas vezes, teste de banco vazio, comandos EF, scripts idempotentes, OpenAPI e `git diff --check` passaram.

## Limitacoes

- O teste nao usa runtime/CLI .NET Core 3.1.
- O baseline e estrutural e representativo; nao e dump completo de uma base real.
- A issue deve permanecer aberta ate o PR ser aprovado no CI.
