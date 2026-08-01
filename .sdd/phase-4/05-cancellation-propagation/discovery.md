# Discovery - Prompt 05

## Baseline

- `git status`: working tree limpa.
- `git branch --show-current`: `phase/4-architecture-modernization`.
- `git log -3 --oneline`:
  - `3b79535 refactor: define explicit unit of work boundary`
  - `d861a72 refactor: replace generic repository with explicit ports`
  - `874f39e refactor: separate sample domain from reusable seed`
- `dotnet build --configuration Release`: passou com 31 warnings preexistentes de analyzers.
- `dotnet test --configuration Release --no-build`: passou, 49 testes em `WebApiCoreSeed.Tests` e 31 em `WebApiCoreSeed.IntegrationTests`.

## Greps executados

```bash
git grep -n "async Task"
git grep -n "CancellationToken"
git grep -n "CancellationToken.None"
git grep -n "SaveChangesAsync"
git grep -n "ToListAsync"
git grep -n "FirstOrDefaultAsync"
git grep -n "SendAsync"
git grep -n "GetAsync"
git grep -n "PostAsync"
git grep -n "Task.Delay"
```

## Achados

- Controllers `PratosController` e `MesasController` nao recebem token nas actions nem helpers privados.
- Services/casos de uso de `SampleRestaurant` nao recebem token.
- Portas de entrada e saida de `SampleRestaurant` nao recebem token, exceto a Unit of Work criada no Prompt 04.
- `PratoRepository` usa `FindAsync`, `AnyAsync`, `ToListAsync` e `CountAsync` sem token.
- `MesaRepository` usa `FindAsync` sem token.
- Repositories de escrita (`Adicionar`, `Atualizar`, `RemoverPorId`, `Registrar`) apenas registram alteracoes no `DbContext` e retornam `Task.CompletedTask`; o I/O real ocorre no commit.
- `SampleRestaurantDbContext.SaveChangesAsync` usa `new CancellationToken()` como default.
- `ResponseCacheService` usa `IDistributedCache.SetStringAsync` e `GetStringAsync` sem token.
- `CachedAttribute` nao passa `HttpContext.RequestAborted` para o cache.
- `SerilogMiddleware` registra qualquer excecao propagada como erro 500; isso inclui cancelamento se a excecao atravessar o middleware.
- `UnhandledExceptionHandler` registra qualquer excecao como erro inesperado; precisa deixar cancelamento sem tratamento ali.
- APIs de `UserManager` e `SignInManager` usadas nos controllers de auth nao oferecem token nas assinaturas atuais.
- Nao ha `FirstOrDefaultAsync` no codigo ativo.
- `CancellationToken.None` nao existe no baseline.
- `Task.Delay` aparece apenas em testes/fixtures de integracao.

## Fire-and-forget

Nenhuma chamada produtiva fire-and-forget foi encontrada nos fluxos avaliados.
