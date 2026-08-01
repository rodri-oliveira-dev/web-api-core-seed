# Handoff - Phase 3

## Estado atual

- Branch atual: `phase/3-quality-and-safety`
- Branch-base: `phase/2-dotnet-10-migration`
- SHA inicial: `f35b72a2af01d46d07379d2b969b0e2f9c1c1196`
- Prompt atual: `01 - Unit test baseline`
- Issue atual: `#11`
- Status do prompt 01: concluido
- Commit de entrega: `test: strengthen existing unit test suite`

## Resultado do prompt 01

- Testes auditados: 34
- Testes finais: 36
- Falsos positivos corrigidos:
  - `ErroValidacaoAtualizarAtendente` chamava `Adicionar` em vez de `Atualizar`.
  - Testes de falha do `AtendenteService` afirmavam notificacao por mock de `TemNotificacao()`, sem observar `Handle`.
- Testes adicionados:
  - Remocao de atendente delega para o repositorio correto.
  - `PedidoPrato` aceita observacao opcional dentro do limite.
- Defeito produtivo corrigido:
  - `PedidoPratoValidation.Observacao` agora usa `MaximumLength(1000)` em vez de exigir texto nulo/branco.
- Convencoes adotadas:
  - Metodo de teste no formato `MetodoQuandoCondicaoDeveResultado`.
  - `DisplayName` em portugues com comportamento observavel.
  - `Notificador` real para validar notificacoes em services.
- Baseline de cobertura:
  - Geral: 29,15% linhas, 17,66% branches.
  - `Restaurante.IO.Api`: 34,38% linhas, 17,82% branches.
  - `Restaurante.IO.Business`: 53,05% linhas, 19,44% branches.
  - `Restaurante.IO.Data`: 0% linhas, 0% branches.
- Limitacoes:
  - Cobertura inclui migrations e codigo legado sem exclusoes deliberadas.
  - Testes HTTP existentes continuam no projeto `Pedidos.Test`, mas nao foram reescritos neste prompt.
  - Warnings de analyzer herdados permanecem fora do escopo.

## Regras para o proximo chat

- Ler `README.md`, `status.md`, `decisions.md`, `handoff.md` e a pasta do prompt antes de agir.
- Nao reescrever novamente a suite unitaria apos o prompt 01; trate-a como baseline confiavel e ajuste apenas regressao comprovada.
- Nao implementar Testcontainers, WebApplicationFactory novo, seguranca, OpenTelemetry, Aspire ou CI completo fora dos prompts correspondentes.
- Nao fazer push sem pedido explicito.

## Validacoes oficiais da fase

```text
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
git diff --check
```

## Proxima issue

`#12` - Integration tests baseline.
