# Build Design

## Directory.Build.props

Defaults centralizados:

```xml
<TargetFramework>net10.0</TargetFramework>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<AnalysisLevel>latest-recommended</AnalysisLevel>
<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
<Deterministic>true</Deterministic>
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

## Propriedades nao centralizadas

Nao foram definidas globalmente:

- `IsPackable=false`
- `OutputType`
- `UserSecretsId`
- `AspNetCoreHostingModel`
- propriedades exclusivas da API
- propriedades exclusivas de testes

## Nullable

Nullable foi habilitado na raiz e os warnings `CS*` foram corrigidos sem `NoWarn` amplo.

Principais tipos de ajuste:

- contratos `T?` para buscas que podem nao encontrar entidade;
- inicializadores `string.Empty` em DTOs/configuracoes;
- validacao explicita de configuracoes obrigatorias;
- propriedades opcionais preservadas onde o snapshot EF ja era nullable;
- `null!` isolado em navegacoes EF e campos obrigatorios materializados/preenchidos por frameworks.

## EF Core

Nullable annotations alteram convencoes do EF. Por isso foram preservadas explicitamente as propriedades opcionais historicas (`LogginEntity.Escopo`, `PedidoPrato.Observacao`) para evitar pending model changes e migration artificial.
