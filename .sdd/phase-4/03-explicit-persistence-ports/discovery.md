# Discovery - Prompt 03

## Comandos executados

```text
git status
git branch --show-current
git log -3 --oneline
dotnet build --configuration Release
dotnet test --configuration Release --no-build
git grep -n "IRepository"
git grep -n "Repository" com token generico legado
git grep -n "Expression<Func"
git grep -n "SaveChanges"
git grep -n "Console.WriteLine"
git grep -n "catch (Exception"
git grep -n "return null"
```

## Baseline

- Branch: `phase/4-architecture-modernization`.
- Working tree inicial: limpa.
- Ultimos commits:
  - `874f39e refactor: separate sample domain from reusable seed`
  - `27abd76 refactor: adopt modular hexagonal architecture`
  - `18af517 ci: add quality and security workflows`
- `dotnet test --configuration Release --no-build`: passou, 47 testes em `WebApiCoreSeed.Tests` e 26 testes em `WebApiCoreSeed.IntegrationTests`.
- `dotnet build --configuration Release`: primeira execucao falhou por lock operacional de DLL porque foi executada em paralelo com `dotnet test`; o arquivo estava bloqueado por `testhost`. Repetir sequencialmente na validacao final.

## Achados

- A interface generica legada declarava CRUD generico, `Buscar(Expression<Func<TEntity, bool>> predicate)`, paginacao generica, contagem generica e `SaveChanges`.
- A implementacao generica legada espelhava `DbSet<TEntity>` e salvava automaticamente em `Adicionar`, `Atualizar` e `Remover`.
- O metodo generico legado de consulta por id engolia excecoes e retornava `null`.
- O metodo generico legado de adicao capturava `Exception`, escrevia em `Console.WriteLine` e relancava.
- Nenhum consumidor produtivo usa `Buscar` ou `ObterTodos`.
- `IPedidoRepository.ObterPedidoItens` existe, mas nao ha consumidor produtivo encontrado.
- `PratoService` usa `ObterPorId`, `Adicionar`, `Atualizar`, `Remover`, `Paginacao` e `TotalRegistros`.
- `MesaService` usa `ObterPorId`, `Adicionar`, `Atualizar` e `Remover`.
- `AtendenteService`, `PedidoService` e `PedidoPratoService` usam apenas escrita simples.
- `LogginService` usa apenas adicionar log.

## Riscos

- O contrato de commit ainda esta dentro de cada metodo de repositorio e sera tratado no Prompt 4.
- A remocao do repositorio base pode causar duplicacao pequena e aceitavel enquanto o Unit of Work nao existir.
- A paginacao ainda nao possui ordenacao deterministica; isso fica para o Prompt 7.
