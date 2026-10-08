# Status - Phase 5

| Prompt | Status |
| --- | --- |
| 01 - Development seed deterministico e idempotente | PR aberto; checks remotos passaram |
| 02 - Normalize UTF-8 encoding and active code naming | PR aberto; checks remotos passaram |
| 03 - Bilingual onboarding, architecture and contributor documentation | PR #50 mergeado; issues #21/#1 encerradas |
| 04 - dotnet new template package (issue #22) | Em implementacao na branch feat/issue-22-dotnet-new-template; PR e checks pendentes |

## Prompt 01

- Branch: `feat/idempotent-development-seed`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/33.
- Comando: `dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed`.
- Primeira execucao isolada: 5 mudancas Identity e 11 mudancas SampleRestaurant.
- Segunda execucao isolada: 0 mudancas Identity e 0 mudancas SampleRestaurant.
- Unit tests: 113 passed.
- Integration tests: 53 passed.
- OpenAPI: regenerado sem diff.
- Vulnerabilidades: nenhuma nas fontes atuais.
- Deprecated: `xunit 2.9.3` nos projetos de teste, fora do escopo desta issue.
- PR: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/34.
- Checks remotos: Build/test, CodeQL, Dependency Review e SonarCloud Quality Gate passaram.

## Prompt 02

- Branch: `refactor/normalize-encoding-and-naming`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/35.
- Nomes ativos corrigidos: `Intefaces` -> `Interfaces`, `Clains` -> `Claims`, `Loggin*` -> `LogEntry*`.
- Identificador legado preservado: tabela `Loggin`.
- OpenAPI: regenerado com mudancas textuais em descricoes 400/401/429.
- Unit tests: 124 passed after coverage remediation.
- Integration tests: 54 passed.
- Architecture tests: 8 passed explicitamente.
- EF pending model changes: sem alteracoes pendentes nos dois DbContexts.
- Vulnerabilidades: nenhuma nas fontes atuais.
- PR: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/36.
- Checks remotos: apos remediacao de cobertura, Build/test, CodeQL, Dependency Review e SonarCloud Quality Gate passaram.

## Prompt 03 - Bilingual Onboarding and Architecture Documentation

- Branch: `docs/issue-21-onboarding-architecture` (a partir de `main`).
- Issues: [#21](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/21) e [#1](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/1).
- PR: [#50](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/50), aberto e ainda sem merge.
- Escopo: README EN/PT-BR, diagramas e contratos arquiteturais, migrations EF Core, migracao do legado, CONTRIBUTING, SECURITY, CHANGELOG, Code of Conduct e tres ADRs.
- Gate adicionado: `python3 scripts/validate-docs.py` no CI (links locais, anchors e hierarquia de secoes entre idiomas).
- Feedback Codex [review #5461397982](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/50#pullrequestreview-5461397982):
  - `dotnet ef` usa factories que leem JSON e variaveis de ambiente, nao os User Secrets da API. Guia EN/PT-BR e READMEs agora exigem `ConnectionStrings__DefaultConnection` na sessao do terminal, sem mudar o codigo de producao.
  - Registros SDD de status, handoff e decisoes agora incluem esta entrega e o novo gate.
- Validacao do primeiro head do PR (`b1d76b9b1c`): gate de docs, build Release, unit tests, integration tests, OpenAPI, vulnerability audit, CodeQL e Dependency Review passaram. O job `SonarCloud Quality Gate` **falhou** apesar do build e testes passarem; o motivo exato do gate nao foi exposto no log do scanner. Os novos commits requerem revalidacao. Nao declarar Sonar aprovado antes de confirmacao.
- Proximo passo: corrigir qualquer gate remanescente e concluir review do PR; depois da incorporacao, seguir com #22 (`dotnet new`) e #23 (release v2.0.0).

## Prompt 04 - dotnet new Template Package

- Branch: `feat/issue-22-dotnet-new-template` (criada da `main` em 2026-10-08).
- Issue: [#22](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/22).
- Identidade: `RodriOliveira.WebApiCoreSeed.CSharp` / `webapi-seed`; pacote `RodriOliveira.WebApiCoreSeed.Templates`.
- Arquivos principais: `.template.config/template.json`, `template-pack/WebApiCoreSeed.Templates.csproj`, READMEs de template EN/PT-BR, `scripts/templates/smoke-test.sh` e `.github/workflows/template-smoke.yml`.
- Docs de distribuicao EN/PT-BR, README principal EN/PT-BR, CHANGELOG e validador de docs atualizados.
- Template substitui `WebApiCoreSeed` pelo `-n` solicitado e atribui `UserSecretsId` novo. Allowlist exclui arquivos SDD, CI e administracao do projeto de origem. Compose usa nome seguro com `COMPOSE_PROJECT_NAME` opcional; links de origem nao sofrem substituicao.
- Testes previstos no workflow: empacotar `.nupkg`, inspecionar conteudo, instalar localmente, gerar `SampleApi`, restore, build, testes unitarios e integrados com Docker, iniciar API e validar `/health/live`.
- Resultado dos checks: pendente da execucao no PR; nao relatar como aprovados sem evidencia.
- Proxima entrega apos merge: [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23), release v2.0.0 e eventual publicacao do pacote no feed.
