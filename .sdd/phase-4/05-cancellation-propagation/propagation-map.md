# Propagation Map - Prompt 05

## `GET /api/v1/Pratos`

```text
RequestAborted
-> PratosController.ObterLista
-> ObterPratos
-> IPratoService.Paginacao
-> PratoService.Paginacao
-> IPratoRepository.ListarPagina
-> PratoRepository.ListarPagina
-> EF Core ToListAsync
```

Tambem:

```text
RequestAborted
-> PratosController.ObterLista
-> ObterPratos
-> IPratoService.TotalRegistros
-> PratoService.TotalRegistros
-> IPratoRepository.Contar
-> PratoRepository.Contar
-> EF Core CountAsync
```

Cache:

```text
RequestAborted
-> CachedAttribute.OnActionExecutionAsync
-> IResponseCacheService.GetCachedResponseAsync / CacheResponseAsync
-> IDistributedCache.GetStringAsync / SetStringAsync
-> Redis provider
```

## `GET /api/v1/Pratos/{id}`

```text
RequestAborted
-> PratosController.ObterPorId
-> ObterPrato
-> IPratoService.ObterPorId
-> PratoService.ObterPorId
-> IPratoRepository.ObterPorId
-> PratoRepository.ObterPorId
-> EF Core FindAsync
```

## `POST /api/v1/Pratos`

```text
RequestAborted
-> PratosController.Adicionar
-> IPratoService.Adicionar
-> PratoService.Adicionar
-> IPratoRepository.ExisteComId
-> PratoRepository.ExisteComId
-> EF Core AnyAsync
-> IPratoRepository.Adicionar
-> ISampleRestaurantUnitOfWork.CommitAsync
-> SampleRestaurantDbContext.SaveChangesAsync
-> EF Core SaveChangesAsync
```

## `PUT /api/v1/Pratos/{id}`

```text
RequestAborted
-> PratosController.Atualizar
-> ObterPrato
-> IPratoService.ObterPorId
-> PratoRepository.ObterPorId
-> EF Core FindAsync
-> IPratoService.Atualizar
-> PratoService.Atualizar
-> IPratoRepository.Atualizar
-> ISampleRestaurantUnitOfWork.CommitAsync
-> EF Core SaveChangesAsync
```

## `DELETE /api/v1/Pratos/{id}`

```text
RequestAborted
-> PratosController.Excluir
-> ObterPrato
-> IPratoService.ObterPorId
-> PratoRepository.ObterPorId
-> EF Core FindAsync
-> IPratoService.Remover
-> PratoService.Remover
-> IPratoRepository.RemoverPorId
-> ISampleRestaurantUnitOfWork.CommitAsync
-> EF Core SaveChangesAsync
```

## `GET /api/v1/Mesas/{id}`

```text
RequestAborted
-> MesasController.ObterPorId
-> ObterMesa
-> IMesaService.ObterPorId
-> MesaService.ObterPorId
-> IMesaRepository.ObterPorId
-> MesaRepository.ObterPorId
-> EF Core FindAsync
```

## `POST|PUT|DELETE /api/v1/Mesas`

```text
RequestAborted
-> MesasController
-> IMesaService
-> MesaService
-> IMesaRepository
-> ISampleRestaurantUnitOfWork.CommitAsync
-> SampleRestaurantDbContext.SaveChangesAsync
-> EF Core SaveChangesAsync
```

`PUT` e `DELETE` tambem consultam a mesa antes da escrita via `FindAsync`.

## Fluxos sem suporte direto a token

- `AuthController` v1/v2 usa `UserManager` e `SignInManager`; as chamadas usadas (`CreateAsync`, `SignInAsync`, `PasswordSignInAsync`, `FindByEmailAsync`, `GetClaimsAsync`, `GetRolesAsync`) nao expõem `CancellationToken`.
- Upload de arquivo em `PratosController.UploadArquivo` e sincrono e ficou fora de escopo.
