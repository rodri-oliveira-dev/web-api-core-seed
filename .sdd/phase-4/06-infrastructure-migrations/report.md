# Report - Prompt 06

## Resumo

As migrations de Identity foram movidas da API para `WebApiCoreSeed.Identity.Infrastructure`. As migrations do modulo demonstrativo continuam em `WebApiCoreSeed.SampleRestaurant.Infrastructure`. Ambos os DbContexts agora possuem `MigrationsAssembly` explicito e factory design-time.

## Migrations Movidas

- `ApplicationDbContext`: `20200817223121_InitialCreate` e `ApplicationDbContextModelSnapshot` foram movidos para `src/Identity.Infrastructure/Migrations`.
- `SampleRestaurantDbContext`: `20200817223231_InitialCreate` permaneceu em `src/SampleRestaurant.Infrastructure/Migrations`.

## Assemblies

- Identity: `WebApiCoreSeed.Identity.Infrastructure`.
- SampleRestaurant: `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- Startup EF: `WebApiCoreSeed.Api`.

## Factories

- `ApplicationDbContextFactory`.
- `SampleRestaurantDbContextFactory`.

As factories leem `ConnectionStrings:DefaultConnection` a partir de `src/WebApiCoreSeed.Api/appsettings*.json` e variaveis de ambiente.

## Compatibilidade de Schema

`ApplicationDbContext` usa a base generica de 8 tipos do Identity e configura `IdentityUserLogin<string>`/`IdentityUserToken<string>` com max length 128 para preservar a migration historica. Nao foi criada migration nova.

## Seed

Nao existe seed runtime, `HasData`, initializer ou comando de seed. Testes criam dados por caso.

## Validacao

- Restore/build/test passaram.
- `dotnet ef dbcontext list` e `dotnet ef migrations list` passaram por contexto.
- `dotnet ef migrations has-pending-model-changes` passou para ambos os contextos.
- Scripts idempotentes foram gerados em `%TEMP%`.
- SQL Server Testcontainers aplicou migrations em banco vazio.
- API nao contem arquivos de migration.

## Delivery

- Commit semantico planejado: `refactor: move EF Core migrations to infrastructure`.
- Push: nao realizado.
- Proximo prompt: Prompt 7 - Paginacao deterministica.
- Proxima issue registrada: `#19`.
