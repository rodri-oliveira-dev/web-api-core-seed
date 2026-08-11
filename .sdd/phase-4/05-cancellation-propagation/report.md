# Report - Prompt 05

## Resumo

O Prompt 05 propagou `CancellationToken` desde os controllers HTTP do sample ate portas de entrada, casos de uso, portas de saida, repositories, Unit of Work, EF Core e cache Redis.

## Convencao adotada

- Nome: `CancellationToken cancellationToken`.
- Posicao: ultimo parametro.
- Controllers HTTP recebem token obrigatorio por action binding do ASP.NET Core.
- Contratos internos de Application/Persistence usam token explicito com default `default` para preservar compatibilidade de callers nao HTTP.
- Operacoes sincronas de tracking no repository checam cancelamento ja solicitado antes de alterar o `ChangeTracker`.

## Fluxos atualizados

- `GET /api/v1/Pratos`: controller -> `IPratoService.Paginacao` / `TotalRegistros` -> `IPratoRepository.ListarPagina` / `Contar` -> EF Core `ToListAsync` / `CountAsync`.
- `GET /api/v1/Pratos/{id}`: controller -> `IPratoService.ObterPorId` -> `IPratoRepository.ObterPorId` -> EF Core `FindAsync`.
- `POST /api/v1/Pratos`: controller -> `IPratoService.Adicionar` -> `ExisteComId` / `Adicionar` -> Unit of Work -> EF Core `SaveChangesAsync`.
- `PUT /api/v1/Pratos/{id}`: controller -> consulta por id -> atualizacao -> Unit of Work -> EF Core.
- `DELETE /api/v1/Pratos/{id}`: controller -> consulta por id -> remocao -> Unit of Work -> EF Core.
- `GET /api/v1/Mesas/{id}`: controller -> `IMesaService.ObterPorId` -> `IMesaRepository.ObterPorId` -> EF Core `FindAsync`.
- `POST|PUT|DELETE /api/v1/Mesas`: controller -> `IMesaService` -> `IMesaRepository` -> Unit of Work -> EF Core.
- Cache de resposta: `CachedAttribute` -> `IResponseCacheService` -> `IDistributedCache` com `RequestAborted`.
- Health responses: serializacao JSON usa `HttpContext.RequestAborted`.
- OpenAPI generator: `HttpClient.GetAsync` e `CopyToAsync` usam token cancelado por Ctrl+C.

## Dependencias

- EF Core: `FindAsync`, `AnyAsync`, `ToListAsync`, `CountAsync`, `SaveChangesAsync`.
- Redis: `IDistributedCache.GetStringAsync` e `SetStringAsync`.
- HttpClient: `tools/OpenApiGenerator`.
- ASP.NET Core: action binding de `CancellationToken`, `HttpContext.RequestAborted`, `IExceptionHandler`.

## APIs sem suporte direto a token

- `UserManager` e `SignInManager` nas actions de auth usam metodos que nao expõem `CancellationToken` nas assinaturas atuais.
- Upload de arquivo em `PratosController.UploadArquivo` permanece sincrono e fora de escopo.
- Chamadas diretas de `DbContext.SaveChangesAsync()` em testes permanecem sem token quando fazem seed/setup e nao representam fluxo HTTP cancelado.

## Comportamento de cancelamento

- Services interrompem cedo quando recebem token ja cancelado.
- Cancelamento em repository ou commit propaga `OperationCanceledException`.
- Commit cancelado nao persiste alteracoes.
- `SerilogMiddleware` nao registra `OperationCanceledException` como erro 500.
- `UnhandledExceptionHandler` nao transforma `OperationCanceledException` em Problem Details 500.

## Testes

- `AtendenteServiceTest`:
  - token ja cancelado nao chama dependencias;
  - cancelamento do commit propaga;
  - cancelamento no repository nao executa commit.
- `ProblemDetailsContractTests`:
  - request HTTP cancelada durante operacao controlada nao retorna 500.
  - fake repository confirma que o token recebido pelo adaptador e cancelavel.
- `SqlServerIntegrationTests`:
  - Unit of Work com commit cancelado nao persiste alteracao.

## Validacao

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou.
- `dotnet test --configuration Release --no-build`: passou, 53 + 32 testes.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration`: passou, 32 testes.
- OpenAPI v1/v2 regenerado sem diff de contrato.
- `CancellationToken.None`: nenhuma ocorrencia.

## Delivery

- Commit semantico planejado: `refactor: propagate cancellation tokens`.
- Push: nao realizado.
- Proximo prompt: Prompt 6 - Migrations na infraestrutura.
- Proxima issue registrada para continuidade conforme prompt: `#18`.
