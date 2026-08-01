# Dependency Rules - Phase 4

## Regras por camada

| Origem | Pode depender de | Nao pode depender de |
| --- | --- | --- |
| Domain | BCL e bibliotecas de validacao estritamente usadas em invariantes legadas | `Restaurante.IO.Api`, `Restaurante.IO.Data`, ASP.NET Core, EF Core, Redis, Identity, logging |
| Application | Domain e portas do proprio modulo | `Restaurante.IO.Api`, `Restaurante.IO.Data`, ASP.NET Core, EF Core, Redis, Identity concreta |
| Infrastructure | Application, Domain e bibliotecas tecnicas necessarias | API, controllers, view models |
| Api | Application e Infrastructure para composicao | Infraestrutura interna de outro modulo por conveniencia |
| Tests | Projetos necessarios para verificacao | Dependencias produtivas artificiais |

## Regras entre modulos

- Um modulo nao deve acessar `DbContext`, repositorio concreto, migrations ou mappings internos de outro modulo.
- Comunicacao entre modulos deve usar contratos publicos, portas de entrada ou casos de uso.
- Mensageria nao deve ser introduzida apenas para comunicacao dentro do monolito.
- `SharedKernel` deve permanecer inexistente ou minimo ate surgir necessidade real.

## Regras verificaveis nesta entrega

- `Restaurante.IO.Business` nao referencia `Restaurante.IO.Api`.
- `Restaurante.IO.Business` nao referencia `Restaurante.IO.Data`.
- `Restaurante.IO.Business` nao referencia assemblies `Microsoft.AspNetCore*`.
- `Restaurante.IO.Business` nao referencia assemblies `Microsoft.EntityFrameworkCore*`.
- `Restaurante.IO.Business` nao referencia assemblies `Microsoft.Extensions.Logging*`.
- `Restaurante.IO.Data` referencia `Restaurante.IO.Business`.
- `Restaurante.IO.Api` referencia `Restaurante.IO.Business` e `Restaurante.IO.Data`.
- Controllers de dominio nao devem injetar interfaces `IRepository<>` nem interfaces cujo nome termine em `Repository`.

## Testes implementados

As regras verificaveis desta entrega sao cobertas por `test/Pedidos.Test/Arquitetura/ModularHexagonalArchitectureTest.cs`.
