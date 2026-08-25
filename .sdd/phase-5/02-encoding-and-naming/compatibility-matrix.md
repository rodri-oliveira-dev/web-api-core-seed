# Compatibility Matrix

| Area | Current problem | Change | Compatibility result |
| --- | --- | --- | --- |
| HTTP routes | No route uses `Intefaces`, `Clains`, or `Loggin`. | No route changes. | Non-breaking |
| JSON properties | No public JSON property is renamed by this change. | No serializer contract changes planned. | Non-breaking |
| Status codes | Login/domain validation keeps existing flow. | Only message text is corrected. | Non-breaking, visible text change |
| Problem Details | Portuguese text is corrupted or unaccented in active messages. | Correct text in `title`, `detail`, and notification messages. | Public text correction |
| OpenAPI | Generated response descriptions inherit active text. | Regenerate OpenAPI after text corrections. | OpenAPI text-only change |
| EF table `Loggin` | Misspelled persisted table name. | Preserve table with `ToTable("Loggin")`. | Legacy identifier preserved |
| EF columns in `Loggin` | Legacy columns `EventId`, `Escopo`, `LogLevel`, `Message`, `CreatedTime`. | No column changes. | Non-breaking |
| Historical migrations | Contain `Loggin` and old model name. | Preserve historical migration IDs and files. | Legacy history preserved |
| Model snapshot | Represents current EF model. | Update active snapshot to `LogEntry` while keeping `ToTable("Loggin")`. | Required to avoid pending model changes |
| Legacy upgrade tests | Use table `Loggin` explicitly. | Preserve table assertions, adapt C# imports if needed. | Compatibility validated |
| Namespaces | `Intefaces` and `Clains` are active internal typos. | Rename to `Interfaces` and `Claims`. | Source-breaking only internally; no package API promise in seed |
| Documentation | Active docs contain obsolete `FluentValidator`. | Correct active docs. | Non-breaking |

## Identifiers Preserved

- `Loggin` table name.
- `PK_Loggin` primary key name in historical migration/schema.
- `__EFMigrationsHistory` entries:
  - `20200817223121_InitialCreate`
  - `20200817223231_InitialCreate`
  - `20260801191447_AddPratosPaginationOrderingIndex`
- Identity `AspNet*` table and index names.
