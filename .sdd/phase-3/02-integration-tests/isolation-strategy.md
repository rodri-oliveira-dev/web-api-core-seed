# Isolation Strategy - 02 Integration Tests

## Ciclo de vida

- Um par de containers SQL Server/Redis e compartilhado pela collection `api-integration`.
- Containers sobem uma vez por execucao do projeto de integracao.
- Containers sao descartados no `DisposeAsync` da fixture.

## Isolamento

- Todos os testes pertencem a mesma collection xUnit, evitando paralelismo dentro da suite de integracao.
- Cada teste chama `ResetStateAsync()` antes do cenario.
- SQL e limpo por `DELETE` em ordem de dependencias, preservando `__EFMigrationsHistory`.
- Redis e limpo por `FLUSHDB` no database do container de teste.

## Migrations e seed

- Migrations sao aplicadas uma vez, antes dos testes.
- Nao e usado `EnsureCreated`.
- Seeds sao minimos e criados por teste.
- Dados usam nomes deterministicos ou `Guid` para evitar colisao.

## Descarte

- `HttpClient` e descartado por teste.
- Contexts sao resolvidos em escopo e descartados ao final do helper.
- Multiplexer Redis e host da API sao descartados antes dos containers.
- Variaveis de ambiente alteradas pela fixture sao restauradas no teardown.
