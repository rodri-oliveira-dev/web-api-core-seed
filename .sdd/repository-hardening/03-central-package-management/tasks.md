# Tasks

- [x] Executar inventario inicial com `dotnet list WebApiCoreSeed.sln package`.
- [x] Executar inventario de outdated, deprecated e vulnerable.
- [x] Classificar pacotes em raiz, `src`, `tests` e `tools`.
- [x] Criar `Directory.Packages.props` raiz.
- [x] Criar `src/Directory.Packages.props` com pacotes produtivos.
- [x] Criar `tests/Directory.Packages.props` com pacotes de teste.
- [x] Criar `tools/Directory.Packages.props` importando a raiz.
- [x] Remover `Version` de todos os `PackageReference`.
- [x] Preservar `PrivateAssets` e `IncludeAssets`.
- [x] Resolver divergencias de `Microsoft.NET.Test.Sdk`, `coverlet.collector` e `xunit.runner.visualstudio`.
- [x] Gerar lock files com `dotnet restore --force-evaluate`.
- [x] Validar restore, build, testes, greps, transitive package list e MSBuild preprocessado.
- [x] Atualizar `status.md`, `decisions.md`, `handoff.md` e relatorio do prompt.
