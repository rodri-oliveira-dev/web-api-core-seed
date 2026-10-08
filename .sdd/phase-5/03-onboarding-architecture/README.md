# Prompt 03 — Bilingual Onboarding and Architecture Documentation

## 1. Specification

- Issues: [#21](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/21) (documentacao) e [#1](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/1) (API, testes, migrations, seed).
- Branch: `docs/issue-21-onboarding-architecture` criada a partir da `main`.
- Objetivo: permitir onboarding da aplicacao real .NET 10 em ingles e PT-BR; documentar arquitetura, operacao, configuracao, contribuicao, seguranca e evolucao do legado.
- Restricoes: nenhuma alteracao de runtime/contratos de API, schemas, migrations ou tags historicas; nao antecipar as issues #22 e #23.

## 2. Discovery

- Fonte de verdade: `WebApiCoreSeed.slnx`, `global.json` (SDK 10.0.401), `compose.yaml`, `Program.cs`, `HostingConfig.cs`, factories dos DbContexts, `DevelopmentSeedRunner.cs`, configuracoes da API, testes e workflows CI.
- O README antigo citava VS2019 e arquitetura em camadas/repository generico, nao refletindo a separacao modular/hexagonal.
- O Compose aplica migrations Identity e SampleRestaurant, e o modo `--seed` explicito aplica migrations e dados deterministicamente fora de Production.
- `ApplicationDbContextFactory` e `SampleRestaurantDbContextFactory` carregam JSON e `AddEnvironmentVariables()`; ambas **nao leem User Secrets**. A variavel `ConnectionStrings__DefaultConnection` e necessaria para comandos `dotnet ef`, mesmo quando o runtime ja funciona com User Secrets.

## 3. Design

- `README.md` em ingles e `README.pt-BR.md` em portugues, com seletor de idioma e hierarquia de titulos equivalente.
- Guias especificos EN/PT-BR para arquitetura, migrations e comparativo de migracao legada; diagramas Mermaid e referencia aos limites implementados.
- Documentos sociais: `CONTRIBUTING.md`, `SECURITY.md`, `CHANGELOG.md`, `CODE_OF_CONDUCT.md`.
- ADRs aceitos: portas explicitas/modulos, contextos/migrations/seed e cache/observabilidade; nao representam novas funcionalidades criadas por este PR.
- Gate CI: `python3 scripts/validate-docs.py` checa links locais, anchors e paridade da estrutura dos documentos bilingues.
- As factories EF existentes permanecem intactas; variavel local temporaria e leitura interativa nao imprimem valores sensiveis no historico de comandos.

## 4. Development

- PR [#50](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/50) inclui o pacote documental e o validador do CI.
- Comentario de onboarding publicado na issue #1 com comandos atuais.
- Revisao [#5461397982](https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/50#pullrequestreview-5461397982) identificou lacuna de design-time EF e ausencia de SDD. Ambos os pontos foram incorporados nos guias e no SDD da fase.

## 5. Validation

- Primeiro head `b1d76b9b1c`: validador de documentacao, build Release, testes unitarios, integracao, OpenAPI, auditoria de dependencias, CodeQL e Dependency Review **aprovados** no CI.
- `SonarCloud Quality Gate` no primeiro head **falhou**, apesar de build e testes desse job estarem aprovados; o log informa apenas `QUALITY GATE STATUS: FAILED`. Nao presumir aprovacao nem inferir uma condicao especifica sem acesso as metricas do Sonar.
- Confirmar os mesmos checks depois dos ajustes da revisao, e registrar o resultado no PR/status da fase. Validacao local nao foi reproduzida por CLI neste atendimento; checks remotos sao a evidencia.
- A documentacao de `dotnet ef` foi verificada estaticamente contra ambas as factories de design-time, sem executar operacao de migration ou revelar connection string.

## 6. Delivery and handoff

- Atualizacoes versionadas na branch `docs/issue-21-onboarding-architecture`.
- PR #50 em revisao; sem merge.
- Referencias cruzadas: [decisions.md](../decisions.md), [status.md](../status.md), [handoff.md](../handoff.md).
- Depois do merge (e somente se o PR permanecer com `Closes #21` e `Closes #1`), o GitHub encerra as issues.
- Proximas entregas planejadas: #22 (template `dotnet new`) e #23 (release v2.0.0).
