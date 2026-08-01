# Validation - Prompt 01

## Baseline inicial

- `dotnet --info`: SDK `10.0.302`.
- `dotnet restore RestauranteAPI.sln`: passou.
- `dotnet build RestauranteAPI.sln --configuration Release --no-restore`: passou.
- `dotnet test RestauranteAPI.sln --configuration Release --no-build`: passou, 41 testes em `Pedidos.Test` e 26 em `WebApiCoreSeed.IntegrationTests`.

## Validacao final

- `dotnet restore RestauranteAPI.sln`: passou.
- `dotnet build RestauranteAPI.sln --configuration Release --no-restore`: passou.
- `dotnet test RestauranteAPI.sln --configuration Release --no-build`: passou, 47 testes em `Pedidos.Test` e 26 em `WebApiCoreSeed.IntegrationTests`.
- `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --filter Architecture`: passou, 6 testes.
- `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build`: passou, 47 testes.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration`: passou, 26 testes.

## Smoke e regressao

- Smoke/regressao de `/api/v1/Pratos`, `/api/v1/Mesas/{id}`, autenticacao, autorizacao, Problem Details, rate limiting, security headers, health checks e readiness cobertos pelas suites existentes de `Pedidos.Test` e `WebApiCoreSeed.IntegrationTests`.
- Contrato OpenAPI regenerado com `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build`.
- `git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json`: passou, sem diferencas.

## Contratos preservados

- Sem alteracao em rotas.
- Sem alteracao em payloads.
- Sem alteracao em status codes esperados.
- Sem alteracao em autenticacao/autorizacao.
- Sem alteracao em Problem Details.
- Sem alteracao em rate limiting.
- Sem alteracao em health checks.
