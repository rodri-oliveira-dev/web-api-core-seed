# Validation - Prompt 04

## Planejado

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration
git grep -n "SaveChanges" -- "src/**/*.cs"
```

## Baseline inicial

- `dotnet build --configuration Release`: passou com warnings de analyzers ja existentes.
- `dotnet test --configuration Release --no-build`: passou, 48 testes unitarios/leves e 26 testes de integracao.

## A validar apos desenvolvimento

- Repositorios sem `SaveChanges`: validado por grep final.
- Services de escrita chamando Unit of Work uma vez no fluxo de sucesso: validado em `AtendenteServiceTest`.
- Services de escrita sem commit quando validacao falha: validado em `AtendenteServiceTest`.
- Excecao de commit propagada: validado em `AtendenteServiceTest`.
- SQL Server real preservando criacao, atualizacao, falha antes do commit, falha durante commit, duas alteracoes atomicas e ausencia de persistencia parcial: validado em `SqlServerIntegrationTests`.
- Smoke/regressao HTTP preservados pela suite de integracao e OpenAPI sem diff de contrato esperado: validado.

## Resultado final

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou sem warnings.
- `dotnet test --configuration Release --no-build`: passou, 49 testes em `WebApiCoreSeed.Tests` e 31 testes em `WebApiCoreSeed.IntegrationTests`.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration`: passou, 31 testes.
- `git grep -n "SaveChanges" -- "src/**/*.cs"` retornou apenas:
  - `SampleRestaurantDbContext.SaveChangesAsync`, override autorizado.
  - `base.SaveChangesAsync`, chamada autorizada dentro do override.
- `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build`: passou.
- `git diff -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json`: sem diff de conteudo.
