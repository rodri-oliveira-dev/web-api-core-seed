# Ignored Files Matrix

| Padrao | Arquivo afetado | Motivo | Manter | Remover | Risco |
| --- | --- | --- | --- | --- | --- |
| `**/[Bb]in/` | `src/*/bin/`, `tests/*/bin/`, `tools/*/bin/` | Output de build local. | Sim | Nao | Baixo; arquivos sao regeneraveis. |
| `**/[Oo]bj/` | `src/*/obj/`, `tests/*/obj/`, `tools/*/obj/` | Output intermediario de build/restore. | Sim | Nao | Baixo; arquivos sao regeneraveis. |
| `TestResults/` | `TestResults/` | Resultado local/CI de testes. | Sim | Nao | Baixo; artefatos devem vir do CI. |
| `coverage/` | `coverage/` | Cobertura local. | Sim | Nao | Baixo; relatorios sao regeneraveis. |
| `coverage-report/` | `coverage-report/` | Relatorio gerado. | Sim | Nao | Baixo. |
| `artifacts/` | `artifacts/` | Saidas locais de build/publicacao. | Sim | Nao | Baixo. |
| `*.binlog` | Logs MSBuild | Logs grandes e locais. | Sim | Nao | Baixo. |
| `*.user`, `*.suo` | Preferencias de IDE | Estado individual do desenvolvedor. | Sim | Nao | Baixo. |
| `.env`, `.env.*` | Secrets locais | Evitar vazamento de configuracao sensivel. | Sim | Nao | Medio se removido. |
| `!.env.example`, `!.env.local.example` | Exemplos de ambiente | Exemplos podem ser versionados. | Sim | Nao | Baixo. |
| `.dotnet/`, `.dotnet-home/` | SDK/runtime local | Cache local pesado. | Sim | Nao | Baixo. |
| `.nuget/` | Cache local NuGet | Cache local pesado/sensivel ao ambiente. | Sim | Nao | Baixo. |
| `StrykerOutput/` | Relatorio mutation testing | Saida local de ferramenta. | Sim | Nao | Baixo. |
| `**/Properties/launchSettings.json` | Futuro launch settings da API | Regra antiga bloqueava DX compartilhada. | Nao | Sim | Baixo; arquivo nao existe hoje. |
| `*.http` | Requisicoes HTTP versionaveis | Artefatos uteis de desenvolvimento/API. | Nao | Sim | Medio se ignorado. |
| `docs/openapi/` | Contratos OpenAPI | Baselines versionadas do repositorio. | Nao | Sim | Alto se ignorado. |
| `.globalconfig` | Configuracao de analyzers | Pode ser necessaria no Prompt 6. | Nao | Sim | Medio se ignorado. |
| `Directory.Build.*` | Configuracao MSBuild compartilhada | Necessaria para build. | Nao | Sim | Alto se ignorado. |
| `Directory.Packages.props` | Futuro CPM | Necessario no Prompt 3. | Nao | Sim | Alto se ignorado. |
