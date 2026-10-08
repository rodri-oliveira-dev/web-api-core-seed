# Handoff - Phase 5

## Estado Atual

- Branch: `feat/idempotent-development-seed`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/33.
- Pull Request: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/34.
- Objetivo do prompt 01: adicionar seed de desenvolvimento explicito, seguro, deterministico e idempotente.

## Implementacao

- API continua como composition root.
- `Program.cs` roteia `--seed` para `DevelopmentSeedRunner` e nao inicia o pipeline HTTP neste modo.
- `DevelopmentSeedConfiguration` bloqueia `Production` e valida credenciais externas.
- `DevelopmentSeedIdentitySeeder` cria/atualiza usuario por `UserManager<IdentityUser>`.
- `DevelopmentSeedSampleRestaurantSeeder` faz upsert por GUIDs deterministicas no `SampleRestaurantDbContext`.
- Nao ha `EnsureCreated`, `HasData`, JWT emitido pelo seed ou senha real versionada.

## Validacao Local

- Restore locked, build, unit tests, integration tests, seed isolado duas vezes, bloqueio em Production, OpenAPI, vulnerabilidades, deprecated e `git diff --check` foram executados.
- Resultado completo em `.sdd/phase-5/01-development-seed/validation.md`.

## Delivery

- Commit semantico criado e enviado.
- PR aberto para `main` com `Closes #33`.
- Checks remotos passaram: Build/test, CodeQL, Dependency Review e SonarCloud Quality Gate.
- Merge nao realizado.

## Prompt 02 - Encoding And Naming

- Branch: `refactor/normalize-encoding-and-naming`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/35.
- Objetivo: normalizar codificacao UTF-8 e nomenclatura ativa sem quebrar contratos HTTP, OpenAPI, schema legado ou migrations historicas.
- Nomes ativos corrigidos:
  - `WebApiCoreSeed.SampleRestaurant.Intefaces` -> `WebApiCoreSeed.SampleRestaurant.Interfaces`.
  - `WebApiCoreSeed.Api.Extensions.Clains` -> `WebApiCoreSeed.Api.Extensions.Claims`.
  - `Loggin*` C# ativo -> `LogEntry*`.
- Persistencia:
  - `LogEntryMapping` preserva `builder.ToTable("Loggin")`.
  - `SampleRestaurantDbContextModelSnapshot` foi atualizado para o tipo ativo `LogEntry`.
  - Migrations historicas e designers foram preservados.
- OpenAPI:
  - Regenerado com alteracoes textuais apenas em descricoes.
- Validacao local:
  - Restore locked, build Release, unit tests, integration tests, architecture tests, migrations em banco vazio, upgrade legado, EF pending, OpenAPI JSON, vulnerabilidades e `git diff --check` passaram.
- PR: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/36.
- Checks remotos:
  - Primeira rodada passou build/test, CodeQL e Dependency Review.
  - SonarCloud Quality Gate falhou inicialmente por cobertura de codigo novo (`new_coverage` 66.0 abaixo do limiar 80).
  - Foram adicionados testes focados para `LogEntryValidation`, `LogEntryService`, `LogEntryRepository` e textos normalizados de Problem Details antes do novo push.
  - Rodada final passou Build/test, CodeQL, Dependency Review e SonarCloud Quality Gate.

## Prompt 03 - Documentacao Bilingue e Onboarding (PR #50)

- Branch: `docs/issue-21-onboarding-architecture`; base `main`.
- Issues: [#21](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/21) e [#1](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/1), referenciadas como `Closes` no PR.
- Objetivo: tornar a aplicacao real .NET 10 executavel a partir de um checkout limpo, em ingles e PT-BR; responder diretamente a pergunta da issue #1.
- Entregue: README EN/PT-BR, diagramas Mermaid, arquitetura modular/hexagonal com limites pragmaticos, guias de migrations e migracao legada EN/PT-BR, politica de contribuicao, seguranca, changelog, codigo de conduta e ADRs 0001-0003.
- Mudanca de CI: `python3 scripts/validate-docs.py` verifica referencias internas e paridade de hierarquia de titulos entre os pares de idiomas. Foi validado no job de build inicial do PR.
- Correcao da review #5461397982: as factories design-time de Identity e SampleRestaurant usam arquivos JSON e `AddEnvironmentVariables` (nao User Secrets). Ambos os guias de EF e os READMEs explicam a variavel `ConnectionStrings__DefaultConnection`, com comandos Bash/PowerShell para leitura interativa e limpeza posterior.
- Codigo funcional, banco, migrations e `v1.0.0-legacy` nao alterados pela entrega documental.
- Estado dos checks no head original do PR: build/testes/documentacao/OpenAPI/CodeQL/Dependency Review aprovados; SonarCloud Quality Gate **reprovado**. Registrar o resultado do ultimo head antes do merge; nao afirmar validacao que ainda esteja pendente.
- Pendencias posteriores: issue #22 (`dotnet new`) e #23 (v2.0.0); release e empacotamento nao feitos. O PR #50 permanece aberto, sem merge.
- Detalhes das etapas da entrega em [03-onboarding-architecture/README.md](03-onboarding-architecture/README.md).

## Prompt 04 - Template dotnet new (issue #22)

- Origem: `main` apos merge do PR #50.
- Branch: `feat/issue-22-dotnet-new-template`.
- Escopo: geracao/empacotamento de template de solucao .NET 10; nenhuma alteracao de comportamento na API de origem.
- Comandos para teste no repositorio: `dotnet pack template-pack/WebApiCoreSeed.Templates.csproj -c Release -o ./artifacts/templates`, `dotnet new install ./artifacts/templates/RodriOliveira.WebApiCoreSeed.Templates.2.0.0-preview.1.nupkg`, `dotnet new webapi-seed -n SampleApi`.
- A validacao formal de ponta a ponta e `bash scripts/templates/smoke-test.sh` (precisa do SDK .NET e Docker), executada no GitHub Actions `template-smoke.yml`.
- O pacote usa lista explicita dos arquivos reutilizaveis, conta com READMEs gerados em dois idiomas e evita levar dados internos de CI/SDD.
- `sourceName` altera nomes/paths/namespaces e `userSecretsId` gera GUID novo. O Compose mantem nome padrao seguro com override `COMPOSE_PROJECT_NAME`; eliminou-se a substituicao global de slug, pois alterava links de proveniencia e produzia caracteres invalidos para nomes com ponto. A regressao valida o nome `Acme.Api` no Compose e links originais em ambos os READMEs.
- Flags opcionais de remocao de componentes adiadas ate haver testes das combinacoes; Redis e OpenTelemetry podem ser configurados em runtime.
- Revisar os resultados reais do CI, smoke, SonarCloud e CodeQL antes do merge. O PR incluira `Closes #22` e nao sera mergeado automaticamente.
- Publicacao externa e v2.0.0 continuam na issue #23.

## Prompt 05 - Release v2.0.0 e encerramento do roadmap

- Issue #23; [PR #52](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/52) aberto sobre a branch `release/v2.0.0-productization`, criada da main contendo merges #49/#50/#51.
- Preparacao de release em `docs/releases/v2.0.0.md`, `docs/releases/release-checklist.md` e versao PT-BR, changelog e READMEs.
- Pacote passa a `RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg`, sem publicar no NuGet.org.
- Script de smoke aceita `TEMPLATE_PACKAGE` pre-empacotado e testa o arquivo da release (proveniencia, nomes `SampleApi`/`Acme.Api`, Docker Compose, restore, build, 2 suites de testes, health).
- Workflow `.github/workflows/release-v2.yml` somente publica na `main` apos CI, CodeQL e template-smoke **success** no mesmo SHA; anexa pacote e `SHA256SUMS.txt` na GitHub Release e verifica download e checksum.
- Tag `v2.0.0` nao e movimentada nem sobrescrita pelo processo em reexecucoes ou novos pushes. Restricao de imutabilidade nas Settings requer administracao.
- Revisao #5461961414 (PR #52): introduzido classificador de release via REST com paginação e estados `missing`/`published`/`draft`/`invalid`. Draft com assets interrompidos falha antes da idempotencia; verificação final exige publicação efetiva antes de baixar assets e fechar #23.
- Revisao #5462068123 (PR #52): privilegios de escrita removidos do job de empacotamento e smoke, que agora roda read-only separado do job de publicação; actions fixadas por SHA; 404 de consulta da tag tratado especificamente; prazo global de gates com falha imediata para conclusoes nao aprovadas; SHA esperado entregue por env, sem interpolacao shell. Run #37834123837 mostrou teste unitario OpenTelemetry flutuante de span ASP.NET Core (139 de 140 testes aprovados); estabilizado na suite gerada. Testes de regressão do classificador fazem parte do CI.
- Nao usar `Closes #23` no PR porque isso encerraria a issue antes da publicacao. O workflow fecha a #23 apos assets verificados, com comentario e link.
- Verificar logs da release e estado da issue apos merge; se workflow falhar, issue permanece aberta para correcao.
- Pendencias fora do workflow: escolher licenca (repo nao tem `LICENSE`) e atualizar descricao, homepage e topics no GitHub About; sem token administrativo disponivel.
- Checagens anteriores da `main` apos PR #51: template-smoke, CI incluindo SonarCloud e CodeQL **success**. O PR novo deve passar checks proprios antes do merge.
- Status deste handoff e **pre-publicacao**, nao equivale a uma GitHub Release ja concluida.
