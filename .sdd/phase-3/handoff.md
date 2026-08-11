# Handoff - Phase 3

## Estado final local

- Branch atual: `phase/3-quality-and-safety`.
- Branch-base: `phase/2-dotnet-10-migration`.
- Fase 3: concluida localmente.
- Push: pendente.
- PR: pendente.
- Proxima branch planejada: `phase/4-architecture-modernization`.
- Issues seguintes: `#14`, `#15`, `#16`, `#17`, `#18`, `#19`, `#20`.

## Status dos prompts

- 01 - Testes unitarios: concluido.
- 02 - Testes de integracao: concluido.
- 03 - Seguranca: concluido.
- 04 - OpenTelemetry: concluido.
- 05 - CI e gates: concluido.

## Cinco commits da fase

- `d8730d3 test: strengthen existing unit test suite`
- `e21a215 test: add API and infrastructure integration tests`
- `c9c4641 fix: harden API security defaults`
- `4b493e2 feat: add OpenTelemetry observability`
- `ci: add quality and security workflows` (este commit)

## Testes e cobertura

- Suite final: 41 testes em `Pedidos.Test` e 26 testes em `WebApiCoreSeed.IntegrationTests`.
- `dotnet test RestauranteAPI.sln --configuration Release --no-build`: passou.
- Cobertura baseline geral historica: 29,15% linhas / 17,66% branches.
- Cobertura local do comando de CI unit: 32,78% linhas / 20,42% branches.
- Cobertura local do comando de CI integration: 67,41% linhas / 23,54% branches.
- Nenhum threshold novo foi definido.

## Testcontainers e WebApplicationFactory

- Projeto de integracao: `test/WebApiCoreSeed.IntegrationTests`.
- Host: `WebApplicationFactory<Program>` em ambiente `Testing`.
- Containers: SQL Server `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04` e Redis `redis:7.4.2-alpine`.
- Isolamento: collection xUnit compartilhada, reset SQL/Redis antes de cada teste.
- Docker e requisito para a suite de integracao e para o CI principal.

## Seguranca

- CORS fechado por padrao em producao quando `Cors:AllowedOrigins` esta vazio.
- Forwarded headers desabilitados por padrao e restritos a proxies/redes conhecidos quando habilitados.
- Headers modernos ativos: `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`, CSP com `frame-ancestors 'none'`, `X-Frame-Options` e no-store em respostas sensiveis.
- `/health/live` e `/hc` expoem status minimo; `/health/ready` expoe detalhes apenas em Development/Testing.
- `dotnet list package --vulnerable`: nenhum pacote vulneravel.
- Dependency Review ativo em PR com `fail-on-severity: moderate`.
- CodeQL ativo para C#.

## OpenTelemetry

- Registro central em `AddApiOpenTelemetry`.
- Traces/metrics: ASP.NET Core, HttpClient, EF Core e Runtime.
- OTLP opcional; startup passa com telemetria desativada e ativada sem collector obrigatorio.
- Serilog permanece pipeline de logs com `TraceId` e `SpanId`.
- Redis spans nao foram adicionados; Redis segue coberto por health/readiness e testes de integracao.

## Workflows e check names

- `.github/workflows/ci.yml`: `Build, test and quality gates`.
- `.github/workflows/codeql.yml`: `CodeQL analysis`.
- `.github/workflows/dependency-review.yml`: `Review dependency changes`.
- `.github/dependabot.yml`: updates semanais agrupados para NuGet e GitHub Actions.

## Comandos locais

```text
dotnet restore RestauranteAPI.sln
dotnet build RestauranteAPI.sln --configuration Release --no-restore
dotnet test RestauranteAPI.sln --configuration Release --no-build
dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory TestResults/Unit
dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory TestResults/Integration
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json
dotnet list RestauranteAPI.sln package --vulnerable
dotnet list RestauranteAPI.sln package --deprecated
```

## Debitos tecnicos e riscos

- `dotnet format --verify-no-changes` falha por divida de whitespace existente; gate adiado.
- `xunit` 2.9.3 aparece como deprecated/Legacy nos projetos de teste.
- Nao ha `packages.lock.json`; cache NuGet melhora tempo de restore, mas lock files seriam melhoria futura de reproducibilidade.
- `actionlint` nao estava disponivel localmente; YAML foi validado com PyYAML.
- EF Core OpenTelemetry instrumentation permanece em pacote beta.
- Producao precisa configurar `Cors:AllowedOrigins` e, se houver proxy, `ForwardedHeaders`.

## Proximo PR

O PR futuro da Fase 3 deve conter:

```text
Closes #9
Closes #10
Closes #11
Closes #12
Closes #13
```
