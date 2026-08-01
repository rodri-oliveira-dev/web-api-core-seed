# Discovery - 01 Unit Test Baseline

## Contexto lido

- `AGENTS.md`
- `.sdd/phase-2/README.md`
- `.sdd/phase-2/status.md`
- `.sdd/phase-2/decisions.md`
- `.sdd/phase-2/handoff.md`
- `RestauranteAPI.sln`
- `Directory.Build.props`
- `.editorconfig`
- `.github/workflows/dependency-review.yml`
- `test/Pedidos.Test/Pedidos.Test.csproj`
- Todos os arquivos em `test/Pedidos.Test/`
- Validators e services de Business relacionados aos testes unitarios

## Skills usadas

- `repository-governance-sdd`
- `dotnet-service-change`
- `test-anti-patterns`

`coverage-analysis` foi citada no prompt, mas nao esta instalada na lista de skills disponivel neste ambiente.

## Verificacoes iniciais

| Comando | Resultado |
| --- | --- |
| `git status` | Working tree limpa em `phase/3-quality-and-safety` |
| `git branch --show-current` | `phase/3-quality-and-safety` |
| `git log -5 --oneline` | Topo inicial `f35b72a refactor: modernize OpenAPI and API versioning` |
| `git rev-parse HEAD` | `f35b72a2af01d46d07379d2b969b0e2f9c1c1196` |
| `dotnet --info` | SDK `10.0.302`, runtimes `10.0.10` |
| `dotnet test --configuration Release` | Passou: 34 testes |
| `dotnet test test/Pedidos.Test/Pedidos.Test.csproj --configuration Release` | Passou: 34 testes |

## Projetos e configuracoes

- Projeto de teste ativo: `test/Pedidos.Test/Pedidos.Test.csproj`.
- Target framework do teste: `net10.0`.
- Pacotes de teste: xUnit 2.9.3, runner 3.1.5, Moq 4.20.72, FluentValidation.TestHelper via projeto Business, `Microsoft.AspNetCore.Mvc.Testing`, EF Core InMemory e `coverlet.collector`.
- Nao ha `Directory.Packages.props`.
- Nao ha `*.runsettings` ou `coverlet.runsettings`.
- Workflow ativo existente: apenas `dependency-review.yml`; nao ha workflow completo de teste nesta entrega.

## Buscas obrigatorias

| Busca | Resultado |
| --- | --- |
| `git grep -n "[Fact]"` | 34 ocorrencias: 21 unitarias e 13 HTTP em `Integracao` |
| `git grep -n "[Theory]"` | Nenhuma ocorrencia |
| `git grep -n "Verify("` | 4 ocorrencias em `AtendenteServiceTest` |
| `git grep -n "Throws"` | Nenhuma ocorrencia |
| `git grep -n "DateTime.Now"` | 2 ocorrencias em `PedidoValidationTest`; demais ocorrencias em producao |
| `git grep -n "DateTime.UtcNow"` | 1 ocorrencia em teste HTTP para expirar JWT; demais em producao |
| `git grep -n "Task.Delay"` | Nenhuma ocorrencia |
| `git grep -n "Thread.Sleep"` | Nenhuma ocorrencia |

## Achados

- `ErroValidacaoAtualizarAtendente` declara erro na atualizacao, mas chama `Adicionar`, confirmando o problema suspeito.
- Testes de falha do `AtendenteService` usam mock de `TemNotificacao()` para afirmar notificacao, mas o service nunca chama esse metodo; isso pode mascarar ausencia de notificacao real.
- `PedidoValidationTest` usa `DateTime.Now.Second` para criar numeros de pedido e mesa; a regra nao depende de tempo, entao o dado deve ser deterministico.
- `PedidoPratoValidation` parece conter defeito produtivo pequeno: a regra de `Observacao` exige string nula/branca e depois aplica `Length(1000)`, o que reprova qualquer observacao preenchida, apesar da mensagem e do teste existente indicarem limite maximo.
- Os testes HTTP em `Integracao` nao sao unitarios, mas tem asserts comportamentais relevantes e pertencem a regressao da Fase 2.
- Nao foram encontrados sleeps, delays, exceptions engolidas ou dependencia de ordem.
