# `dotnet new` template distribution

[English](template-distribution.md) | [Português (Brasil)](template-distribution.pt-BR.md) | [Back to README](../README.md)

The repository can now be **packed locally** as a .NET template NuGet. **This is not a published NuGet.org package or v2.0.0 GitHub Release**; publication is tracked in [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23).

## Identity and package layout

| Property | Value |
| --- | --- |
| Template identity | `RodriOliveira.WebApiCoreSeed.CSharp` |
| Short name | `webapi-seed` |
| Package ID | `RodriOliveira.WebApiCoreSeed.Templates` |
| Package version | `2.0.0-preview.1` (local preview) |
| Target SDK | `global.json` — .NET 10.0.401 |
| Source token | `WebApiCoreSeed`, replaced with the supplied `-n` name |

The NuGet contains only the reusable source projects, tests, OpenAPI generator/contracts, Compose/Docker/runtime scripts, base configuration, and a generated-project README in EN/PT-BR. It explicitly **does not ship** the source repo CI, SDD history, contribution administration, secrets or `bin/obj` outputs. Each generated API receives a newly generated `UserSecretsId`; the historical project's fixed ID is not copied.

### Build and install locally

From the root of the repository:

```bash
dotnet pack template-pack/WebApiCoreSeed.Templates.csproj -c Release -o ./artifacts/templates
dotnet new install ./artifacts/templates/RodriOliveira.WebApiCoreSeed.Templates.2.0.0-preview.1.nupkg
dotnet new webapi-seed -n SampleApi
cd SampleApi
dotnet restore SampleApi.slnx
dotnet build SampleApi.slnx --configuration Release --no-restore
```

The example generates a full project named `SampleApi`: projects, namespaces, solution references, Dockerfile entrypoints and runtime scripts inherit the new name. Docker Compose deliberately uses a safe default project ID (`web-api-core-seed`) independent of the .NET name; set `COMPOSE_PROJECT_NAME` in the generated `.env.local` to a unique lowercase value such as `sample-api` to avoid collisions. Dotted names such as `Acme.Api` remain valid C# names and generate valid Compose configurations. The generated EN/PT-BR READMEs retain the source repository links.

### Run the generated application

Start Docker, copy `.env.local.example` to `.env.local`, replace all credential placeholders and run:

```bash
docker compose --env-file .env.local up --build -d
docker compose --env-file .env.local --profile tools up seed
curl -i http://localhost:8080/health/live
```

The generated project contains unit/integration tests and sample domain data. Integration tests use Testcontainers. For runtime and migration customization, see the generated README.

### Update or uninstall

For **local** package upgrades, first uninstall the previously installed package, then install the new preview `.nupkg`:

```bash
dotnet new uninstall RodriOliveira.WebApiCoreSeed.Templates
dotnet new install ./artifacts/templates/RodriOliveira.WebApiCoreSeed.Templates.2.0.0-preview.1.nupkg
```

To remove the template:

```bash
dotnet new uninstall RodriOliveira.WebApiCoreSeed.Templates
```

For a future **feed-published** package, `dotnet new install RodriOliveira.WebApiCoreSeed.Templates` and `dotnet new update` become appropriate after release; do not assume this NuGet ID is available online today.

## Options evaluated

Authentication, SQL Server, Redis and OpenTelemetry are **part of the maintained seed**. We intentionally do not expose `--auth`, `--database`, `--redis` or `--telemetry` generation-time flags yet: removing these components is architectural (services, references, endpoints, tests, migrations and configurations) and cannot safely be implemented by changing `appsettings.json` alone. The generated app can disable Redis and OpenTelemetry at **runtime** through configuration. Optional feature generation may be introduced later with per-combination tests.

## Quality checks

Run `bash scripts/templates/smoke-test.sh` from the repository to pack, inspect, install into an isolated CLI home, generate `SampleApi`, verify rename/secrets isolation, restore, build, run unit/integration tests and probe a started generated API. GitHub Actions runs the same script in [template-smoke](../.github/workflows/template-smoke.yml). The script requires Docker for integration tests and does not publish any package.
