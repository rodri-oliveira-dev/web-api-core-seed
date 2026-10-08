# Status - Phase 5

| Prompt | Status |
| --- | --- |
| 01 - Development seed deterministico e idempotente | PR #34 mergeado; checks remotos passaram |
| 02 - Normalize UTF-8 encoding and active code naming | PR #36 mergeado; checks remotos passaram |
| 03 - Bilingual onboarding, architecture and contributor documentation | PR #50 mergeado; issues #21/#1 encerradas |
| 04 - dotnet new template package (issue #22) | PR #51 mergeado; issue #22 encerrada; CI/template-smoke/CodeQL na main aprovados |

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

## Prompt 05 - GitHub Release v2.0.0 (#23)

- Base: `main` apos os merges #49, #50 e #51; todos confirmados como merged. Branch: `release/v2.0.0-productization`.
- Issue: [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23).
- Runtime/toolchain: .NET 10; distribuicao escolhida: **GitHub Release assets** (sem feed NuGet.org).
- Versao do pacote alterada de `2.0.0-preview.1` para **`2.0.0`**, alinhando tag, nome do asset, notas e instalacao.
- `scripts/templates/smoke-test.sh` aceita `TEMPLATE_PACKAGE` para validar o asset exato sem repack.
- `release-v2.yml` executa em push/dispatch na main, aguarda CI/CodeQL/template-smoke **no mesmo commit**, empacota o NuGet estavel, valida API gerada, testes e health, calcula SHA-256, cria `v2.0.0`/GitHub Release, baixa os assets e confere checksum/tag.
- Revisao [#5462068123](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/52#pullrequestreview-5462068123): jobs separados por privilegio, actions SHA-pin, erros HTTP da consulta de tag, timeout global e gate nao-sucesso, interpolacao shell protegida e documentacao de dispatch corrigida. Run 37834123837: test de span de servidor intermitente (139/140 aprovados), ajuste deterministico no teste de observabilidade, aguardando novo smoke.
- Revisao [#5461961414](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/52#pullrequestreview-5461961414): release em draft com assets enviados nao pode ser confundida com publicada. Preflight agora diferencia estados e falha fechado; gate final exige metadata de publicacao antes de fechar #23. Testes automatizados em `scripts/releases/test-release-state.sh` no CI.
- Reentradas nao movem tag; outras revisoes de main nao tentam republicar v2.0.0. A issue sera fechada **pelo workflow apenas apos a verificacao**; o PR nao deve conter `Closes #23` para evitar fechamento prematuro.
- Documentos: notas completas de breaking changes, changelog v2, checklists EN/PT-BR, badge de release, guias de template estavel.
- Limites nao resolvidos por este PR: metadados "About"/topics requerem token administrativo; repo ainda sem LICENSE. Nao declarar como licenciado open source sem decisao/procedencia.
- **Status atual:** [PR #52](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/52) aberto sem conflitos; checks remotos em execucao, aguardando revisao/merge e execucao automatica da release em main. Nenhuma tag v2.0.0 ou GitHub Release foi criada nesta preparacao.
