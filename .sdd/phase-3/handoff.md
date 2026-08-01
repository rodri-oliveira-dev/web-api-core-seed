# Handoff - Phase 3

## Estado atual

- Branch atual: `phase/3-quality-and-safety`.
- Branch-base: `phase/2-dotnet-10-migration`.
- Prompt atual: `02 - Integration test baseline`.
- Issue atual: `#12`.
- Status do prompt 01: concluido.
- Status do prompt 02: concluido.
- Commit do prompt 01: `test: strengthen existing unit test suite`.
- Commit do prompt 02: `test: add API and infrastructure integration tests`.

## Resultado do prompt 02

- Projeto criado: `test/WebApiCoreSeed.IntegrationTests`.
- Testes adicionados: 18.
- Fixtures/helpers:
  - `ApiFactory`
  - `DatabaseReset`
  - `AuthenticationHelper`
  - `JsonAssertions`
  - `TestData`
- Containers:
  - SQL Server `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`
  - Redis `redis:7.4.2-alpine`
- Migrations:
  - `ApplicationDbContext`: `20200817223121_InitialCreate`
  - `MeuDbContext`: `20200817223231_InitialCreate`
- Isolamento:
  - collection xUnit `api-integration`
  - sem paralelismo interno
  - reset SQL por `DELETE` ordenado
  - reset Redis por `FLUSHDB`
  - seeds minimos por teste
- Traits:
  - `Category=Integration`
  - `Category=Container`

## Ajustes relevantes

- `AtendenteMapping` ignora `Email` e `Telefone` para preservar o schema legado versionado nas migrations.
- `Program` ja era publico; nao foi necessario adicionar `public partial class Program`.
- O ambiente `Testing` usa variaveis de ambiente temporarias restauradas no teardown para garantir que `AddApiServices` leia as connection strings dos containers.

## Validacoes oficiais

```text
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet test --configuration Release --no-build --filter "Category=Integration"
dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "Category=Integration"
```

Resultados:

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou.
- `dotnet test --configuration Release --no-build`: passou com 36 testes existentes e 18 testes de integracao.
- Filtro `Category=Integration`: passou com 18 testes.
- Segunda execucao isolada do projeto de integracao: passou com 18 testes.
- Docker estava disponivel; nenhuma limitacao de runtime foi registrada.
- Tempo observado da suite completa com containers: cerca de 40 segundos.

## Proxima issue

`#9` - executar o Prompt 3 da Fase 3, iniciando seguranca.
