# Property Inventory

## Comando inicial

```bash
git grep -n -E 'TargetFramework|Nullable|ImplicitUsings|GenerateDocumentationFile|NoWarn|IsPackable|AnalysisLevel' -- '*.csproj' '*.props' '*.targets'
```

## Inventario inicial

| Propriedade | Ocorrencias | Acao |
| --- | ---: | --- |
| `TargetFramework` | Todos os projetos ativos | Centralizar em `Directory.Build.props`. |
| `Nullable` | Raiz `disable`; alguns projetos `enable` | Centralizar `enable` e corrigir warnings nullable. |
| `ImplicitUsings` | Raiz e alguns projetos | Centralizar na raiz. |
| `AnalysisLevel` | Raiz | Manter na raiz. |
| `GenerateDocumentationFile` | Projetos `src`; API duplicada | Centralizar na raiz e remover duplicacoes. |
| `NoWarn` para `1591` | API | Remover; manter decisao unica em `.editorconfig`. |
| `IsPackable=false` | Testes e tool | Manter local, pois nao deve ser global. |

## Inventario final

```text
Directory.Build.props: TargetFramework, Nullable, ImplicitUsings, AnalysisLevel, GenerateDocumentationFile
tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj: IsPackable=false
tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj: IsPackable=false
tools/OpenApiGenerator/OpenApiGenerator.csproj: IsPackable=false
```

## Justificativa de duplicacoes remanescentes

`IsPackable=false` permanece somente em projetos que nao representam pacotes publicaveis: testes e ferramenta local. A propriedade nao foi definida globalmente porque projetos futuros podem ser empacotaveis.
