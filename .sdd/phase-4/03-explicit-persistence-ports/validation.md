# Validation - Prompt 03

## Plano

Executar sequencialmente:

```text
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration
git grep literal da interface generica legada
git grep literal da implementacao generica legada
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
git diff -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json
```

## Resultados

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou com 21 warnings de analyzers ja existentes na API.
- `dotnet test --configuration Release --no-build`: passou, 48 testes em `WebApiCoreSeed.Tests` e 26 em `WebApiCoreSeed.IntegrationTests`.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration`: passou, 26 testes.
- Grep literal da interface generica legada: vazio.
- Grep literal da implementacao generica legada: vazio.
- `git grep -n "Expression<Func" -- src test`: vazio.
- `git grep -n "Console.WriteLine" -- src`: apenas `MemoryMetricsClient`, fora da persistencia alterada.
- `git grep -n "catch (Exception" -- src`: apenas middleware/programa da API, fora da persistencia alterada.
- `git grep -n "return null" -- src`: apenas `RateLimitConfig`, fora da persistencia alterada.
- `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build`: passou.
- `git diff -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json`: sem diff de conteudo.

## Observacao

A primeira execucao inicial de `dotnet build --configuration Release` falhou por lock de DLL porque foi rodada em paralelo com `dotnet test --configuration Release --no-build`. A validacao final sequencial passou.
