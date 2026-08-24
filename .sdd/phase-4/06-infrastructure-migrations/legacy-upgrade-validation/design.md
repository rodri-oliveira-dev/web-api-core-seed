# Design - Legacy Upgrade Validation

## Estrategia

Criar um teste de integracao dedicado que sobe um SQL Server Testcontainer, aplica um script SQL versionado de baseline legado, insere dados representativos, executa as migrations atuais dos dois contextos e valida o resultado.

## Baseline Legado

O baseline sera um arquivo SQL pequeno em `tests/WebApiCoreSeed.IntegrationTests/Infrastructure/LegacyUpgrade/legacy-schema-baseline.sql`.

O script sera derivado manualmente das operacoes `Up` das duas migrations legadas do commit `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d`, nao das migrations atuais. Ele contera:

- `dbo.__EFMigrationsHistory`.
- Tabelas de Identity e SampleRestaurant.
- PKs, FKs, indices e defaults essenciais das migrations.
- Inserts em `dbo.__EFMigrationsHistory` para os dois IDs historicos.

Dados de teste nao ficarao no script; serao inseridos pelo teste para manter o baseline estrutural e reutilizavel.

## Fixture

Criar uma fixture pequena somente para o upgrade:

- Usa `MsSqlBuilder(ApiFactory.SqlServerImage)`.
- Cria connection string descartavel do proprio container.
- Nao inicializa API, Redis ou `WebApplicationFactory`.
- Expoe helpers para abrir `SqlConnection` e criar `ApplicationDbContext` / `SampleRestaurantDbContext` atuais.
- Aplica o SQL baseline dividindo batches por `GO`.
- Usa `CancellationToken` nas operacoes async.

## Fluxo do Teste

1. Inicializar container SQL Server.
2. Aplicar `legacy-schema-baseline.sql`.
3. Confirmar migrations historicas e ausencia de `IX_Pratos_Titulo_Id`.
4. Inserir dados legados representativos via SQL parametrizado.
5. Executar `ApplicationDbContext.Database.MigrateAsync(cancellationToken)`.
6. Executar `SampleRestaurantDbContext.Database.MigrateAsync(cancellationToken)`.
7. Confirmar historico final: os dois IDs historicos continuam uma vez e a migration nova aparece uma vez.
8. Confirmar dados preservados por SQL e pelo `SampleRestaurantDbContext` atual.
9. Inserir um novo `Prato` pelo `SampleRestaurantDbContext` atual e consultar novamente.
10. Confirmar tabelas, indices e constraints essenciais.
11. Executar `MigrateAsync` uma segunda vez para os dois contextos.
12. Confirmar que historico e objetos permanecem inalterados.

## Asserts Principais

- `__EFMigrationsHistory` antes: `20200817223121_InitialCreate`, `20200817223231_InitialCreate`.
- `__EFMigrationsHistory` depois: IDs historicos e `20260801191447_AddPratosPaginationOrderingIndex`.
- Contagem por migration ID sempre igual a `1`.
- `IX_Pratos_Titulo_Id` ausente antes e presente depois.
- Dados legados preservados: usuario, prato, mesa, atendente, pedido e pedido-prato.
- FKs esperadas existem.
- Indices legados e novo indice existem.

## Limitacoes Aceitas

- O teste nao executa binario .NET Core 3.1 nem EF Core 3.1 CLI, porque o ambiente moderno preserva o legado por commit/tag.
- A derivacao e estrutural a partir das migrations legadas, nao de um dump de producao.
- O teste valida um conjunto representativo minimo, nao uma matriz exaustiva de todos os tipos de dados.
