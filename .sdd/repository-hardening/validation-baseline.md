# Validation Baseline

## Baseline inicial

| Comando | Resultado inicial |
| --- | --- |
| `git grep -n -i -E 'sonar|sonarqube|sonarcloud|sonarlint'` | Encontrou referencias historicas em `.sdd/` e artefatos ativos em `src/`. |
| `git grep` de caminhos pessoais de Windows, macOS e Linux | Encontrou caminhos historicos em `.sdd/` e caminho absoluto no batch removido. |
| `git check-ignore -v src/WebApiCoreSeed.Api/Properties/launchSettings.json` | O arquivo era ignorado por `.gitignore`. |
| `git ls-files` | Confirmou metadados operacionais, fontes, testes e artefatos ativos de analise em `src/`. |

## Baseline esperada apos Prompt 1

- Nenhuma ocorrencia Sonar fora de `.sdd/`.
- Nenhum caminho pessoal absoluto rastreado.
- `launchSettings.json` nao ignorado globalmente.
- Restore, build, testes e `git diff --check` sem regressao.

## Resultado do Prompt 1

Baseline confirmada. A busca por Sonar permanece apenas em registros `.sdd/`; a busca por caminhos pessoais absolutos nao retornou ocorrencias. Restore, build Release, testes Release e `git diff --check` passaram.
