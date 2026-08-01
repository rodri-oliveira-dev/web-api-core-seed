# Discovery - Prompt 01

## Baseline

- Branch criada: `phase/4-architecture-modernization`.
- Base usada: `phase/3-quality-and-safety` em `18af517adab5d21ae58ac9674da411244a5379b9`.
- Working tree inicial: limpa.
- `dotnet --info`: SDK `10.0.302`, runtime `10.0.10`.
- `dotnet restore RestauranteAPI.sln`: passou.
- `dotnet build RestauranteAPI.sln --configuration Release --no-restore`: passou com warnings de analyzers existentes.
- `dotnet test RestauranteAPI.sln --configuration Release --no-build`: passou com 41 testes em `Pedidos.Test` e 26 em `WebApiCoreSeed.IntegrationTests`.

## Projetos

| Projeto | Referencias |
| --- | --- |
| `Restaurante.IO.Api` | `Restaurante.IO.Business`, `Restaurante.IO.Data` |
| `Restaurante.IO.Business` | nenhuma referencia projeto-a-projeto |
| `Restaurante.IO.Data` | `Restaurante.IO.Business` |
| `Pedidos.Test` | `Restaurante.IO.Api`, `Restaurante.IO.Business` |
| `WebApiCoreSeed.IntegrationTests` | `Restaurante.IO.Api`, `Restaurante.IO.Data` |
| `OpenApiGenerator` | `Restaurante.IO.Api` |

## Dependencias NuGet relevantes

- API: ASP.NET Core, Identity, EF Core, Redis cache, OpenAPI, OpenTelemetry, Serilog, health checks.
- Business: FluentValidation e, inicialmente, `Microsoft.Extensions.Logging.Abstractions`.
- Data: EF Core SQL Server.
- Integration tests: WebApplicationFactory, Testcontainers SQL Server, Testcontainers Redis, StackExchange.Redis.

## Controllers

- `PratosController` injeta `IPratoRepository`, `IPratoService`, `IMapper`, `ILogger<PratosController>` e `IUser`.
- `MesasController` injeta `IMesaRepository`, `IMesaService` e `IMapper`.
- Auth controllers usam diretamente ASP.NET Core Identity, coerente com API como adaptador/composition root nesta etapa.

## Persistencia

- `MeuDbContext` fica em `Restaurante.IO.Data.Context`.
- Repositorios concretos ficam em `Restaurante.IO.Data.Repository`.
- Interfaces de repositorio ficam no Business e funcionam como portas de saida temporarias.
- `ApplicationDbContext` e migrations de Identity permanecem na API.

## Achados

- O nucleo de negocio nao referencia API nem Data, mas ainda tinha dependencia de logging via `LogginEntity.LogLevel`.
- Controllers de dominio dependiam de repositorios diretamente para leitura.
- A estrutura fisica ainda era por camadas tecnicas (`Business`, `Data`, `Api`), nao por modulo.
- Nao ha `Startup` ativo; o hosting moderno esta em `Program.cs` e `HostingConfig`.
- Nao foram encontrados domain events ativos.
