# Report - Encoding And Naming

## Summary

Active UTF-8 text and naming issues were normalized without changing HTTP routes, JSON properties, status codes, persisted table names, or historical migration IDs.

## Issue And Branch

- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/35.
- Branch: `refactor/normalize-encoding-and-naming`.

## Renamed Files

- `src/WebApiCoreSeed.Api/Extensions/Clains/CustomAuthorization.cs` -> `src/WebApiCoreSeed.Api/Extensions/Claims/CustomAuthorization.cs`
- `LogginEntity.cs` -> `LogEntry.cs`
- `LogginValidation.cs` -> `LogEntryValidation.cs`
- `LogginService .cs` -> `LogEntryService.cs`
- `ILogginService.cs` -> `ILogEntryService.cs`
- `ILogginRepository.cs` -> `ILogEntryRepository.cs`
- `LogginRepository.cs` -> `LogEntryRepository.cs`
- `LogginMapping.cs` -> `LogEntryMapping.cs`

## Corrected Namespaces And Types

- `WebApiCoreSeed.SampleRestaurant.Intefaces` -> `WebApiCoreSeed.SampleRestaurant.Interfaces`
- `WebApiCoreSeed.SampleRestaurant.Intefaces.Service` -> `WebApiCoreSeed.SampleRestaurant.Interfaces.Service`
- `WebApiCoreSeed.Api.Extensions.Clains` -> `WebApiCoreSeed.Api.Extensions.Claims`
- `LogginEntity` -> `LogEntry`
- `ILogginService` -> `ILogEntryService`
- `ILogginRepository` -> `ILogEntryRepository`
- `LogginService` -> `LogEntryService`
- `LogginRepository` -> `LogEntryRepository`
- `LogginValidation` -> `LogEntryValidation`
- `LogginMapping` -> `LogEntryMapping`

## Legacy Identifiers Preserved

- Table `Loggin`.
- Primary key `PK_Loggin`.
- Historical migration IDs and historical migration designer metadata.
- Legacy upgrade fixture SQL and assertions against table `Loggin`.

## OpenAPI Changes

Only response descriptions changed:

- `Autenticacao necessaria.` -> `Autenticação necessária.`
- `A chamada precisa ser efetuada por um usuario autenticado.` -> `A chamada precisa ser efetuada por um usuário autenticado.`
- `Requisicao invalida.` -> `Requisição inválida.`
- `Limite de requisicoes excedido.` -> `Limite de requisições excedido.`

No route, schema, status code, or security requirement changed.

## Tests Added Or Updated

- File-level regression test for mojibake and corrected active names.
- Runtime Problem Details/OpenAPI tests for corrected Portuguese text.
- Architecture tests updated to the corrected `Interfaces` namespace and `LogEntry` ports.
- SQL Server integration test proving `LogEntry` persists through legacy table `Loggin`.

## Validation

Full results are recorded in `validation.md`.
