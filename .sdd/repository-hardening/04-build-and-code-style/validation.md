# Validation

## Comandos executados

```bash
dotnet format WebApiCoreSeed.sln --verify-no-changes
dotnet restore WebApiCoreSeed.sln
dotnet build WebApiCoreSeed.sln --configuration Release --no-restore --no-incremental
dotnet test WebApiCoreSeed.sln --configuration Release --no-build
```

## Resultado

| Comando | Resultado |
| --- | --- |
| `dotnet format WebApiCoreSeed.sln --verify-no-changes` | Falhou por divida historica de formatacao/encoding. |
| `dotnet restore WebApiCoreSeed.sln` | Sucesso. |
| `dotnet build WebApiCoreSeed.sln --configuration Release --no-restore --no-incremental` | Sucesso, 30 warnings `CA*`, 0 warnings `CS*`. |
| `dotnet test WebApiCoreSeed.sln --configuration Release --no-build` | Sucesso, 95 testes aprovados. |

## dotnet format

Execucao final resumida:

| Categoria | Qtde |
| --- | ---: |
| `ENDOFLINE` | 1923 |
| `FINALNEWLINE` | 17 |
| `CHARSET` | 11 |
| `WHITESPACE` | 4 |
| `CA1848` | 1 |
| `CA2254` | 1 |

Nao foi executada formatacao automatica da solution inteira para evitar churn fora do escopo. Foram corrigidos apenas whitespaces em arquivos tocados pela tarefa.

## Propriedades duplicadas

Inventario final confirmou que `TargetFramework`, `Nullable`, `ImplicitUsings`, `AnalysisLevel` e `GenerateDocumentationFile` estao centralizados. `IsPackable=false` permanece local somente em testes e tool.
