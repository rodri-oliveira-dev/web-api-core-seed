# Diagnostics Baseline

## Baseline antes da mudanca

Build Release inicial:

| ID | Qtde | Projeto principal | Causa | Acao proposta |
| --- | ---: | --- | --- | --- |
| CA1305 | 8 | API e SampleRestaurant | Cultura explicita ausente em conversoes/formatacao | Manter como divida tecnica fora deste prompt. |
| CA1309 | 1 | API | Comparacao de string nao ordinal | Corrigido ao ajustar autorizacao customizada. |
| CA1510 | 1 | API | Lancamento manual de `ArgumentNullException` | Manter como divida tecnica. |
| CA1816 | 6 | SampleRestaurant | `Dispose` sem `GC.SuppressFinalize` | Manter como divida tecnica. |
| CA1848 | 6 | API | Logging sem `LoggerMessage` | Manter como divida tecnica. |
| CA1854 | 1 | SampleRestaurant | `ContainsKey` + indexador | Manter como divida tecnica. |
| CA1859 | 1 | SampleRestaurant | Campo poderia usar tipo concreto | Manter como divida tecnica. |
| CA1860 | 1 | SampleRestaurant | `Any()` em colecao com `Count` | Manter como divida tecnica. |
| CA1861 | 2 | API | Arrays constantes em chamada repetida | Manter como divida tecnica. |
| CA1869 | 1 | API | `JsonSerializerOptions` recriado | Manter como divida tecnica. |
| CA1873 | 1 | API | Argumento de log avaliado antes do nivel | Manter como divida tecnica. |
| CA2254 | 2 | API | Template de log variavel | Manter como divida tecnica. |

Total inicial: 31 warnings.

## Nullable

`Nullable=enable` simulado antes das correcoes adicionava 101 warnings `CS*`:

| ID | Qtde |
| --- | ---: |
| CS8600 | 1 |
| CS8602 | 14 |
| CS8603 | 8 |
| CS8604 | 11 |
| CS8618 | 37 |
| CS8620 | 2 |
| CS8625 | 15 |
| CS8765 | 4 |

## Baseline final

Build Release final:

- 30 warnings.
- Todos `CA*`.
- Nenhum warning `CS*`.
- Nenhum warning novo em relacao ao baseline inicial.

O total caiu de 31 para 30 porque a comparacao de claims passou a usar `StringComparison.OrdinalIgnoreCase`, removendo `CA1309`.
