# Discovery - 03 Problem Details

## Comandos Executados

- `git grep -n "Exception"`
- `git grep -n "ErrorHandlingMiddleware"`
- `git grep -n "UseExceptionHandler"`
- `git grep -n "CustomResult"`
- `git grep -n "DomainNotification"`
- `git grep -n "ModelState"`
- `git grep -n "BadRequest"`
- `git grep -n "NotFound"`
- `git grep -n "StatusCode"`

## Mecanismos Encontrados

- `HostingConfig.UseApiPipeline` usava `UseExceptionHandler("/error-local-development")` em Development e `UseExceptionHandler("/error")` nos demais ambientes.
- `ErrorController` atendia `/error-local-development`, `/error` e `/error/{id}`; o endpoint local expunha `StackTrace` em Development.
- `ErrorHandlingMiddleware` capturava qualquer `Exception`, registrava log e escrevia `CustomResult` com `application/json`.
- `UseStatusCodePages` escrevia `CustomResult` com `statusCode` e `errorMessage`.
- `MainController` V1/V2 retornava `CustomResult` para 400 e 404.
- `ModelState` tinha filtro automatico suprimido e era tratado manualmente em controllers.
- `INotificador`/`Notificador` continua sendo o mecanismo de notificacao de dominio; nao existe tipo chamado `DomainNotification`.
- `RequisitoClaimFilter` retornava `CustomUnauthorizedResult` e `CustomForbiddenResult`.
- `JwtBearer` nao customizava payload de challenge/forbidden.
- Repositories podem relancar falhas de persistencia, especialmente `DbUpdateException`; alguns pontos tambem engolem excecao e retornam `null`, como `Repository.ObterPorId`.

## Observacoes

- `CustomResult` tambem embrulha respostas de sucesso; por isso nao foi removido nesta tarefa.
- `CustomNoContentResult` permanece em sucesso 204, mas deixou de depender de wrapper de erro.
- A dependencia real de SQL Server continua afetando smoke de `/hc` fora da factory de teste.
- `AspNetCoreRateLimit` continua temporario e sera tratado na proxima issue.
