# Discovery - Development Seed

## Baseline

- Branch base atualizada: `main`.
- Branch de trabalho: `feat/idempotent-development-seed`.
- Solution ativa: `WebApiCoreSeed.slnx`.
- API/composition root: `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`.
- Identity infrastructure: `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure`.
- SampleRestaurant core: `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant`.
- SampleRestaurant infrastructure: `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure`.

## Documentacao Lida

- `AGENTS.md`.
- `README.md`.
- `LEGACY.md`.
- `docs/development/containerized-local-development.md`.
- `.sdd/phase-4/status.md`.
- `.sdd/phase-4/handoff.md`.
- `.sdd/phase-4/06-infrastructure-migrations/*`.

## DbContexts

- `ApplicationDbContext` herda de `IdentityDbContext` e preserva schema legado de Identity com max length 128 em logins/tokens.
- `SampleRestaurantDbContext` expoe `Atendentes`, `Mesas`, `PedidoPrato`, `Pedidos`, `Pratos` e `Loggins`.
- Os contextos usam a mesma connection string, mas nao compartilham uma Unit of Work distribuida.
- `SampleRestaurantDbContext.SaveChangesAsync` preserva comportamento legado de `DataCadastro` quando existir.

## Migrations

- Identity possui migration `20200817223121_InitialCreate`.
- SampleRestaurant possui migrations `20200817223231_InitialCreate` e `20260801191447_AddPratosPaginationOrderingIndex`.
- O servico Compose `migrations` aplica os dois contextos por `dotnet ef database update`.
- Testes de integracao aplicam `Database.MigrateAsync` para os dois contextos em `ApiFactory`.

## Autenticacao Atual

- Login existente: `POST /api/v{version}/entrar`.
- V1 assina JWT com `HS384`; V2 com `HS256`.
- Claims do usuario sao lidas por `UserManager.GetClaimsAsync`.
- Roles sao lidas por `UserManager.GetRolesAsync` e emitidas como claim `role`.
- Endpoints protegidos usam claims como `Mesas=ObterPorId`, `Mesas=Adicionar`, `Pratos=Adicionar`.
- Registro V1 existe, mas exige autenticacao; o seed deve usar `UserManager<IdentityUser>` diretamente.

## Infraestrutura Local

- `compose.yaml` possui `sqlserver`, `redis`, `migrations` e `api`.
- `.env.local.example` contem apenas placeholders para SQL/JWT/portas.
- Host mode usa User Secrets para `ConnectionStrings:DefaultConnection` e `AppSettings:Secret`.
- Docker mode usa variaveis em `.env.local`.

## Testcontainers e Reset

- `ApiFactory` usa SQL Server e Redis reais por Testcontainers.
- `DatabaseReset` remove dados de Identity e SampleRestaurant, mas preserva `__EFMigrationsHistory`.
- O reset atual limpa tabelas conhecidas e e adequado para testes do seed apos migrations.

## Ausencia de Seed Ativo

Busca por `Seed`, `HasData`, `EnsureCreated`, `Database.Migrate`, initializers e inserts versionados confirmou:

- Nao ha seed runtime ativo.
- Nao ha `HasData` no modelo ativo.
- Nao ha `EnsureCreated`.
- Nao ha initializer de banco.
- A unica aplicacao automatica de migrations ocorre em testes; Compose aplica migrations por servico explicito.
