# Validation - Encoding And Naming

## Planned Commands

```powershell
rg -n "<mojibake-marker-regex>" .
rg -n "Intefaces|Clains|Loggin|FluentValidator" .
dotnet restore WebApiCoreSeed.slnx --locked-mode
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build
dotnet ef migrations has-pending-model-changes --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context ApplicationDbContext
dotnet ef migrations has-pending-model-changes --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --context SampleRestaurantDbContext
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
dotnet list WebApiCoreSeed.slnx package --vulnerable
git diff --check
```

## Initial Results

- Initial active code mojibake search found:
  - `src/WebApiCoreSeed.Api/Controllers/AuthControllerBase.cs`
- Initial active naming search found:
  - `Intefaces` in active namespaces/usings.
  - `Clains` in API folder/namespace/usings.
  - `Loggin` in active C# types/files plus preserved schema/migration/test references.
  - `FluentValidator` in active docs.

## Final Results

| Validation | Result |
| --- | --- |
| Active mojibake marker search in the full repository excluding build outputs | Passed; no matches. |
| Active search for corrected names | Passed except historical migration designers, intentionally preserved schema references and compatibility tests. |
| `dotnet restore WebApiCoreSeed.slnx --locked-mode` | Passed. |
| `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore` | Passed with 0 warnings and 0 errors after the coverage remediation tests. |
| Unit tests | Passed: 124 tests. |
| Integration tests | Passed: 54 tests. |
| Architecture tests | Passed explicitly: 8 tests. |
| Migrations on empty database test | Passed explicitly. |
| Legacy schema upgrade test | Passed explicitly. |
| `dotnet ef migrations has-pending-model-changes` for `ApplicationDbContext` | Passed with dummy `ConnectionStrings__DefaultConnection`; no pending changes. |
| `dotnet ef migrations has-pending-model-changes` for `SampleRestaurantDbContext` | Passed with dummy `ConnectionStrings__DefaultConnection`; no pending changes. |
| OpenAPI regeneration | Passed with `tools/OpenApiGenerator`. |
| OpenAPI JSON validation | Passed for `openapi-v1.json` and `openapi-v2.json`. |
| OpenAPI comparison | Text-only description changes for accented Portuguese in 400/401/429 responses. |
| `dotnet list WebApiCoreSeed.slnx package --vulnerable` | Passed; no vulnerable packages in current sources. |
| `git diff --check` | Passed. |
| Unexpected migrations created | None. |

## Remote Quality Gate Follow-Up

- The first PR run passed build/test, CodeQL and dependency review, but failed the SonarCloud Quality Gate because new-code coverage was `66.0`, below the `80` threshold.
- The failed condition was `new_coverage`; all other queried quality gate conditions were OK.
- Additional focused tests were added for `LogEntryValidation`, `LogEntryService`, `LogEntryRepository` and newly normalized Problem Details text.
- Local unit tests increased from 115 to 124 passing tests before the follow-up push.
- The follow-up PR run passed Build/test, CodeQL, Dependency Review and SonarCloud Quality Gate.

## Notes

- `dotnet ef` emitted the existing local warning that EF tools `10.0.10` are older than runtime `10.0.11`; commands still passed.
- The first EF attempt failed because the design-time connection string was absent; rerun passed with a dummy `DefaultConnection`, consistent with previous phase notes.
- Final full-repository mojibake marker search returned no matches.
- Final naming search returned only preserved `Loggin` table/schema references in mappings, snapshots, historical migrations and compatibility tests.
