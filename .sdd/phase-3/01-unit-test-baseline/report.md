# Report - 01 Unit Test Baseline

## Resumo

O prompt 01 auditou os 34 testes existentes no projeto `Pedidos.Test`, corrigiu falsos positivos na suite unitaria, estabilizou dados de teste e registrou uma baseline de cobertura. A suite final tem 36 testes: 23 unitarios e 13 HTTP existentes.

## Problemas corrigidos

- `ErroValidacaoAtualizarAtendente` executava `Adicionar` em vez de `Atualizar`.
- Testes de erro do `AtendenteService` validavam notificacao por mock preconfigurado, sem comprovar que o service notificou.
- `PedidoValidationTest` dependia de `DateTime.Now.Second` para dados que nao tinham relacao com tempo.
- `PedidoPratoValidationTest` misturava ausencia de `Prato` com limite de `Observacao`.
- `NotificadorTest` verificava apenas quantidade, nao o conteudo da notificacao.

## Defeito produtivo encontrado

`PedidoPratoValidation.Observacao` rejeitava qualquer texto preenchido porque usava `Must(c => string.IsNullOrWhiteSpace(c))` antes de `Length(1000)`. O teste novo com `"Sem cebola"` falhou antes da correcao. A regra agora usa `MaximumLength(1000)`, preservando observacao opcional e rejeitando textos acima do limite.

## Testes adicionados

- `AtendenteServiceTest.RemoverQuandoIdInformadoDeveRemoverAtendente`
- `PedidoPratoValidationTest.PedidoPratoQuandoObservacaoDentroDoLimiteDevePassarValidacao`

## Testes nao reescritos

Os 13 testes em `test/Pedidos.Test/Integracao/ProblemDetailsContractTests.cs` foram auditados e mantidos. Eles sao testes HTTP criados na Fase 2, nao testes unitarios, e devem ser tratados na issue `#12` apenas se houver motivo de integracao.

## Cobertura

- Geral: 29,15% linhas, 17,66% branches.
- `Restaurante.IO.Api`: 34,38% linhas, 17,82% branches.
- `Restaurante.IO.Business`: 53,05% linhas, 19,44% branches.
- `Restaurante.IO.Data`: 0% linhas, 0% branches.

## Validacao final

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou com warnings herdados.
- `dotnet test --configuration Release --no-build`: passou com 36 testes.
- `git diff --check`: passou.

## Proximo passo

Executar o Prompt 2 da Fase 3 para a issue `#12`, criando a baseline de testes de integracao sem reescrever novamente a suite unitaria.
