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
| RH-D013 | Centralizar defaults .NET 10 em `Directory.Build.props`. | Aceita | `TargetFramework`, `Nullable`, `ImplicitUsings`, `AnalysisLevel`, `EnforceCodeStyleInBuild`, `Deterministic` e XML docs sao comuns aos projetos ativos. |
| RH-D014 | Nao criar `Directory.Build.targets` no Prompt 4. | Aceita | A discovery nao encontrou target tardio real; props, `.editorconfig` e analyzers nativos cobrem o cenario atual. |
| RH-D015 | Manter `CA2007` silencioso para codigo ASP.NET Core. | Aceita | Exigir `ConfigureAwait` no pipeline de request adicionaria ruido sem beneficio pratico neste repositorio. |
| RH-D016 | Tratar `CS1591` apenas em `.editorconfig`. | Aceita | Evita supressao simultanea por `NoWarn` em projetos e preserva uma decisao unica para XML docs. |
| RH-D017 | Adotar SLNX como arquivo de solution ativo. | Aceita | O SDK 10 suporta o formato, a migracao oficial preservou projetos/folders e os gates passaram usando o novo arquivo. |
| RH-D018 | Bloquear a adocao de `CSF.Analyzers` ate existir fonte NuGet reproduzivel para os pacotes v2. | Aceita | `CSF.Analyzers.Architecture`, `CSF.Analyzers.Reliability` e `CSF.Analyzers.Testing` retornaram 404/sem resultados no NuGet.org, e o repositorio nao configura outro feed. |
| RH-D019 | Nao instalar `CSF.Analyzers.Testing` na primeira adocao. | Aceita | Os testes atuais usam Moq e nao adotam politica baseada em NSubstitute ou FluentAssertions. |
| RH-D020 | Renumerar `CSF.Analyzers` para Prompt 07 e inserir `Ambiente local com Docker e User Secrets` como Prompt 06. | Aceita | O prompt adicional define a nova sequencia obrigatoria; o historico do bloqueio de analyzers foi preservado em `.sdd/repository-hardening/07-csf-analyzers`. |
| RH-D021 | Remover Dockerfiles legados sob `docker/`. | Aceita | Eles continham senha fixa, tags flutuantes e health check invalido; a stack oficial passa a usar o root `Dockerfile` e imagens oficiais no Compose. |
| RH-D022 | Usar servico one-shot `migrations` em vez de aplicar migrations no startup da API. | Aceita | A API nao aplica migrations hoje; manter essa responsabilidade fora do startup evita concorrencia e falhas silenciosas. |
| RH-D023 | Nao adicionar `HEALTHCHECK` ao Dockerfile da API. | Aceita | A imagem ASP.NET final nao possui cliente HTTP; instalar `curl` ou `wget` aumentaria superficie apenas para health check. |
| RH-D024 | Publicar SQL Server e Redis no host por padrao no Compose local. | Aceita | O Compose tambem suporta o modo host `dotnet run`, que usa `localhost` para as dependencias. |
