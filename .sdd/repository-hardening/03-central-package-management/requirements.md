# Requirements

## Objetivo

Adotar Central Package Management hierarquico para todos os projetos ativos da solution `WebApiCoreSeed.sln`.

## Requisitos funcionais

- Habilitar `ManagePackageVersionsCentrally`.
- Remover `Version` dos `PackageReference` em todos os `.csproj`.
- Separar versoes por escopo:
  - raiz para configuracao NuGet comum, propriedades de versao compartilhadas e pacotes usados em mais de um escopo;
  - `src/` para pacotes produtivos;
  - `tests/` para SDK de teste, xUnit, Moq, Bogus, Coverlet, Testcontainers e auxiliares de teste;
  - `tools/` para pacotes exclusivos de tooling.
- Importar explicitamente o arquivo raiz em cada arquivo filho.
- Preservar metadados de pacote como `PrivateAssets` e `IncludeAssets`.
- Resolver divergencias antigas sem introduzir versoes flutuantes, wildcards, `VersionOverride` ou pinning transitivo sem necessidade.

## Requisitos de reproducibilidade

- Usar versoes fixas de pacote.
- Habilitar `RestorePackagesWithLockFile` para versionar `packages.lock.json` por projeto ativo.
- Validar `dotnet restore WebApiCoreSeed.sln --force-evaluate` sem `NU1008`.

## Fora de escopo

- Migrar xUnit 2 para xUnit v3.
- Atualizar majors de runtime/producao fora da consolidacao necessaria dos pacotes de teste.
- Alterar projetos, referencias entre projetos, codigo produtivo, testes ou pipelines.
