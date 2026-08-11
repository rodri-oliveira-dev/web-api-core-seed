# Validation - 03 Problem Details

## Baseline

- `git status --short`: limpo antes da alteracao.
- `git branch --show-current`: `phase/2-dotnet-10-migration`.
- `git log -3 --oneline`:
  - `24f701d refactor: adopt modern ASP.NET Core hosting`
  - `b8593c5 build: migrate solution to .NET 10`
  - `1bcce0d chore: bootstrap modernization tooling`
- `dotnet build --configuration Release`: passou, com avisos existentes.
- `dotnet test --configuration Release --no-build`: passou, 21 testes.

## Validacao Final

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou.
- `dotnet test --configuration Release --no-build`: passou, 27 testes.

## Testes Adicionados

Arquivo: `test/Pedidos.Test/Integracao/ProblemDetailsContractTests.cs`.

Cenarios:

- payload invalido retorna 400 `application/problem+json` com `errors` e `traceId`;
- recurso inexistente retorna 404 `application/problem+json`;
- regra de dominio retorna 400 `application/problem+json` com `errors.notifications`;
- excecao inesperada retorna 500 `application/problem+json` sem stack trace, SQL, connection string ou token;
- endpoint protegido sem token retorna 401 `application/problem+json`;
- Swagger e `/hc` respondem no host de teste com health checks limpos.

Infraestrutura temporaria:

- `WebApplicationFactory<Program>`;
- EF Core InMemory para `MeuDbContext` e `ApplicationDbContext`;
- fakes de repositorios para isolar contrato HTTP de SQL Server;
- health checks limpos apenas no host de teste.

## Smoke Real

API iniciada com:

```text
dotnet run --project src/DevIO.Api/Restaurante.IO.Api.csproj --configuration Release --no-build --urls http://127.0.0.1:5099
```

Variaveis locais usadas no job:

- `ASPNETCORE_ENVIRONMENT=Testing`
- `RedisCacheSettings__Enabled=false`
- `DatasulSeqSettings__Enabled=false`

Resultados:

| Endpoint | Resultado |
| --- | --- |
| `/swagger/v1/swagger.json` | 200 `application/json;charset=utf-8` |
| `POST /api/v1/entrar` com `{}` | 400 `application/problem+json` |
| `/api/v1/nao-existe` | 404 `application/problem+json` |
| `/api/v1/Mesas/{guid}` sem token | 401 `application/problem+json; charset=utf-8` |
| `/hc` | `000`, timeout local por dependencia SQL Server |

## Regressao

A regressao dos contratos principais foi coberta por HTTP via `WebApplicationFactory`:

- endpoint valido de Swagger;
- payload invalido;
- ID inexistente;
- endpoint autenticado sem token;
- erro controlado de regra de dominio;
- erro inesperado controlado por fake de repositorio;
- health check em host de teste.

## Duplicacoes

Pesquisas apos a implementacao:

- `ErrorHandlingMiddleware`: sem ocorrencias ativas no codigo rastreado.
- `UseExceptionHandler`: uma ocorrencia em `HostingConfig`, usando `UseExceptionHandler()`.
- `AddProblemDetails`: uma ocorrencia em `HostingConfig`.
- `IExceptionHandler`: tres handlers coesos em `src/DevIO.Api/Errors`.
