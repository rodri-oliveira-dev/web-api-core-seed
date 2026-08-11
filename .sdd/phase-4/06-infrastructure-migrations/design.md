# Design - Prompt 06

## Decisao Principal

Criar `WebApiCoreSeed.Identity.Infrastructure` para hospedar `ApplicationDbContext` e suas migrations. Isso remove migrations da API sem misturar Identity com a infraestrutura do modulo demonstrativo.

## ApplicationDbContext

- Projeto: `src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`.
- Namespace do contexto: `WebApiCoreSeed.Identity.Infrastructure.Context`.
- Migrations: `src/Identity.Infrastructure/Migrations`.
- `MigrationsAssembly`: `typeof(ApplicationDbContext).Assembly.FullName`.
- Startup project EF: `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`.
- Factory: `ApplicationDbContextFactory`, lendo `ConnectionStrings:DefaultConnection` da configuracao da API ou de variaveis de ambiente.
- Schema legado preservado: `ApplicationDbContext` usa a base generica de 8 tipos do Identity e fixa `maxLength: 128` para chaves de `IdentityUserLogin<string>` e `IdentityUserToken<string>`, evitando migration estrutural gerada pela mudanca de defaults do Identity 10.

## SampleRestaurantDbContext

- Projeto: `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj`.
- Migrations permanecem em `src/SampleRestaurant.Infrastructure/Migrations`.
- `MigrationsAssembly`: `typeof(SampleRestaurantDbContext).Assembly.FullName`.
- Startup project EF: `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`.
- Factory: `SampleRestaurantDbContextFactory`, lendo `ConnectionStrings:DefaultConnection` da configuracao da API ou de variaveis de ambiente.

## Seed

Nao existe seed via `HasData`, initializer, script versionado de inserts ou runtime seed. Testes criam dados por caso e limpam tabelas via `DatabaseReset`. Nenhum seed automatico sera adicionado.

## Compatibilidade

- IDs de migrations e nomes de tabelas permanecem iguais.
- `__EFMigrationsHistory` continua contendo `20200817223121_InitialCreate` e `20200817223231_InitialCreate`.
- Banco legado com essas migrations aplicadas deve continuar reconhecido porque o migration id nao muda.
