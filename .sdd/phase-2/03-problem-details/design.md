# Design - 03 Problem Details

## Pipeline

O pipeline passa a ter uma unica trilha central de excecao:

1. CORS por ambiente.
2. `UseExceptionHandler()`.
3. Serilog request logging.
4. `SerilogMiddleware`.
5. HSTS.
6. `UseStatusCodePages` escrevendo Problem Details.
7. Rate limiting legado temporario.
8. Middlewares de seguranca, compressao, MVC, Swagger e health checks.

Foram removidos os endpoints `/error` e `/error-local-development` como mecanismo de pipeline, assim como o `ErrorHandlingMiddleware`.

## Componentes

- `ApiProblemDetails`: helper central para tipos, titulos, detalhes seguros, `traceId`, `instance` e conversao para result HTTP.
- `FluentValidationExceptionHandler`: mapeia `FluentValidation.ValidationException` para 400 com `errors`.
- `PersistenceExceptionHandler`: mapeia `DbUpdateConcurrencyException` para 409 e `DbUpdateException` para 500 seguro.
- `UnhandledExceptionHandler`: registra excecoes inesperadas e retorna 500 seguro fora de Development.
- `ProblemDetailsResult`: result explicito com `application/problem+json`.

## Controllers e Filtros

- `MainController` V1/V2 agora retorna Problem Details para ModelState invalido, notificacoes de dominio, 404 e 400 de erro.
- `RequisitoClaimFilter` retorna 401/403 com Problem Details.
- Eventos JWT `OnChallenge` e `OnForbidden` escrevem Problem Details.
- `CustomResult` permanece para respostas de sucesso.

## Seguranca

- Stack trace nao e exposto fora de Development.
- Excecoes inesperadas nao retornam mensagem bruta em ambientes nao Development.
- Falhas de persistencia usam mensagem segura e nao retornam SQL, connection strings, tokens ou caminhos internos.
