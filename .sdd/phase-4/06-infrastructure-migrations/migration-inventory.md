# Migration Inventory - Prompt 06

| DbContext | Caminho atual | Assembly atual | Snapshot | Qtde migrations | Projeto destino | Schema/tabelas | Risco |
| --- | --- | --- | --- | ---: | --- | --- | --- |
| `ApplicationDbContext` | `src/WebApiCoreSeed.Api/DataContext/ApplicationContext.cs` | `WebApiCoreSeed.Api` | `src/WebApiCoreSeed.Api/Migrations/ApplicationDbContextModelSnapshot.cs` | 1 | `src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj` | Identity ASP.NET Core: `AspNetUsers`, `AspNetRoles`, claims, logins, roles, tokens | Medio: mover contexto e migration exige atualizar DI, testes, tooling e comandos EF sem alterar schema. |
| `SampleRestaurantDbContext` | `src/SampleRestaurant.Infrastructure/Modules/SampleRestaurant/Infrastructure/Persistence/Context/SampleRestaurantDbContext.cs` | `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `src/SampleRestaurant.Infrastructure/Migrations/SampleRestaurantDbContextModelSnapshot.cs` | 1 | `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | Sample: `Atendentes`, `Mesas`, `Pratos`, `Pedidos`, `PedidoPrato`, `Loggin` | Baixo: migration ja esta na infraestrutura; falta explicitar assembly e factory. |

## Migrations Preservadas

- `20200817223121_InitialCreate`: Identity.
- `20200817223231_InitialCreate`: SampleRestaurant.

Nenhuma migration nova deve ser gerada apenas pela mudanca de pasta.

## Estado Final

| DbContext | Caminho final | Assembly final | Snapshot final | Qtde migrations | Schema/tabelas | Risco residual |
| --- | --- | --- | --- | ---: | --- | --- |
| `ApplicationDbContext` | `src/Identity.Infrastructure/Context/ApplicationDbContext.cs` | `WebApiCoreSeed.Identity.Infrastructure` | `src/Identity.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs` | 1 | Identity ASP.NET Core legado, com max length 128 preservado em login/token keys | Baixo: sem pending model changes; passkeys de Identity 10 nao foram introduzidas por preservar schema historico. |
| `SampleRestaurantDbContext` | `src/SampleRestaurant.Infrastructure/Modules/SampleRestaurant/Infrastructure/Persistence/Context/SampleRestaurantDbContext.cs` | `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `src/SampleRestaurant.Infrastructure/Migrations/SampleRestaurantDbContextModelSnapshot.cs` | 1 | SampleRestaurant legado | Baixo: sem pending model changes. |
