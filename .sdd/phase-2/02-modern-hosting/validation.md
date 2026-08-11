# Validation - 02 Modern Hosting

## Baseline

| Command | Result |
| --- | --- |
| `dotnet build --configuration Release` | Passed with existing analyzer warnings and legacy-hosting warnings. |
| `dotnet test --configuration Release --no-build` | Passed, 21 tests. |

## Final Commands

| Command | Result |
| --- | --- |
| `dotnet restore` | Passed; all projects were up to date. |
| `dotnet build --configuration Release --no-restore` | Passed with 0 warnings in the final incremental Release build. |
| `dotnet test --configuration Release --no-build` | Passed, 21 tests. |
| Host-cleanup grep set | Passed; no matches for the legacy startup class, duplicate host builders or static startup configuration access. |
| `git diff --check` | Passed; Git reported only LF/CRLF normalization warnings from the Windows checkout. |

## Smoke Plan

Run the API from `src/DevIO.Api` with:

```powershell
dotnet run --no-build --configuration Release --urls http://localhost:5068
```

Use `ASPNETCORE_ENVIRONMENT=Development`, disable Redis and Seq for local smoke, and check:

- Process startup.
- `GET /swagger/v1/swagger.json`.
- `GET /error/404`.
- `POST /api/v1/nova-conta` without token as an authentication challenge.
- Development CORS preflight when viable.
- `GET /hc`.
- Clean shutdown.

## Known Runtime Limitation

Full `/hc` success still depends on local SQL Server availability. If SQL Server is unavailable, the expected smoke result is that the endpoint is registered and executes the SQL health check, but may return unhealthy or time out according to the legacy health-check behavior.

## Smoke Result

Smoke ran with `ASPNETCORE_ENVIRONMENT=Development`, Redis disabled and Seq health-check registration disabled.

| Check | Result |
| --- | --- |
| Process startup | Passed; API listened on `http://127.0.0.1:5068`. |
| Swagger document | `GET /swagger/v1/swagger.json` returned `200`. |
| Existing endpoint | `GET /error/404` returned `404`. |
| Authentication challenge | `POST /api/v1/nova-conta` without token returned `401`. |
| Development CORS | `OPTIONS /api/v1/Pratos` returned `204` with `Access-Control-Allow-Origin: *`. |
| Health check | `GET /hc` timed out after 12 seconds because local SQL Server is unavailable; endpoint registration was preserved. |
| Shutdown | Smoke job was stopped and port `5068` was no longer listening afterward. |
