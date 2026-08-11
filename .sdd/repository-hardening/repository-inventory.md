# Repository Inventory

Inventario feito na branch `phase/4-architecture-modernization`.

## Metadados raiz

| Caminho | Estado antes | Estado depois | Observacao |
| --- | --- | --- | --- |
| `.gitignore` | Existia | Atualizado | Tinha regra global para `Properties/launchSettings.json`; agora preserva esse arquivo quando existir. |
| `.gitattributes` | Ausente | Criado | Define normalizacao, finais de linha e binarios. |
| `.dockerignore` | Ausente | Criado | Prepara contexto para conteinerizacao futura. |

## Arquivos operacionais versionados

| Area | Arquivos versionados | Observacao |
| --- | ---: | --- |
| `.vscode/` | 4 | Launch, tasks, settings e extensions. |
| `.github/` | 6 | CODEOWNERS, PR template, Dependabot e workflows CI/CodeQL/dependency-review. |
| `.githooks/` | 2 | README e pre-push shell. |
| `scripts/` | 2 | Setup de hooks em PowerShell e shell. |
| `docker/` | 3 | Imagens auxiliares de SQL Server, Redis e Seq; nao ha Dockerfile ativo da API. |
| `src/` | 148 antes da remocao | Incluia quatro artefatos ativos de Sonar removidos neste prompt. |
| `tests/` | 25 | Projeto unitario e projeto de integracao. |
| `tests/` | 0 | Pasta ausente. |
| `tools/` | 2 | OpenApiGenerator. |

## Docker e compose

| Padrao | Resultado |
| --- | --- |
| `Dockerfile*` | Nenhum arquivo versionado na raiz. |
| `docker/*` | Tres dockerfiles auxiliares versionados. |
| `compose*.yml` | Nenhum arquivo versionado. |
| `compose*.yaml` | Nenhum arquivo versionado. |

## Achados relevantes

- `git check-ignore -v src/WebApiCoreSeed.Api/Properties/launchSettings.json` apontava a regra antiga `**/Properties/launchSettings.json`.
- `src/sonar-project.properties`, `src/sonar-push.bat` e `src/.scannerwork/*` eram artefatos ativos ou gerados da integracao removida.
- Caminhos pessoais em `.sdd/phase-1/` e `.sdd/phase-2/` foram substituidos por `<user-home>`.
