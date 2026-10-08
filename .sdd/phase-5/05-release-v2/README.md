# Phase 5 / Prompt 05 — Release v2.0.0

## 1. Specification

Issue [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23): publicar uma grande versao rastreavel e consumivel, preservando a tag legada, documentando breaking changes e validando o **artefato** da release, nao apenas o source code.

Critérios:
- GitHub tag/release `v2.0.0` apontando para SHA aprovado de `main`.
- NuGet `RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg` e `SHA256SUMS.txt` nos assets.
- Instalar e gerar `dotnet new webapi-seed -n SampleApi`, build, suites unit/integration e /health/live usando exato asset.
- Versionamento e migracao legada claros (netcoreapp3.1 vs net10.0).
- Sem merge automatico por este prompt e sem declaracao de release concluida antes de workflow remoto.

## 2. Discovery

- PRs #49/#50/#51 confirmados incorporados a main. O ultimo merge (#51) trouxe `dotnet new` e teve workflow `template-smoke` e checks `ci` (incluindo SonarCloud)/`codeql` aprovados em `f77d85d`.
- GitHub ainda nao possui GitHub Release nem `v2.0.0` no momento da preparacao.
- Pacote de template antes em `2.0.0-preview.1`. Projetos e SDK da solucao ativa miram net10.0.
- Descricao e topics do GitHub About ainda referenciam .NET Core 3.1. Nao existe `LICENSE` no repositorio.
- GitHub Actions `GITHUB_TOKEN` com scopes `contents`, `actions` e `issues` permite publicar a release, consultar checks e fechar issue; nao permite alterar metadados `Administration: write` por configuracao simples.
- Nao foi escolhido feed NuGet. O canal inicial e GitHub Release assets, com publicacao posterior opcional em feed por decisao do mantenedor.

## 3. Design

- Branch `release/v2.0.0-productization` da `main` contem release workflow, version bump, docs EN/PT-BR e smoke parametrizavel.
- `release-v2.yml` dispara ao fazer push em `main` e aceita `workflow_dispatch`; nunca roda em PR.
- Antes de publicar, exige conclusao success de `ci.yml`, `codeql.yml`, `template-smoke.yml` no **mesmo SHA**.
- Faz pack da versao 2.0.0, chama `TEMPLATE_PACKAGE=...` em `scripts/templates/smoke-test.sh` e so entao publica.
- Release via `gh release create --target <SHA> --notes-file docs/releases/v2.0.0.md`. Download e SHA-256 validam os assets publicados.
- Tag nao pode ser movida. Ao receber pushes futuros, workflow encontra release existente e verifica assets sem criar nova tag.
- Issue #23 tem fechamento deliberadamente fora do PR para evitar fechamento antes da release. Workflow fecha apos confirmar integridade.
- Licenciamento, About/topics e branch/tag protection sao acoes administrativas documentadas, nao executadas com privilégio indisponível.

## 4. Development

- `template-pack/WebApiCoreSeed.Templates.csproj` alterado para `2.0.0`.
- `scripts/templates/smoke-test.sh` agora aceita pacote ja criado como fonte em vez de reempacotar.
- Adicionado `.github/workflows/release-v2.yml`.
- Notas completas em `docs/releases/v2.0.0.md`; checklists `release-checklist.md` e `release-checklist.pt-BR.md`.
- README e guias de distribuicao atualizados EN/PT-BR, badges de release/template e changelog 2.0.0.
- Estado, handoff e decisoes P5-D018..D021 atualizados.
- `scripts/validate-docs.py` cobre o checklist e as notas da versao.

## 5. Validation

### Concluido antes desta branch

- Merge #51 em `main`: `template-smoke` success (pacote preview, instalacao, geracao, build, testes e health); CI e SonarCloud success; CodeQL success.
- Conteudo da versao 2.0.0 ainda depende de novos checks no PR.

### Gates restantes

- PR de release: doc links/paridade, build, suites de testes, CodeQL, SonarCloud, `template-smoke` usando o pacote 2.0.0.
- Depois do merge: `release-v2` verifica todos os checks de main, valida pacote **exato**, publica e verifica SHA256 de assets baixados.
- Nao simular aprovacao do workflow antes de executa-lo. A release ainda nao existe enquanto esta branch nao for incorporada.

## 6. Delivery

- PR com `Refs #23` (nao `Closes #23`).
- Revisar a publicacao em [Releases](https://github.com/rodri-oliveira-dev/web-api-core-seed/releases) e no workflow [release-v2](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/release-v2.yml).
- Se falhar, o problema deve ser corrigido sem mover `v2.0.0`; a issue permanece aberta ate confirmar artefatos.
- Apos sucesso, o workflow fecha a #23 com link da release e SHA efetivamente publicado.
- Acoes do proprietario: atualizar description/topics/homepage, definir licenca e eventualmente proteger tags/ativar release immutability.
