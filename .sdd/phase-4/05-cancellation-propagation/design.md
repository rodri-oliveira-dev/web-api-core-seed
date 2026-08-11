# Design - Prompt 05

## Convencao

Usar:

```csharp
CancellationToken cancellationToken
```

O token deve ser o ultimo parametro.

## Obrigatorio, opcional e nao aplicavel

- Controllers HTTP: token obrigatorio na action, recebido por model binding do ASP.NET Core.
- Helpers privados de controller: token obrigatorio quando chamam operacao assincrona cancelavel.
- Portas internas de Application e Persistence: token explicito com default `default` para reduzir churn em callers que ainda nao sao HTTP.
- Implementacoes de services e repositories: token explicito com default `default`, propagado sem criar novos `CancellationTokenSource`.
- Unit of Work: manter `CommitAsync(CancellationToken cancellationToken = default)`.
- EF Core: passar token em `FindAsync`, `AnyAsync`, `ToListAsync`, `CountAsync` e `SaveChangesAsync`.
- Redis por `IDistributedCache`: passar token em `GetStringAsync` e `SetStringAsync`.
- Operacoes puramente em memoria/sincronas: token nao aplicavel.

## Cancelamento e erros

- Nao capturar `OperationCanceledException` para registrar como erro.
- `UnhandledExceptionHandler` retorna `false` para `OperationCanceledException`, evitando classificar como erro 500 inesperado.
- `SerilogMiddleware` nao registra `OperationCanceledException` como erro.
- Timeout existente por middleware de request limits continua separado de cancelamento cooperativo; nenhum timeout novo sera criado.

## Testes

- Unitario em service: token ja cancelado propagado ao commit e excecao de cancelamento preservada.
- Unitario em service: cancelamento antes do commit nao executa commit quando repository observa o token.
- Unitario em repository com EF InMemory: token cancelado chega ao EF.
- HTTP leve: request cancelada pelo `HttpClient` nao retorna 500 tratado pela aplicacao.
- Integracao SQL Server: Unit of Work com token ja cancelado nao persiste.
