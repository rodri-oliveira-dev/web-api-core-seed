# Discovery - Prompt 06

## Baseline

| Comando | Resultado |
| --- | --- |
| `git status --short` | Limpo. |
| `git branch --show-current` | `phase/4-architecture-modernization`. |
| `git log -3 --oneline` | `e43a21d refactor: propagate cancellation tokens`; `3b79535 refactor: define explicit unit of work boundary`; `d861a72 refactor: replace generic repository with explicit ports`. |
| `dotnet build --configuration Release` | Passou com warnings de analyzer preexistentes. |
| `dotnet test --configuration Release --no-build` | Passou: 53 testes em `WebApiCoreSeed.Tests` e 32 em `WebApiCoreSeed.IntegrationTests`. |

## DbContexts Ativos

| DbContext | Caminho | Assembly atual | Uso |
| --- | --- | --- | --- |
| `ApplicationDbContext` | `src/WebApiCoreSeed.Api/DataContext/ApplicationContext.cs` | `WebApiCoreSeed.Api` | ASP.NET Core Identity. |
| `SampleRestaurantDbContext` | `src/SampleRestaurant.Infrastructure/Modules/SampleRestaurant/Infrastructure/Persistence/Context/SampleRestaurantDbContext.cs` | `WebApiCoreSeed.SampleRestaurant.Infrastructure` | Persistencia do modulo demonstrativo `SampleRestaurant`. |

## Migrations Encontradas

- API: `src/WebApiCoreSeed.Api/Migrations/20200817223121_InitialCreate*` e `ApplicationDbContextModelSnapshot.cs`.
- Sample Infrastructure: `src/SampleRestaurant.Infrastructure/Migrations/20200817223231_InitialCreate*` e `SampleRestaurantDbContextModelSnapshot.cs`.

## Configuracao EF

- `HostingConfig.AddApiServices` registra `SampleRestaurantDbContext` com `UseSqlServer(defaultConnection)` sem `MigrationsAssembly` explicito.
- `IdentityConfig.AddIdentityConfiguration` registra `ApplicationDbContext` com `UseSqlServer(DefaultConnection)` sem `MigrationsAssembly` explicito.
- Nao havia `IDesignTimeDbContextFactory`.

## Greps Solicitados

| Busca | Resultado |
| --- | --- |
| `git grep -n "MigrationsAssembly"` | Sem ocorrencias. |
| `git grep -n "IDesignTimeDbContextFactory"` | Sem ocorrencias. |
| `git grep -n "Database.Migrate"` | Apenas `test/WebApiCoreSeed.IntegrationTests/Infrastructure/ApiFactory.cs`, aplicando migrations em Testcontainers. |
| `git grep -n "EnsureCreated"` | Apenas documentacao historica. |
| `git grep -n "HasData"` | Apenas documentacao historica. |
| `rg -n "\bSeed\b|\bseed\b|HasData|EnsureCreated|Database\.Migrate|INSERT INTO|insert into"` | Nenhum seed ativo; apenas docs, nomes `WebApiCoreSeed` e testes aplicando migrations. |

## dotnet ef

`dotnet ef --version`, `dotnet ef dbcontext list` e `dotnet ef migrations list` falharam inicialmente porque `dotnet-ef` nao estava instalado no PATH do ambiente.

A ferramenta `dotnet-ef` 10.0.10 foi instalada globalmente no ambiente para validacao deste prompt.

## Descoberta Durante Desenvolvimento

- `dotnet ef migrations has-pending-model-changes` apontou diferenca inicial em `ApplicationDbContext`.
- Uma migration temporaria de diagnostico mostrou que a diferenca era a perda de `maxLength: 128` nas chaves `LoginProvider`, `ProviderKey` e `Name` de `AspNetUserLogins`/`AspNetUserTokens`.
- A migration temporaria foi removida e nao faz parte da entrega.
- `ApplicationDbContext` passou a usar a base generica de 8 tipos do Identity e configuracao explicita de max length 128 para preservar o schema legado sem gerar migration nova.

## Testes de Integracao

`ApiFactory` usa SQL Server e Redis via Testcontainers. Durante `InitializeAsync`, aplica:

- `ApplicationDbContext.Database.MigrateAsync()`;
- `SampleRestaurantDbContext.Database.MigrateAsync()`.

O teste `MigrationsQuandoBancoVazioDeveCriarSchema` valida que as migrations `20200817223121_InitialCreate` e `20200817223231_InitialCreate` foram aplicadas em banco vazio.
