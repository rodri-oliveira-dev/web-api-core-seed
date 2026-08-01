# Async Operation Inventory - Prompt 05

| Metodo | Camada | Tipo de I/O | Token atual | Origem do token | Acao | Risco |
| --- | --- | --- | --- | --- | --- | --- |
| `PratosController.ObterLista` | API/controller | HTTP -> dominio + Redis cache | nenhum | ASP.NET Core action binding | Receber e propagar `CancellationToken` | Baixo; contrato HTTP nao muda |
| `PratosController.ObterPorId` | API/controller | HTTP -> dominio | nenhum | ASP.NET Core action binding | Receber e propagar token | Baixo |
| `PratosController.Adicionar` | API/controller | HTTP -> dominio -> EF commit | nenhum | ASP.NET Core action binding | Receber e propagar token | Medio; upload de arquivo continua sincrono e fora de escopo |
| `PratosController.Atualizar` | API/controller | HTTP -> dominio -> EF query/commit | nenhum | ASP.NET Core action binding | Receber e propagar token | Medio |
| `PratosController.Excluir` | API/controller | HTTP -> dominio -> EF query/commit | nenhum | ASP.NET Core action binding | Receber e propagar token | Medio |
| `MesasController.ObterPorId` | API/controller | HTTP -> dominio -> EF query | nenhum | ASP.NET Core action binding | Receber e propagar token | Baixo |
| `MesasController.Adicionar` | API/controller | HTTP -> dominio -> EF commit | nenhum | ASP.NET Core action binding | Receber e propagar token | Medio |
| `MesasController.Atualizar` | API/controller | HTTP -> dominio -> EF query/commit | nenhum | ASP.NET Core action binding | Receber e propagar token | Medio |
| `MesasController.Excluir` | API/controller | HTTP -> dominio -> EF query/commit | nenhum | ASP.NET Core action binding | Receber e propagar token | Medio |
| `CachedAttribute.OnActionExecutionAsync` | API/filter | Redis | nenhum | `HttpContext.RequestAborted` | Passar token ao cache service | Baixo |
| `ResponseCacheService.CacheResponseAsync` | API/service | Redis via `IDistributedCache` | nenhum | Caller | Adicionar token explicito | Baixo |
| `ResponseCacheService.GetCachedResponseAsync` | API/service | Redis via `IDistributedCache` | nenhum | Caller | Adicionar token explicito | Baixo |
| `PratoService.*` | Application/use case | Repositorio + commit | nenhum | Controller/test caller | Adicionar token e propagar | Medio; altera mocks/testes |
| `MesaService.*` | Application/use case | Repositorio + commit | nenhum | Controller/test caller | Adicionar token e propagar | Medio |
| `AtendenteService.*` | Application/use case | Repositorio + commit | nenhum | Caller interno/testes | Adicionar token explicito opcional | Baixo |
| `PedidoService.*` | Application/use case | Repositorio + commit | nenhum | Caller interno/testes | Adicionar token explicito opcional | Baixo |
| `PedidoPratoService.*` | Application/use case | Repositorio + commit | nenhum | Caller interno/testes | Adicionar token explicito opcional | Baixo |
| `LogginService.Adicionar` | Application/use case | Repositorio + commit | nenhum | Caller interno/testes | Adicionar token explicito opcional | Baixo |
| `PratoRepository.ObterPorId` | Infrastructure/repository | EF Core query | nenhum | Service | Usar `FindAsync([id], token)` | Baixo |
| `PratoRepository.ExisteComId` | Infrastructure/repository | EF Core query | nenhum | Service | Usar `AnyAsync(..., token)` | Baixo |
| `PratoRepository.ListarPagina` | Infrastructure/repository | EF Core query | nenhum | Service | Usar `ToListAsync(token)` | Baixo |
| `PratoRepository.Contar` | Infrastructure/repository | EF Core query | nenhum | Service | Usar `CountAsync(token)` | Baixo |
| `MesaRepository.ObterPorId` | Infrastructure/repository | EF Core query | nenhum | Service | Usar `FindAsync([id], token)` | Baixo |
| `SampleRestaurantUnitOfWork.CommitAsync` | Infrastructure/UoW | EF Core save | ja aceita token | Service | Manter e garantir propagacao | Baixo |
| `SampleRestaurantDbContext.SaveChangesAsync` | Infrastructure/EF | EF Core save | `new CancellationToken()` default | Unit of Work | Trocar para `default` | Baixo |
| `AuthController` v1/v2 | API/controller | Identity store | nenhum | ASP.NET Core action binding | Registrar limitacao; API usada nao aceita token | Baixo |
| `HttpClient` em testes/ferramenta OpenAPI | Test/tools | HTTP | nenhum | Test/tool caller | Usar token em novos testes e ferramenta | Baixo |
