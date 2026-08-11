# Validation - Prompt 06

| Comando | Resultado |
| --- | --- |
| `dotnet restore` | Passou. |
| `dotnet build --configuration Release --no-restore` | Passou com warnings de analyzer preexistentes. |
| `dotnet test --configuration Release --no-build` | Passou: 53 testes em `WebApiCoreSeed.Tests` e 32 em `WebApiCoreSeed.IntegrationTests`. |
| `dotnet ef --version` | `10.0.10`. |
| `dotnet ef dbcontext list --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build` | Listou `WebApiCoreSeed.Identity.Infrastructure.Context.ApplicationDbContext`. |
| `dotnet ef dbcontext list --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build` | Listou `WebApiCoreSeed.SampleRestaurant.Infrastructure.Context.SampleRestaurantDbContext`. |
| `dotnet ef migrations list --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --configuration Release --no-build --no-connect` | Listou `20200817223121_InitialCreate`. |
| `dotnet ef migrations list --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --configuration Release --no-build --no-connect` | Listou `20200817223231_InitialCreate`. |
| `dotnet ef migrations has-pending-model-changes` para `ApplicationDbContext` | Passou: no changes. |
| `dotnet ef migrations has-pending-model-changes` para `SampleRestaurantDbContext` | Passou: no changes. |
| `dotnet ef migrations script ... --idempotent --output %TEMP%/...` para Identity | Passou; script temporario gerado fora do repo. |
| `dotnet ef migrations script ... --idempotent --output %TEMP%/...` para SampleRestaurant | Passou; script temporario gerado fora do repo. |
| `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~MigrationsQuandoBancoVazioDeveCriarSchema"` | Passou; SQL Server Testcontainers aplicou migrations em banco vazio e descartou o banco. |
| `Get-ChildItem -Path src/WebApiCoreSeed.Api -Recurse -File \| Where-Object { $_.FullName -like '*Migrations*' }` | Sem arquivos. |

## Observacoes

- `dotnet ef migrations list --no-connect` mostra a migration, mas nao informa pending/applied status por nao conectar ao banco local; isso foi intencional para nao tocar banco do usuario.
- Uma migration temporaria de diagnostico `__IdentityPendingCheck` foi gerada e removida durante development para identificar diferenca de modelo; ela nao foi versionada.
- Validacao de upgrade a partir de schema legado preservado foi feita indiretamente por IDs e snapshots preservados, e diretamente por `has-pending-model-changes` sem diferenca de modelo. Nenhum banco legado local do usuario foi alterado.
