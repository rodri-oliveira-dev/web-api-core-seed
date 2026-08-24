# Discovery - Encoding And Naming

## Sources Read

- `AGENTS.md`
- `.editorconfig`
- `.gitattributes`
- `README.md`
- `src/README.md`
- `LEGACY.md`
- `.sdd/phase-4/status.md`
- `.sdd/phase-4/handoff.md`
- `.sdd/phase-5/status.md`
- `.sdd/phase-5/handoff.md`
- `.sdd/phase-5/decisions.md`

## Active Solution

- Solution: `WebApiCoreSeed.slnx`
- API: `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`
- Sample core: `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj`
- Sample infrastructure: `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj`
- Identity infrastructure: `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`
- Unit tests: `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`
- Integration tests: `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`

## OpenAPI And Baselines

- Current generated OpenAPI:
  - `docs/openapi/openapi-v1.json`
  - `docs/openapi/openapi-v2.json`
- Historical baseline:
  - `docs/openapi/baseline/swagger-v1.json`
  - `docs/openapi/baseline/swagger-v2.json`
- Active OpenAPI descriptions currently expose unaccented Portuguese text such as `Requisicao invalida.` and `Autenticacao necessaria.`.
- No current generated OpenAPI file contained `Ã`, `Â`, or `�` in the initial targeted search.

## Migrations And Loggin

- Historical sample migration:
  - `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/Migrations/20200817223231_InitialCreate.cs`
  - Creates table `Loggin`.
- Current sample snapshot:
  - `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/Migrations/SampleRestaurantDbContextModelSnapshot.cs`
  - Contains entity name `WebApiCoreSeed.SampleRestaurant.Models.LogginEntity` and `ToTable("Loggin")`.
- Current mapping:
  - `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/Modules/SampleRestaurant/Infrastructure/Persistence/Mappings/LogginMapping.cs`
  - Explicitly maps to table `Loggin`.
- Legacy upgrade tests insert and count table `Loggin`; these are compatibility fixtures and must preserve the persisted identifier.

## Architecture Tests

- `tests/WebApiCoreSeed.UnitTests/Arquitetura/ModularHexagonalArchitectureTest.cs`
- The test currently references `WebApiCoreSeed.SampleRestaurant.Intefaces.Service` and `ILoggin*` contracts.
- It is the right place to assert corrected active namespaces.

## Initial Searches

- `Ã|Â|�` in active code found two active occurrences in `src/WebApiCoreSeed.Api/Controllers/AuthControllerBase.cs`.
- `Intefaces` appears in active namespaces/usings for inbound application ports and tests.
- `Clains` appears in API extension namespace/folder and related usings.
- `Loggin` appears in active C# types/files, EF mapping, DbContext, DI, tests, snapshots, migrations, legacy upgrade tests, and SDD history.
- `FluentValidator` appears in active docs `README.md` and `src/README.md`.
