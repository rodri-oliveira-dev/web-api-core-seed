# Validation - Legacy Upgrade Validation

## Ambiente

- Branch: `test/legacy-schema-upgrade`.
- SQL Server Testcontainers: `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`.
- `dotnet ef --version`: `10.0.10`.
- Observacao: os comandos EF exibiram aviso de ferramenta `10.0.10` menor que runtime `10.0.11`, sem falha.

## Baseline Legado

- Origem: migrations do commit `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d`.
- Blobs de origem:
  - `be224608397a0f3a4fd8613ad166df2f4d6aec21` para `src/DevIO.Api/Migrations/20200817223121_InitialCreate.cs`.
  - `580219d970d4b793da44c9a93f0ea6efa45a831c` para `src/DevIO.Data/Migrations/20200817223231_InitialCreate.cs`.
- Script versionado: `tests/WebApiCoreSeed.IntegrationTests/Infrastructure/LegacyUpgrade/legacy-schema-baseline.sql`.
- SHA-256 do script: `DB3116099B513AB76C4BEFB37AED1138B6A9493E8A3DAC564C767895BC0B5601`.

## Comandos Executados

| Comando | Resultado |
| --- | --- |
| `dotnet restore WebApiCoreSeed.slnx` | Passou. |
| `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore` | Passou com `0` warnings e `0` erros apos ajuste. |
| `dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build` | Passou: `93` testes. |
| `dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build` | Passou: `46` testes. |
| `dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~LegacySchemaUpgradeIntegrationTests"` | Passou: `1` teste; primeira execucao. |
| Mesmo teste isolado pela segunda vez | Passou: `1` teste; segunda execucao. |
| `dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~MigrationsQuandoBancoVazioDeveCriarSchema"` | Passou: `1` teste. |
| `dotnet ef migrations list` para `ApplicationDbContext` com `--no-connect` e connection string dummy por ambiente | Passou; listou `20200817223121_InitialCreate`. |
| `dotnet ef migrations list` para `SampleRestaurantDbContext` com `--no-connect` e connection string dummy por ambiente | Passou; listou `20200817223231_InitialCreate` e `20260801191447_AddPratosPaginationOrderingIndex`. |
| `dotnet ef migrations has-pending-model-changes` para `ApplicationDbContext` com connection string dummy por ambiente | Passou; sem pending model changes. |
| `dotnet ef migrations has-pending-model-changes` para `SampleRestaurantDbContext` com connection string dummy por ambiente | Passou; sem pending model changes. |
| `dotnet ef migrations script --idempotent` para `ApplicationDbContext` | Passou; script gerado em `%TEMP%\web-api-core-seed-identity-idempotent.sql`. |
| `dotnet ef migrations script --idempotent` para `SampleRestaurantDbContext` | Passou; script gerado em `%TEMP%\web-api-core-seed-sample-idempotent.sql`. |
| `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build` | Passou; gerou `docs/openapi/openapi-v1.json` e `docs/openapi/openapi-v2.json`. |
| `git diff -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json` | Sem diff. |
| `git diff --check` | Passou com exit `0`; exibiu apenas aviso CRLF no lockfile preexistente `tools/OpenApiGenerator/packages.lock.json`. |

## Migrations Antes e Depois do Upgrade

Antes do upgrade, o baseline registra:

- `20200817223121_InitialCreate`
- `20200817223231_InitialCreate`

Depois de executar `MigrateAsync` nos contextos atuais:

- `20200817223121_InitialCreate`
- `20200817223231_InitialCreate`
- `20260801191447_AddPratosPaginationOrderingIndex`

O teste valida contagem `1` por migration ID antes/depois e repete `MigrateAsync` uma segunda vez sem criar duplicidades.

## Dados Preservados

Dados inseridos antes do upgrade:

- `AspNetUsers`: `legacy-user-001`, email `legacy.user@example.test`.
- `Atendentes`: `Atendente legado`.
- `Mesas`: `M-LEG-01`.
- `Pratos`: `Caldo legado`.
- `Pedidos`: `PED-LEG-01`.
- `PedidoPrato`: observacao `Sem cebola`.
- `Loggin`: mensagem `Evento legado preservado`.

Dados recuperados depois:

- Usuario consultado pelo `ApplicationDbContext` atual.
- Prato e item de pedido consultados pelo `SampleRestaurantDbContext` atual.
- Novo prato `Prato atual apos upgrade` persistido pelo `SampleRestaurantDbContext` atual.

## Schema Validado

Indices:

- `IX_Pratos_Titulo_Id` ausente antes do upgrade e presente depois.
- Colunas de `IX_Pratos_Titulo_Id`: `Titulo`, `Id`.
- `IX_PedidoPrato_PedidoId`.
- `IX_PedidoPrato_PratoId`.
- `IX_Pedidos_AtendenteId`.
- `IX_Pedidos_MesaId`.
- `UserNameIndex`.

Constraints:

- `FK_Pedidos_Atendentes`.
- `FK_Pedidos_Mesas`.
- `FK_PedidoPrato_Pedidos`.
- `FK_PedidoPrato_Pratos`.

## Tempo Aproximado

- Teste isolado novo: primeira execucao levou aproximadamente `79s` de wall time por start inicial do container; a segunda execucao levou aproximadamente `21s`.
- Duracao reportada pelo runner para o teste em si: entre `2s` e `4s`.

## Limitacoes

- O baseline nao executa EF Core 3.1 CLI; ele e derivado do commit legado por inspecao das migrations versionadas.
- O teste cobre um conjunto representativo minimo, nao todas as combinacoes de dados do dominio.
- Os comandos EF usam connection string dummy via ambiente porque `appsettings.json` ativo nao contem `DefaultConnection`.
