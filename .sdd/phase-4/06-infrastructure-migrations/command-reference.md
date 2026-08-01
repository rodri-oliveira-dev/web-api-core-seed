# Command Reference - Prompt 06

> Comandos a validar apos a mudanca. Todos assumem execucao a partir da raiz do repositorio.

## Identity

Listar DbContexts:

```bash
dotnet ef dbcontext list --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build
```

Listar migrations:

```bash
dotnet ef migrations list --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --configuration Release --no-build --no-connect
```

Criar migration:

```bash
dotnet ef migrations add NomeDaMigration --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --output-dir Migrations
```

Gerar script:

```bash
dotnet ef migrations script --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --configuration Release --no-build --idempotent --output "%TEMP%/identity-migrations.sql"
```

Aplicar migration:

```bash
dotnet ef database update --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext
```

Remover ultima migration ainda nao aplicada:

```bash
dotnet ef migrations remove --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext
```

## SampleRestaurant

Listar migrations:

```bash
dotnet ef dbcontext list --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build
```

Listar migrations:

```bash
dotnet ef migrations list --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --configuration Release --no-build --no-connect
```

Criar migration:

```bash
dotnet ef migrations add NomeDaMigration --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --output-dir Migrations
```

Gerar script:

```bash
dotnet ef migrations script --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --configuration Release --no-build --idempotent --output "%TEMP%/sample-restaurant-migrations.sql"
```

Aplicar migration:

```bash
dotnet ef database update --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext
```

Remover ultima migration ainda nao aplicada:

```bash
dotnet ef migrations remove --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext
```

## Banco Vazio

Validar banco SQL Server vazio sem tocar banco local:

```bash
dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "FullyQualifiedName~MigrationsQuandoBancoVazioDeveCriarSchema"
```

## Seed

Nao existe seed runtime ou migration seed. Nao ha comando de seed a executar. Dados de teste sao criados pelos testes e nao representam seed de desenvolvimento.

## Validar Modelo Sem Migration Pendente

```bash
dotnet ef migrations has-pending-model-changes --project src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext --configuration Release --no-build
dotnet ef migrations has-pending-model-changes --project src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext --configuration Release --no-build
```
