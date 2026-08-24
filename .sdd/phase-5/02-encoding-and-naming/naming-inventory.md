# Naming Inventory

| Occurrence | Location | Classification | Decision | Correction classification |
| --- | --- | --- | --- | --- |
| `WebApiCoreSeed.SampleRestaurant.Intefaces` | Active usings/namespaces in API, sample core, unit tests, integration tests | Código ativo interno / teste | Rename namespace to `WebApiCoreSeed.SampleRestaurant.Interfaces`. | Interna e não quebradora |
| `WebApiCoreSeed.SampleRestaurant.Intefaces.Service` | Active usings/namespaces in inbound ports, API controllers, DI, tests | Código ativo interno / teste | Rename namespace to `WebApiCoreSeed.SampleRestaurant.Interfaces.Service`. | Interna e não quebradora |
| `src/WebApiCoreSeed.Api/Extensions/Clains` | API folder and namespace | Código ativo interno | Rename folder/namespace to `Claims`. | Interna e não quebradora |
| `LogginService .cs` | Active service file | Código ativo interno | Rename file to `LogEntryService.cs`. | Interna e não quebradora |
| `LogginEntity` | Active domain model and EF model | Código ativo interno | Rename to `LogEntry`. | Interna e não quebradora |
| `LogginValidation` | Active validator | Código ativo interno | Rename to `LogEntryValidation`. | Interna e não quebradora |
| `ILogginService` | Active inbound port | Código ativo interno | Rename to `ILogEntryService`. | Interna e não quebradora |
| `ILogginRepository` | Active outbound port | Código ativo interno | Rename to `ILogEntryRepository`. | Interna e não quebradora |
| `LogginRepository` | Active EF repository | Código ativo interno | Rename to `LogEntryRepository`. | Interna e não quebradora |
| `LogginMapping` | Active EF mapping | Código ativo interno / identificador persistido no banco | Rename C# type/file to `LogEntryMapping`, keep `ToTable("Loggin")`. | Identificador legado preservado |
| `DbSet<LogginEntity> Loggins` | `SampleRestaurantDbContext` | Código ativo interno / EF model | Rename to `DbSet<LogEntry> LogEntries`. | Interna e não quebradora |
| `Loggin` table | Migrations, snapshots, legacy SQL, tests | Identificador persistido no banco / migration histórica / fixture | Preserve table name exactly. | Identificador legado preservado |
| `WebApiCoreSeed.SampleRestaurant.Models.LogginEntity` in historical migration designers | Historical generated migrations | Migration histórica / artefato gerado | Preserve historical files; update only active model snapshot if required by EF model alignment. | Identificador legado preservado |
| `FluentValidator` | `README.md`, `src/README.md` | Documento ativo | Correct to `FluentValidation`. | Interna e não quebradora |

## Final Naming Decision

Use `LogEntry` for active C# model/service/repository naming because each persisted row represents one log event entry, while the database table name `Loggin` remains a legacy persisted identifier.
