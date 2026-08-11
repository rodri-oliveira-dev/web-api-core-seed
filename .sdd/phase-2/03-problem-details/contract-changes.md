# Contract Changes - 03 Problem Details

## Antes

Erros eram retornados em formatos diferentes:

- `CustomResult` com `success` e `data`.
- `CustomResult` com `statusCode` e `errorMessage` em status-code pages.
- Challenge JWT podia retornar sem body padronizado.
- Excecoes podiam passar por `UseExceptionHandler` com `ErrorController` ou por `ErrorHandlingMiddleware`.

Exemplo legado:

```json
{
  "success": false,
  "data": [
    "O campo Email e obrigatorio"
  ]
}
```

## Depois

Erros passam a seguir Problem Details:

```json
{
  "type": "urn:problem:validation",
  "title": "Validacao da requisicao falhou.",
  "status": 400,
  "detail": "Corrija os campos indicados e tente novamente.",
  "instance": "/api/v1/entrar",
  "traceId": "0H...",
  "errors": {
    "Email": [
      "O campo Email e obrigatorio"
    ]
  }
}
```

## Breaking Changes

- Envelope de erro muda de `CustomResult` para Problem Details.
- `Content-Type` dos erros passa para `application/problem+json`.
- Notificacao de duplicidade conhecida pode retornar 409 em vez de 400.
- 401 e 403 passam a retornar body padronizado.
- Endpoints legados `/error`, `/error-local-development` e `/error/{id}` foram removidos.

## Justificativa

A mudanca reduz duplicacao, usa o pipeline nativo do ASP.NET Core, evita vazamento de informacoes sensiveis e cria contrato deterministico para consumidores.

## Impacto para Consumidores

Consumidores que parseavam `success`/`data` em erros devem migrar para `type`, `title`, `status`, `detail`, `instance`, `traceId` e `errors`.

Respostas de sucesso nao foram alteradas.
