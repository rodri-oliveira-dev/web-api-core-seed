# Report

## Entrega

Central Package Management hierarquico adotado na solution `WebApiCoreSeed.sln`.

## Arquivos principais

- `Directory.Packages.props`
- `src/Directory.Packages.props`
- `tests/Directory.Packages.props`
- `tools/Directory.Packages.props`
- `packages.lock.json` por projeto ativo

## Mudancas

- `ManagePackageVersionsCentrally` habilitado na raiz.
- `RestorePackagesWithLockFile` habilitado para gerar locks reproduziveis.
- `PackageReference` dos `.csproj` ficou sem `Version`.
- `PrivateAssets` e `IncludeAssets` foram preservados nos projetos.
- Pacotes produtivos foram centralizados em `src`.
- Pacotes de teste foram centralizados em `tests`.
- Pacotes compartilhados por `tests` e `tools` ficaram na raiz.
- `tools/Directory.Packages.props` importa a raiz e nao declara pacotes exclusivos porque o tooling atual nao tem pacote direto exclusivo.

## Conflitos resolvidos

- `coverlet.collector`: alinhado em `10.0.1`.
- `Microsoft.NET.Test.Sdk`: alinhado em `18.8.1`.
- `xunit.runner.visualstudio`: alinhado em `3.1.5`.

## Riscos restantes

- `xunit` `2.9.3` esta deprecated como `Legacy`; migracao para `xunit.v3` requer prompt proprio por ser major e potencialmente impactar codigo/test runner.
- `OpenTelemetry.Instrumentation.EntityFrameworkCore` segue em `1.17.0-beta.1`; o comando de outdated reporta `Nao encontrado nas fontes`.
- `RestoreLockedMode` ainda nao foi ligado; a decisao ficou para o Prompt 4, junto da padronizacao de build/estilo.

## Validacao

- Restore, build e testes passaram.
- Nenhum `PackageReference` com `Version`.
- Nenhuma vulnerabilidade reportada.
- MSBuild preprocessado confirmou os imports esperados de `Directory.Packages.props`.
