# Design

## Estrutura CPM

```text
Directory.Packages.props
src/Directory.Packages.props
tests/Directory.Packages.props
tools/Directory.Packages.props
```

## Raiz

Responsabilidades:

- habilitar `ManagePackageVersionsCentrally`;
- habilitar `RestorePackagesWithLockFile`;
- habilitar `RestoreUseStaticGraphEvaluation`;
- manter propriedades de versao compartilhadas:
  - `MicrosoftDotNetPackageVersion`;
  - `OpenTelemetryPackageVersion`;
- declarar apenas `PackageVersion` de pacotes diretos usados por mais de um escopo:
  - `Microsoft.AspNetCore.Mvc.Testing`;
  - `Microsoft.EntityFrameworkCore.InMemory`.

## `src/`

Responsabilidades:

- importar explicitamente `..\Directory.Packages.props`;
- declarar pacotes produtivos da API e dos modulos;
- usar as propriedades comuns da raiz para pacotes Microsoft e OpenTelemetry alinhados.

## `tests/`

Responsabilidades:

- importar explicitamente `..\Directory.Packages.props`;
- declarar SDK de teste, xUnit, Moq, Bogus, Coverlet, Testcontainers e cliente Redis usado diretamente pelos testes de integracao;
- alinhar divergencias antigas de tooling de teste.

## `tools/`

Responsabilidades:

- importar explicitamente `..\Directory.Packages.props`;
- nao declarar pacotes exclusivos enquanto o `OpenApiGenerator` usa apenas pacotes ja compartilhados com `tests/`.

## Decisoes de versionamento

- Sem versoes flutuantes.
- Sem wildcards.
- Sem `VersionOverride`.
- Sem pinning transitivo.
- Sem `RestoreLockedMode` neste prompt; os lock files foram introduzidos, mas a politica de build/CI fica para o Prompt 4.
- `PrivateAssets` e `IncludeAssets` permanecem nos projetos consumidores, pois sao metadados de consumo e nao versao.
