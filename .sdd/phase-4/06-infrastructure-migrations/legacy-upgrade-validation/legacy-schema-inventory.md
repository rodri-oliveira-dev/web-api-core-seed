# Legacy Schema Inventory

## Fonte

Inventario derivado das migrations do commit `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d`:

- `src/DevIO.Api/Migrations/20200817223121_InitialCreate.cs`
- `src/DevIO.Data/Migrations/20200817223231_InitialCreate.cs`

Blob IDs Git no commit legado:

- Identity migration: `be224608397a0f3a4fd8613ad166df2f4d6aec21`
- SampleRestaurant migration: `580219d970d4b793da44c9a93f0ea6efa45a831c`

Baseline versionado:

- Arquivo: `tests/WebApiCoreSeed.IntegrationTests/Infrastructure/LegacyUpgrade/legacy-schema-baseline.sql`
- SHA-256: `DB3116099B513AB76C4BEFB37AED1138B6A9493E8A3DAC564C767895BC0B5601`

O arquivo `sql/restaurante.sql` legado tambem existe, mas nao sera usado como baseline porque contem script amplo de banco local, comandos `DROP/CREATE DATABASE`, codificacao UTF-16, diferencas estruturais em relacao as migrations EF e nao e especifico para o cenario de teste.

## Tabela de Historico

- `dbo.__EFMigrationsHistory`
- `MigrationId nvarchar(150) not null`
- `ProductVersion nvarchar(32) not null`
- PK: `PK___EFMigrationsHistory` em `MigrationId`
- IDs registrados no baseline:
  - `20200817223121_InitialCreate`
  - `20200817223231_InitialCreate`

## Identity

Tabelas:

- `AspNetRoles`
- `AspNetUsers`
- `AspNetRoleClaims`
- `AspNetUserClaims`
- `AspNetUserLogins`
- `AspNetUserRoles`
- `AspNetUserTokens`

Indices e constraints relevantes:

- `RoleNameIndex` unico filtrado em `AspNetRoles(NormalizedName)`.
- `UserNameIndex` unico filtrado em `AspNetUsers(NormalizedUserName)`.
- `EmailIndex` em `AspNetUsers(NormalizedEmail)`.
- FKs das tabelas de claims, logins, roles e tokens para users/roles com cascade.
- PK composta de `AspNetUserLogins(LoginProvider, ProviderKey)`.
- PK composta de `AspNetUserRoles(UserId, RoleId)`.
- PK composta de `AspNetUserTokens(UserId, LoginProvider, Name)`.
- `AspNetUserLogins.LoginProvider`, `AspNetUserLogins.ProviderKey`, `AspNetUserTokens.LoginProvider` e `AspNetUserTokens.Name` usam max length 128 no baseline EF.

## SampleRestaurant

Tabelas:

- `Atendentes`
- `Loggin`
- `Mesas`
- `Pratos`
- `Pedidos`
- `PedidoPrato`

Indices e constraints relevantes:

- PKs em `Id` para todas as tabelas do sample.
- `IX_Pedidos_AtendenteId`.
- `IX_Pedidos_MesaId`.
- `IX_PedidoPrato_PedidoId`.
- `IX_PedidoPrato_PratoId`.
- FK `FK_Pedidos_Atendentes`.
- FK `FK_Pedidos_Mesas`.
- FK `FK_PedidoPrato_Pedidos`.
- FK `FK_PedidoPrato_Pratos`.

## Diferenca Posterior ao Legado

- `IX_Pratos_Titulo_Id` nao existe no baseline.
- `IX_Pratos_Titulo_Id` deve existir depois do upgrade atual.
