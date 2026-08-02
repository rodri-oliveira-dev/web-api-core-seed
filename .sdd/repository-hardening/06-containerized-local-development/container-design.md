# Container Design

## Dockerfile

The root `Dockerfile` uses these stages:

- `runtime`: `mcr.microsoft.com/dotnet/aspnet:10.0`, internal HTTP port `8080`.
- `restore`: `mcr.microsoft.com/dotnet/sdk:10.0`, copies restore inputs first for cache stability.
- `build`: copies source and builds the API in Release.
- `publish`: publishes the API with `UseAppHost=false`.
- `migrations`: SDK-based stage with `dotnet-ef` `10.0.10` and the migration script.
- `final`: copies only publish output and runs `WebApiCoreSeed.Api.dll` as user `app`.

## Restore Inputs

Restore copies `global.json`, `Directory.Build.props`, root and `src` CPM files, `WebApiCoreSeed.slnx` and the project files needed by the API dependency graph before copying source.

## Health Check Decision

The final ASP.NET image does not include `curl`, `wget` or another HTTP client. No Dockerfile `HEALTHCHECK` is added. API health is exposed through `/health/live`, `/health/ready` and `/hc`, and validated externally.

## Tag Decision

The .NET image tags use `10.0` to stay on the supported .NET 10 servicing line. Digests were not pinned to avoid making SDK/runtime updates obscure for this educational seed.
