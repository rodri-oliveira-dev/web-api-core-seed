# Discovery - Prompt 04

## Comandos executados no inicio

```bash
git status
git branch --show-current
git log -3 --oneline
dotnet build --configuration Release
dotnet test --configuration Release --no-build
git grep -n "SaveChanges"
git grep -n "SaveChangesAsync"
git grep -n "BeginTransaction"
git grep -n "TransactionScope"
git grep -n "CommitAsync"
git grep -n "Rollback"
```

## Resultado inicial

- Branch: `phase/4-architecture-modernization`.
- Working tree inicial: limpa.
- Ultimos commits:
  - `d861a72 refactor: replace generic repository with explicit ports`
  - `874f39e refactor: separate sample domain from reusable seed`
  - `27abd76 refactor: adopt modular hexagonal architecture`
- Build baseline: passou com warnings de analyzers ja conhecidos.
- Test baseline: passou, 48 testes em `WebApiCoreSeed.Tests` e 26 testes em `WebApiCoreSeed.IntegrationTests`.

## DbContexts

- `SampleRestaurantDbContext`: contexto do dominio demonstrativo, no projeto `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- `ApplicationDbContext`: contexto de Identity, no projeto `WebApiCoreSeed.Api`, usado por ASP.NET Core Identity.

Os contextos sao independentes. Nao ha caso de uso ativo que precise gravar nos dois contextos em uma unica operacao de negocio.

## SaveChanges em src

- `SampleRestaurantDbContext.SaveChangesAsync`: override autorizado para aplicar `DataCadastro` antes de delegar ao EF Core.
- Repositorios concretos chamavam `SampleRestaurantDbContext.SaveChangesAsync` diretamente:
  - `AtendenteRepository`: 3 chamadas.
  - `MesaRepository`: 3 chamadas.
  - `PedidoRepository`: 3 chamadas.
  - `PedidoPratoRepository`: 3 chamadas.
  - `PratoRepository`: 3 chamadas.
  - `LogginRepository`: 1 chamada.

## Transacoes

- Nao ha `BeginTransaction` em `src`.
- Nao ha `TransactionScope` em `src` ou `test`.
- `CommitAsync` e `Rollback` aparecem apenas em infraestrutura/testes de integracao:
  - `DatabaseReset` usa transacao para limpeza deterministica.
  - `SqlServerIntegrationTests` usa transacao representativa para validar rollback do provider.

## Domain events e interceptors

- Nao foram encontrados `DomainEvent`, `IDomainEvent`, interceptors EF Core ou `SaveChangesInterceptor` no codigo ativo.
- Portanto, nao ha publicacao de eventos acoplada ao commit nesta entrega.

## Problemas encontrados

- Repositorios de escrita confirmam imediatamente e impedem que o caso de uso coordene varias alteracoes.
- O retorno `Task<int>` das operacoes de repository representa linhas afetadas de `SaveChanges`, nao a intencao de registrar alteracao.
- Caso uma escrita futura use dois repositorios no mesmo caso de uso, o estado atual permitiria persistencia parcial se a primeira chamada salvasse e a segunda falhasse.
- Commit duplicado por request pode surgir facilmente quando mais de uma operacao de repository for chamada no mesmo caso de uso.

