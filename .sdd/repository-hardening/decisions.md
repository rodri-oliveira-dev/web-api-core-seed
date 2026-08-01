# Decisions

| ID | Decisao | Status | Motivo |
| --- | --- | --- | --- |
| RH-D001 | Remover arquivos ativos de Sonar em vez de comenta-los. | Aceita | O criterio de aceite exige nenhuma referencia ativa remanescente. |
| RH-D002 | Manter `launchSettings.json` rastreavel quando existir. | Aceita | Configuracoes de launch compartilhadas sao uteis para DX e nao devem ser ignoradas globalmente. |
| RH-D003 | Criar `.gitattributes` com LF como padrao e CRLF apenas para batch/cmd. | Aceita | Reduz churn de fim de linha e preserva compatibilidade de scripts Windows. |
| RH-D004 | Criar `.dockerignore` conservador, preservando arquivos de restore/build. | Aceita | A futura conteinerizacao deve receber `.csproj`, `.props`, `.targets`, `global.json`, solution e codigo-fonte. |
| RH-D005 | Sanitizar caminhos pessoais em documentacao historica. | Aceita | O repositorio nao deve manter caminhos absolutos de maquina. |
| RH-D006 | Agrupar projetos de modulo sob `src/Modules` preservando os assemblies atuais. | Aceita | A mudanca melhora descobribilidade fisica sem alterar limites logicos, contratos ou schema. |
| RH-D007 | Renomear `WebApiCoreSeed.Tests` para `WebApiCoreSeed.UnitTests`. | Aceita | O repositorio ja possui testes de integracao separados; o nome antigo era ambiguo. |
| RH-D008 | Manter `tools/OpenApiGenerator` sem renomear. | Aceita | O projeto ja esta em tooling dedicado e a renomeacao nao agregaria valor alem de simetria. |
| RH-D009 | Adotar Central Package Management hierarquico com raiz, `src/`, `tests/` e `tools/`. | Aceita | O prompt exige separacao por escopo sem filhos isolados que escondam a raiz. |
| RH-D010 | Versionar `packages.lock.json` por projeto ativo. | Aceita | `RestorePackagesWithLockFile` preserva restore reproduzivel apos centralizar versoes. |
| RH-D011 | Alinhar divergencias de tooling de teste para as versoes ja usadas mais recentes no repositorio. | Aceita | `coverlet.collector`, `Microsoft.NET.Test.Sdk` e `xunit.runner.visualstudio` divergiam entre unitarios e integracao; restore, build e testes validaram o alinhamento. |
| RH-D012 | Manter xUnit 2.x apesar do aviso `Legacy`. | Aceita | Migrar para `xunit.v3` e major e deve ser tratado em prompt proprio, nao como efeito colateral do CPM. |
