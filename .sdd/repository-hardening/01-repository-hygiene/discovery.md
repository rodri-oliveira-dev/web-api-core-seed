# Discovery

## Comandos executados

```bash
git grep -n -i -E 'sonar|sonarqube|sonarcloud|sonarlint'
git grep -n -E '<machine-home-path-patterns>'
git ls-files
git check-ignore -v src/WebApiCoreSeed.Api/Properties/launchSettings.json
```

## Achados

- `.gitattributes` e `.dockerignore` nao existiam.
- `.gitignore` era derivado de template antigo do Visual Studio e ignorava `**/Properties/launchSettings.json`.
- `src/WebApiCoreSeed.Api/Properties/launchSettings.json` nao existe hoje, mas a regra global impediria versionamento futuro.
- `src/sonar-project.properties`, `src/sonar-push.bat`, `src/.scannerwork/.sonar_lock` e `src/.scannerwork/report-task.txt` estavam versionados.
- Referencias Sonar em `.sdd/` sao historicas ou registros de decisao de fases anteriores.
- Caminhos pessoais absolutos em `.sdd/phase-1/` e `.sdd/phase-2/` foram encontrados em logs historicos.
- Nao ha pasta `tests/` versionada.
- Nao ha `Dockerfile*` ou `compose*.yml|yaml` versionado na raiz.

## Template de referencia

O `.gitignore` foi adaptado a partir do template oficial atual de Visual Studio mantido em `github/gitignore`.
