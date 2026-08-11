# Error Catalog - 03 Problem Details

| Origem | Tipo | Status atual | Payload atual | Status desejado | `type` | `title` | Extensoes | Impacto |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| ModelState em controllers | Validacao de entrada | 400 | `CustomResult { success, data: [...] }` | 400 | `urn:problem:validation` | `Validacao da requisicao falhou.` | `traceId`, `errors` por campo | Breaking change no envelope de erro. |
| Controller base `NaoEncontrado` | Entidade nao encontrada | 404 | `CustomResult { success: false, data: "Objeto nao foi encontrado" }` | 404 | `urn:problem:not-found` | `Recurso nao encontrado.` | `traceId` | Breaking change no envelope de erro. |
| `INotificador` | Regra de dominio | 400 | `CustomResult { success: false, data: [...] }` | 400 | `urn:problem:domain-rule` | `Regra de dominio violada.` | `traceId`, `errors.notifications` | Mensagens de dominio preservadas, envelope alterado. |
| `INotificador` com duplicidade conhecida | Conflito | 400 | `CustomResult { success: false, data: [...] }` | 409 | `urn:problem:conflict` | `Conflito com o estado atual do recurso.` | `traceId`, `errors.notifications` | Breaking change de status quando a notificacao indica recurso ja existente. |
| JWT challenge | Autenticacao | 401 sem corpo padronizado | Vazio ou payload de middleware/filtro | 401 | `urn:problem:authentication` | `Autenticacao necessaria.` | `traceId` | Consumidores passam a receber body Problem Details. |
| Claims filter | Autorizacao | 403 | `CustomResult { success: false, data: string }` | 403 | `urn:problem:authorization` | `Acesso negado.` | `traceId` | Breaking change no envelope de erro. |
| Rate limit legado | Limite de requisicoes | 429 | Controlado por `AspNetCoreRateLimit` | 429 | `urn:problem:rate-limit` | `Limite de requisicoes excedido.` | `traceId` | Catalogado para a proxima issue; implementacao nativa nao feita aqui. |
| Excecao nao tratada | Falha inesperada | 500 | `CustomResult` por middleware ou `/error` | 500 | `urn:problem:unexpected-error` | `Erro interno.` | `traceId` | Body deixa de expor detalhes brutos fora de Development. |
| EF Core `DbUpdateConcurrencyException` | Conflito de persistencia | 500 ou excecao bruta | `CustomResult` generico | 409 | `urn:problem:conflict` | `Conflito com o estado atual do recurso.` | `traceId` | Novo status deterministico. |
| EF Core `DbUpdateException` | Falha de persistencia | 500 | `CustomResult` generico | 500 | `urn:problem:persistence-failure` | `Falha de persistencia.` | `traceId` | Mensagem segura, sem SQL/connection string. |
| Status code pages | Rota/status sem body | 4xx/5xx | `CustomResult { statusCode, errorMessage }` | Mesmo status | `urn:problem:http-status-{status}` ou tipo conhecido | Titulo por status | `traceId` | Breaking change no envelope de erro. |
