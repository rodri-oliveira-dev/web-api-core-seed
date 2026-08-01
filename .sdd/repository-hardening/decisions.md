# Decisions

| ID | Decisao | Status | Motivo |
| --- | --- | --- | --- |
| RH-D001 | Remover arquivos ativos de Sonar em vez de comenta-los. | Aceita | O criterio de aceite exige nenhuma referencia ativa remanescente. |
| RH-D002 | Manter `launchSettings.json` rastreavel quando existir. | Aceita | Configuracoes de launch compartilhadas sao uteis para DX e nao devem ser ignoradas globalmente. |
| RH-D003 | Criar `.gitattributes` com LF como padrao e CRLF apenas para batch/cmd. | Aceita | Reduz churn de fim de linha e preserva compatibilidade de scripts Windows. |
| RH-D004 | Criar `.dockerignore` conservador, preservando arquivos de restore/build. | Aceita | A futura conteinerizacao deve receber `.csproj`, `.props`, `.targets`, `global.json`, solution e codigo-fonte. |
| RH-D005 | Sanitizar caminhos pessoais em documentacao historica. | Aceita | O repositorio nao deve manter caminhos absolutos de maquina. |
