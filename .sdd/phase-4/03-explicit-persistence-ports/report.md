# Report - Prompt 03

## Resumo

O Prompt 03 removeu o repositorio generico legado do codigo ativo e substituiu a heranca generica por portas especificas de persistencia no modulo `SampleRestaurant`.

## Abstracoes removidas

- Interface generica legada de repository.
- Implementacao generica legada de repository.
- Consulta generica por predicado arbitrario.
- Consulta generica `ObterTodos`.
- `SaveChanges` exposto em porta de Application.
- `IPedidoRepository.ObterPedidoItens`, sem consumidor produtivo encontrado.

## Portas explicitas

- `IPratoRepository`: escrita, consulta por id, existencia por id, pagina de pratos e contagem.
- `IMesaRepository`: escrita e consulta por id.
- `IAtendenteRepository`: escrita.
- `IPedidoRepository`: escrita.
- `IPedidoPratoRepository`: escrita.
- `ILogginRepository`: registro de log.

## Queries

- `IPratoRepository.ExisteComId`.
- `IPratoRepository.ListarPagina`.
- `IPratoRepository.Contar`.
- `IPratoRepository.ObterPorId`.
- `IMesaRepository.ObterPorId`.

## Correcoes

- Persistencia deixou de engolir excecoes em consulta por id.
- Persistencia deixou de escrever em `Console.WriteLine`.
- `PratoService.Adicionar` deixou de usar `.Result`.
- Fakes de testes deixaram de implementar contrato generico.
- Teste arquitetural impede reintroducao de repository generico no core e infraestrutura do sample.

## Debitos mantidos

- Metodos de escrita dos repositories concretos ainda chamam `SampleRestaurantDbContext.SaveChangesAsync`.
- Unit of Work sera tratado no Prompt 4 / issue `#15`.
- CancellationToken transversal fica para o Prompt 5.
- Ordenacao deterministica de pagina fica para o Prompt 7.

## Validacao

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou.
- `dotnet test --configuration Release --no-build`: passou, 48 + 26 testes.
- `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter Category=Integration`: passou, 26 testes.
- Greps literais de repository generico: vazios.
- OpenAPI regenerado sem diff de contrato.
- Smoke, regressao HTTP, SQL Server real e Redis cobertos pela suite de integracao existente; Redis nao foi alterado.

## Delivery

- Commit semantico planejado: `refactor: replace generic repository with explicit ports`.
- Push: nao realizado.
- Proximo prompt/issue: `#15`, Prompt 4 - Unit of Work.
