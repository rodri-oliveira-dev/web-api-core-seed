# Discovery - Legacy Upgrade Validation

## Referencia Legada

| Item | Resultado |
| --- | --- |
| Commit informado | `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d` |
| Tipo do objeto | `commit` |
| Tag esperada | `v1.0.0-legacy` existe localmente e aponta para `2799562943ac03926d69bc716617d091d04ecc82` |
| Branch esperada | `legacy/netcoreapp3.1` existe localmente e aponta para `2799562943ac03926d69bc716617d091d04ecc82` |
| Observacao | A tag/branch de preservacao inclui documentacao posterior; o baseline tecnico sera derivado do commit fixo pedido. |

## Migrations no Commit Legado

| Contexto legado | Caminho legado | Migration ID |
| --- | --- | --- |
| `Restaurante.IO.Api.DataContext.ApplicationDbContext` | `src/DevIO.Api/Migrations/20200817223121_InitialCreate.cs` | `20200817223121_InitialCreate` |
| `Restaurante.IO.Data.Context.MeuDbContext` | `src/DevIO.Data/Migrations/20200817223231_InitialCreate.cs` | `20200817223231_InitialCreate` |

## IDs Preservados Atualmente

| Contexto atual | Caminho atual | Migration ID preservado |
| --- | --- | --- |
| `ApplicationDbContext` | `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/Migrations/20200817223121_InitialCreate.cs` | `20200817223121_InitialCreate` |
| `SampleRestaurantDbContext` | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/Migrations/20200817223231_InitialCreate.cs` | `20200817223231_InitialCreate` |

## Migrations Somente na Versao Atual

| Contexto | Migration ID | Efeito observavel |
| --- | --- | --- |
| `SampleRestaurantDbContext` | `20260801191447_AddPratosPaginationOrderingIndex` | Cria o indice `IX_Pratos_Titulo_Id` em `Pratos(Titulo, Id)`. |

## Historico de Migrations

Nenhum contexto customiza `MigrationsHistoryTable`; ambos usam a tabela padrao `dbo.__EFMigrationsHistory`.

`ApplicationDbContext` e `SampleRestaurantDbContext` usam a mesma connection string `ConnectionStrings:DefaultConnection`. Portanto, Identity e SampleRestaurant compartilham banco e tabela de historico, embora suas migrations estejam em assemblies de infraestrutura separados.

## Testcontainers Existente

- `ApiFactory` usa `Testcontainers.MsSql` com imagem `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`.
- A fixture atual aplica migrations em banco vazio durante `InitializeAsync`.
- O teste de upgrade legado precisa de fixture propria para criar o schema legado antes de executar `MigrateAsync`.
- A nova fixture deve reaproveitar a imagem SQL Server do `ApiFactory` e usar database descartavel no container.

## Upgrade Observavel

O upgrade observavel minimo e a ausencia inicial e criacao posterior do indice `IX_Pratos_Titulo_Id`, adicionado pela migration de paginacao deterministica.

## Dados Minimos

- Um usuario em `AspNetUsers`, demonstrando preservacao do schema de Identity.
- Um atendente, uma mesa e um prato, demonstrando preservacao de entidades principais.
- Um pedido e um `PedidoPrato`, demonstrando preservacao de FKs e relacionamentos.

## Riscos

- Criar o baseline com migrations atuais produziria falso positivo, porque validaria a compatibilidade da versao moderna consigo mesma.
- Executar o upgrade pelo `ApiFactory` existente nao serve, pois a fixture aplica migrations antes do teste.
- O script legado deve ser pequeno e nao deve carregar credenciais nem comandos de criacao de database local.
