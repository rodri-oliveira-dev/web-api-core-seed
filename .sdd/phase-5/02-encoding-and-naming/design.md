# Design - Encoding And Naming

## Decisions

1. Rename active inbound port namespace from `Intefaces` to `Interfaces`.
2. Rename API extension namespace/folder from `Clains` to `Claims`.
3. Rename active `Loggin*` C# model, validator, service, repository, and DI registrations to `LogEntry*`.
4. Preserve the persisted table name `Loggin` by keeping explicit EF Core mapping.
5. Update the current `SampleRestaurantDbContextModelSnapshot` only as an active EF generated artifact, while preserving historical migration files.
6. Correct active user-facing Portuguese messages with UTF-8 text.
7. Regenerate OpenAPI and accept only text-description changes.
8. Add automated regression checks for active source/OpenAPI mojibake and known misspellings.

## Implementation Shape

- Use file renames for active files:
  - `LogginEntity.cs` -> `LogEntry.cs`
  - `LogginValidation.cs` -> `LogEntryValidation.cs`
  - `LogginService .cs` -> `LogEntryService.cs`
  - `ILogginService.cs` -> `ILogEntryService.cs`
  - `ILogginRepository.cs` -> `ILogEntryRepository.cs`
  - `LogginRepository.cs` -> `LogEntryRepository.cs`
  - `LogginMapping.cs` -> `LogEntryMapping.cs`
  - `Extensions/Clains` -> `Extensions/Claims`
- Use targeted source edits after renames.
- Do not edit historical migration files except the active model snapshot if EF model alignment requires it.
- Keep legacy table references in SQL fixtures and migration tests.

## Test Strategy

- Update architecture tests to assert corrected namespaces and renamed contracts.
- Add or update tests for login messages with correct Portuguese characters.
- Add or update Problem Details tests for UTF-8 JSON and corrected text.
- Add OpenAPI regression checks for absence of mojibake and corrected response descriptions.
- Add SQL Server integration coverage that `LogEntry` still maps to table `Loggin`.
- Keep existing empty database and legacy upgrade migration tests as compatibility validation.

## Risks

- EF Core may report pending model changes if the snapshot entity name is not aligned with `LogEntry`.
- Broad text correction could alter assertions unexpectedly; keep changes to public text only.
- SDD inventories necessarily mention legacy misspellings; final searches must distinguish active code from documentation/history.
