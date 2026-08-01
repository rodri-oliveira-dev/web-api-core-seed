# Validation

## Comandos executados

| Comando | Resultado |
| --- | --- |
| `git grep -n -i -E 'sonar|sonarqube|sonarcloud|sonarlint'` | Retornou apenas registros historicos em `.sdd/`; nenhum artefato ativo fora de `.sdd/`. |
| `git grep` de caminhos pessoais de Windows, macOS e Linux | Sem ocorrencias. |
| `git check-attr --all -- .gitattributes .gitignore AGENTS.md` | Confirmou `text` e `eol=lf` para os tres arquivos. |
| `git status --ignored` | Mostrou apenas alteracoes deste prompt e arquivos locais ignorados como `bin/`, `obj/`, `TestResults/`, logs e caches. |
| `dotnet restore WebApiCoreSeed.sln` | Passou; todos os projetos estavam atualizados para restauracao. |
| `dotnet build WebApiCoreSeed.sln --configuration Release --no-restore` | Passou com 31 avisos CA existentes e 0 erros. |
| `dotnet test WebApiCoreSeed.sln --configuration Release --no-build` | Passou: 53 testes unitarios e 42 testes de integracao. |
| `git diff --check` | Passou sem problemas de whitespace. |

## Observacoes

- `git check-ignore -v src/WebApiCoreSeed.Api/Properties/launchSettings.json` agora aponta a excecao `!**/Properties/launchSettings.json`, permitindo versionamento futuro.
- Os avisos CA do build pertencem ao codigo existente e nao foram alterados neste prompt.
