# Discovery - 05 CI Quality Gates

## Inventario `.github/`

Arquivos existentes antes da entrega:

- `.github/CODEOWNERS`
- `.github/PULL_REQUEST_TEMPLATE.md`
- `.github/dependabot.yml`
- `.github/workflows/dependency-review.yml`

## Workflows

- Ativos antes da entrega: `dependency-review.yml`.
- Criados nesta entrega: `ci.yml`, `codeql.yml`.
- Removidos nesta entrega: nenhum.
- Workflows adiados: nenhum arquivo adiado encontrado em `.github/workflows`.

## Dependabot

Configuracao existente cobria NuGet e GitHub Actions semanalmente, com limite de 5 PRs por ecossistema e labels `dependencies`, `dotnet`, `github-actions`.

## OpenAPI

- Gerador existente: `tools/OpenApiGenerator/OpenApiGenerator.csproj`.
- Contratos versionados: `docs/openapi/openapi-v1.json` e `docs/openapi/openapi-v2.json`.
- Comando local validado sem alterar a arvore: `dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build`.

## Testcontainers

- Projeto: `test/WebApiCoreSeed.IntegrationTests`.
- Containers: SQL Server `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04` e Redis `redis:7.4.2-alpine`.
- Todos os testes do projeto usam a collection `api-integration`.
- Traits existentes: `Category=Integration` e `Category=Container`.
- Docker local estava disponivel durante a validacao inicial.

## Cobertura

- Ferramenta adotada: `coverlet.collector` via `XPlat Code Coverage`.
- Nao existe `coverlet.runsettings`.
- Threshold: nenhum threshold confiavel foi definido na Fase 3; decisao D002 preserva cobertura como baseline informativa.

## Analyzers e Formatacao

- Analyzers rodam no build por `AnalysisLevel=latest-recommended`.
- `dotnet format RestauranteAPI.sln --verify-no-changes --verbosity minimal` falha por divida de whitespace existente em arquivos ativos.
- O gate de formatacao foi documentado como pendente para evitar CI permanentemente vermelho.

## Pesquisas Obrigatorias

Resultados em `.github`:

- `git grep -n -i "sonar" -- .github .vscode AGENTS.md`: vazio.
- `git grep -n "PocArquitetura" -- .github`: vazio.
- `git grep -n "LedgerService" -- .github`: vazio.
- `git grep -n "BalanceService" -- .github`: vazio.
- `git grep -n "TransferService" -- .github`: vazio.

Observacao: existem arquivos historicos `src/sonar-project.properties` e `src/sonar-push.bat`, mas nenhum workflow ativo os referencia.

## Riscos Descobertos

- Nao ha `packages.lock.json`; cache NuGet invalida por manifests, mas a reproducibilidade perfeita de dependencias exigiria lock files em uma entrega futura.
- `dotnet list package --deprecated` reporta `xunit` 2.9.3 como `Legacy` nos projetos de teste; foi mantido como relatorio informativo.
- `actionlint` nao estava disponivel localmente.
