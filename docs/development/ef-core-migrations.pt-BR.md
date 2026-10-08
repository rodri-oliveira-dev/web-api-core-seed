# Migrations EF Core (aplicação .NET 10 mantida)

[Voltar ao README](../../README.pt-BR.md) | [English](ef-core-migrations.md)

A solução atual possui **dois DbContexts com migrations independentes**. Execute os comandos na raiz do repositório. Para iniciar o ambiente de desenvolvimento, prefira o serviço Compose:

```bash
docker compose --env-file .env.local up migrations
```

O serviço `migrations` do `compose.yaml` aplica `ApplicationDbContext` e depois `SampleRestaurantDbContext`. Não rode migrations em produção sem revisão e backup.

## Antes dos comandos EF CLI

- Instale o SDK definido em `global.json` e `dotnet-ef` **10.x**: `dotnet tool install --global dotnet-ef --version "10.*"` (ou atualize a ferramenta existente dentro da série 10).
- Prepare o SQL Server: `docker compose --env-file .env.local up -d sqlserver`.
- Configure os User Secrets da API para `ConnectionStrings:DefaultConnection` e `AppSettings:Secret` conforme o README.
- Trabalhe no ambiente Development e em banco local descartável, não em produção.

## DbContext Identity

As migrations ficam em `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/Migrations`:

```bash
dotnet ef migrations list --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --no-connect

dotnet ef migrations add AddIdentityChange --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --output-dir Migrations

dotnet ef database update --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext
```

## DbContext SampleRestaurant

As migrations ficam em `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/Migrations`:

```bash
dotnet ef migrations list --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --no-connect

dotnet ef migrations add AddRestaurantChange --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --output-dir Migrations

dotnet ef database update --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext
```

## Revisão antes de alterar o esquema

Gere um script SQL idempotente substituindo `database update` por `migrations script --idempotent --output <caminho>` e mantendo os argumentos `--project`, `--startup-project` e `--context`. Revise o script antes de qualquer execução não local. Nomes históricos como `Loggin` são contratos de compatibilidade; não os renomeie sem plano de migração.

Use `dotnet ef migrations has-pending-model-changes` com os mesmos argumentos de contexto para detectar alterações não migradas. Os testes de integração com Docker/Testcontainers incluem o cenário de upgrade do esquema legado. Veja [migração do legado](../migration-from-legacy.md).

## O seed é uma operação separada

O comando `--seed` invoca `MigrateAsync` para os dois contextos e depois atualiza dados de exemplo. Veja a [seção de seed no README](../../README.pt-BR.md#gerar-dados-de-exemplo-seed). O comando recusa execução em Production. **Não é** uma migration EF nem uma migração de dados de produção; também não executa no início normal da API.