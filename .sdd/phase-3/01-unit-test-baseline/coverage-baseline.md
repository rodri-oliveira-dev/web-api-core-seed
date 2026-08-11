# Coverage Baseline - 01 Unit Test Baseline

Status: concluido.

## Ferramenta

`coverlet.collector` via `XPlat Code Coverage`.

## Comando

```text
dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory TestResults/Coverage
```

Nao ha `coverlet.runsettings` neste repositorio no inicio ou fim do prompt.

## Resultado

| Escopo | Linha | Branch |
| --- | ---: | ---: |
| Geral | 29,15% | 17,66% |
| `Restaurante.IO.Api` | 34,38% | 17,82% |
| `Restaurante.IO.Business` | 53,05% | 19,44% |
| `Restaurante.IO.Data` | 0,00% | 0,00% |

## Areas criticas descobertas

- `Restaurante.IO.Data` nao tem cobertura nesta baseline; testes de integracao com dependencia real pertencem ao prompt/issue `#12`.
- Migrations e snapshots entram na metrica atual, reduzindo o valor de linha sem refletir diretamente qualidade comportamental.
- `Business` ainda tem services sem testes unitarios dedicados alem de `AtendenteService`.
- `Api` e coberta parcialmente pelos testes HTTP existentes, mas esses testes nao substituem uma estrategia de integracao mais estruturada.

## Arquivos deliberadamente excluidos

Nenhum arquivo foi excluido deliberadamente nesta entrega. A baseline usa a instrumentacao padrao disponivel.

## Observacao

Esta entrega nao define threshold arbitrario. A cobertura e uma baseline auxiliar, nao o indicador unico de qualidade.
