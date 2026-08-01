# Dependency Rules - Phase 4

## Regras por camada

| Origem | Pode depender de | Nao pode depender de |
| --- | --- | --- |
| Domain | BCL e bibliotecas de validacao estritamente usadas em invariantes legadas | `WebApiCoreSeed.Api`, `WebApiCoreSeed.SampleRestaurant.Infrastructure`, ASP.NET Core, EF Core, Redis, Identity, logging |
| Application | Domain e portas do proprio modulo | `WebApiCoreSeed.Api`, `WebApiCoreSeed.SampleRestaurant.Infrastructure`, ASP.NET Core, EF Core, Redis, Identity concreta |
| Infrastructure | Application, Domain e bibliotecas tecnicas necessarias | API, controllers, view models |
| Api | Application e Infrastructure para composicao | Infraestrutura interna de outro modulo por conveniencia |
| Tests | Projetos necessarios para verificacao | Dependencias produtivas artificiais |

## Regras entre modulos

- Um modulo nao deve acessar `DbContext`, repositorio concreto, migrations ou mappings internos de outro modulo.
- Comunicacao entre modulos deve usar contratos publicos, portas de entrada ou casos de uso.
- Mensageria nao deve ser introduzida apenas para comunicacao dentro do monolito.
- `SharedKernel` deve permanecer inexistente ou minimo ate surgir necessidade real.

## Regras verificaveis nesta entrega

- `WebApiCoreSeed.SampleRestaurant` nao referencia `WebApiCoreSeed.Api`.
- `WebApiCoreSeed.SampleRestaurant` nao referencia `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- `WebApiCoreSeed.SampleRestaurant` nao referencia assemblies `Microsoft.AspNetCore*`.
- `WebApiCoreSeed.SampleRestaurant` nao referencia assemblies `Microsoft.EntityFrameworkCore*`.
- `WebApiCoreSeed.SampleRestaurant` nao referencia assemblies `Microsoft.Extensions.Logging*`.
- `WebApiCoreSeed.SampleRestaurant.Infrastructure` referencia `WebApiCoreSeed.SampleRestaurant`.
- `WebApiCoreSeed.Api` referencia `WebApiCoreSeed.SampleRestaurant` e `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- Controllers de dominio nao devem injetar repositorios genericos nem interfaces cujo nome termine em `Repository`.
- O modulo `SampleRestaurant` nao deve declarar repositorio generico no core nem na infraestrutura.
- Portas de persistencia nao devem expor `IQueryable` nem receber predicados arbitrarios `Expression<Func<...>>`.
- Repositories concretos podem usar `SampleRestaurantDbContext` somente dentro da infraestrutura.
- Queries paginadas devem aplicar ordenacao estavel antes de `Skip` e `Take`.
- Page size deve ter default, minimo e maximo documentados.
- Valores invalidos de paginacao devem retornar Problem Details.

## Testes implementados

As regras verificaveis desta entrega sao cobertas por `tests/WebApiCoreSeed.UnitTests/Arquitetura/ModularHexagonalArchitectureTest.cs`.

No Prompt 03 foi adicionada verificacao para impedir a declaracao de repositorio generico no core e na infraestrutura do sample.

No Prompt 07 foram adicionados testes de integracao para limites, metadata e ordenacao estavel da paginacao de pratos.
