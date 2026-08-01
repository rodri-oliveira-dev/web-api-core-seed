# Validation - 01 Unit Test Baseline

Status: concluido.

## Validacoes executadas

| Comando | Resultado |
| --- | --- |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --filter FullyQualifiedName~AtendenteServiceTest` | Passou: 5 testes |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --filter FullyQualifiedName~PedidoPratoValidationTest` antes da correcao produtiva | Falhou como esperado: observacao curta era rejeitada |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --filter FullyQualifiedName~PedidoPratoValidationTest` apos correcao | Passou: 4 testes |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --filter FullyQualifiedName~Pedidos.Test.Unitarios` | Passou: 23 testes |
| `dotnet test --configuration Release` | Passou: 36 testes |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory TestResults/Coverage` | Passou: 36 testes e gerou Cobertura XML |
| `dotnet restore` | Passou |
| `dotnet build --configuration Release --no-restore` | Passou com 40 warnings herdados |
| `dotnet test --configuration Release --no-build` | Passou: 36 testes |
| `git grep -n "Fact(Skip\\|Theory(Skip" -- test src .sdd` | Nenhuma ocorrencia |
| `git grep -n "DateTime.Now" -- test` | Nenhuma ocorrencia |
| `git diff --check` | Passou |

## Checagens de sensibilidade

- Mutacao temporaria em `AtendenteService.Atualizar` para chamar `Adicionar` fez `AtualizarQuandoAtendenteValidoDeveAtualizarAtendente` falhar com mock estrito.
- Mutacao temporaria em `PedidoPratoValidation.Observacao` para `MaximumLength(5)` fez `PedidoPratoQuandoObservacaoDentroDoLimiteDevePassarValidacao` falhar.

## Observacoes

- Uma tentativa de rodar filtros de `dotnet test` em paralelo causou `CS2012` por concorrencia de escrita em artefato de build; os mesmos testes passaram sequencialmente.
- `TestResults/` e ignorado por `.gitignore`; o artefato de cobertura nao foi versionado.
- Nao foi adicionado nenhum `[Fact(Skip = ...)]` ou `[Theory(Skip = ...)]`.
