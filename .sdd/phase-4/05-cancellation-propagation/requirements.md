# Requirements - Prompt 05

## Objetivo

Propagar cancelamento cooperativo desde a borda HTTP ate as operacoes de I/O assincronas relevantes do repositorio.

Fluxo alvo:

```text
HttpContext.RequestAborted
-> Controller
-> porta de entrada
-> caso de uso
-> porta de saida
-> EF Core, Redis ou HTTP
```

## Criterios de aceite

- Controllers recebem `CancellationToken cancellationToken` fornecido pelo ASP.NET Core.
- Portas de entrada e saida internas expõem token explicito, preferencialmente como ultimo parametro.
- Casos de uso propagam o token para repositories e Unit of Work.
- Queries EF Core e `SaveChangesAsync` recebem o token.
- Redis recebe token nas chamadas por `IDistributedCache`.
- HttpClient recebe token em testes e ferramentas quando a chamada e cancelavel.
- `OperationCanceledException` nao e registrada como erro inesperado.
- Nao usar `CancellationToken.None` sem justificativa.
- Testes cobrem propagacao, token ja cancelado, cancelamento antes de commit e comportamento HTTP cancelado.
- Contratos HTTP e OpenAPI permanecem preservados.

## Fora de escopo

- Timeout global novo.
- `CancellationTokenSource` por camada produtiva.
- `Task.Run` ou `Task.Delay` em producao para simular assincronicidade.
- Alteracao de regras de negocio, payloads, rotas ou status codes de sucesso/erro existentes.
- Transacoes explicitas novas.
