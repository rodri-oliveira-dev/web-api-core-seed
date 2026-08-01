# Validation - 05 CI Quality Gates

## Validacao Inicial

| Comando | Resultado |
| --- | --- |
| `git status` | limpo no inicio |
| `git branch --show-current` | `phase/3-quality-and-safety` |
| `git log -5 --oneline` | confirmou commits dos prompts 01-04 |
| `dotnet build --configuration Release` | passou com 34 warnings de analyzer existentes |
| `dotnet test --configuration Release --no-build` | passou com 41 testes em `Pedidos.Test` e 26 em `WebApiCoreSeed.IntegrationTests` |

## Validacoes de Discovery

| Comando | Resultado |
| --- | --- |
| `git grep -n -i "sonar" -- .github .vscode AGENTS.md` | vazio |
| `git grep -n "PocArquitetura" -- .github` | vazio |
| `git grep -n "LedgerService" -- .github` | vazio |
| `git grep -n "BalanceService" -- .github` | vazio |
| `git grep -n "TransferService" -- .github` | vazio |
| `dotnet format RestauranteAPI.sln --verify-no-changes --verbosity minimal` | falhou por divida de whitespace existente |
| `dotnet list RestauranteAPI.sln package --vulnerable` | nenhum pacote vulneravel |
| `dotnet list RestauranteAPI.sln package --deprecated` | `xunit` 2.9.3 reportado como `Legacy` nos projetos de teste |
| `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build` | passou e nao alterou arquivos versionados |
| `actionlint` | nao disponivel localmente |

## Validacao Consolidada

| Comando | Resultado |
| --- | --- |
| `dotnet --info` | SDK 10.0.302, runtime 10.0.10 |
| `dotnet restore RestauranteAPI.sln` | passou |
| `dotnet build RestauranteAPI.sln --configuration Release --no-restore` | passou |
| `dotnet test RestauranteAPI.sln --configuration Release --no-build` | passou: 41 + 26 testes |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage"` | passou: 41 testes; cobertura 32,78% linhas / 20,42% branches |
| `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage"` | passou: 26 testes; cobertura 67,41% linhas / 23,54% branches |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --filter FullyQualifiedName~ObservabilityConfigurationTests` | passou: 5 testes |
| `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build` | passou |
| parse JSON de `docs/openapi/openapi-v*.json` | passou |
| `git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json` | passou |
| `dotnet list RestauranteAPI.sln package --vulnerable` | nenhum pacote vulneravel |
| `dotnet list RestauranteAPI.sln package --deprecated` | `xunit` 2.9.3 reportado como `Legacy` nos projetos de teste |
| `git diff --check` | passou |
| PyYAML parse de `.github/**/*.yml` | passou |
| `git grep -n -i "sonar" -- .github .vscode AGENTS.md` | vazio |
| `git grep -n -i -E "token:|password:|secret:" -- .github` | vazio |
| `git grep -n "PocArquitetura\|LedgerService\|BalanceService\|TransferService" -- .github` | vazio |

## Smoke Consolidado

Coberto pela suite de integracao e pelos testes de observabilidade:

- Endpoint publico: `ObterPratosQuandoEndpointValidoDeveRetornarSucesso`.
- Endpoint protegido: unauthorized e forbidden em `ApiContractIntegrationTests`.
- Problem Details: validation, not found, unauthorized, forbidden, domain rule e rate limit.
- Rate limiting: `ObterPratosQuandoAcimaDoLimiteDeveRetornarTooManyRequests`.
- OpenAPI: `OpenApiQuandoHostDeTesteDeveResponder` e gerador versionado.
- Health: `/health/live`, `/hc` e `/health/ready`.
- SQL Server: migrations, persistencia, FK, indice unico, transacao e funcao nativa.
- Redis: leitura/escrita, chave ausente e expiracao.
- Telemetria desativada e ativada sem collector obrigatorio: `ObservabilityConfigurationTests`.

## Limitacoes

- `actionlint` nao estava instalado; nao foi instalado automaticamente.
- `dotnet format --verify-no-changes` foi avaliado, mas nao habilitado como gate por falhar em divida existente.
