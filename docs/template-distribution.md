# `dotnet new` template distribution

[English](template-distribution.md) | [Português (Brasil)](template-distribution.pt-BR.md) | [Back to README](../README.md)

The repository can be packed locally as a .NET template NuGet. On completion of the [v2.0.0 release workflow](../.github/workflows/release-v2.yml), the **same tested package** will also be attached to [GitHub Release v2.0.0](https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.0). **It is not published on NuGet.org.** If the Release link is not yet available, check the release workflow and [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23); do not assume publication merely because code was merged.

## Identity and package layout

| Property | Value |
| --- | --- |
| Template identity | `RodriOliveira.WebApiCoreSeed.CSharp` |
| Short name | `webapi-seed` |
| Package ID | `RodriOliveira.WebApiCoreSeed.Templates` |
| Package version | `2.0.0` (stable release artifact) |
| Target SDK | `global.json` — .NET 10.0.401 |
| Source token | `WebApiCoreSeed`, replaced with the supplied `-n` name |

The NuGet contains only the reusable source projects, tests, OpenAPI generator/contracts, Compose/Docker/runtime scripts, base configuration, and a generated-project README in EN/PT-BR. It explicitly **does not ship** the source repo CI, SDD history, contribution administration, secrets or `bin/obj` outputs. Each generated API receives a newly generated `UserSecretsId`; the historical project's fixed ID is not copied.

### Build and install locally

From the root of the repository:

```bash
dotnet pack template-pack/WebApiCoreSeed.Templates.csproj -c Release -o ./artifacts/templates
dotnet new install ./artifacts/templates/RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg
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

For **local** package upgrades, first uninstall the previously installed package, then install the new `.nupkg`:

```bash
dotnet new uninstall RodriOliveira.WebApiCoreSeed.Templates
dotnet new install ./artifacts/templates/RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg
```

To remove the template:

```bash
dotnet new uninstall RodriOliveira.WebApiCoreSeed.Templates
```

For a future **NuGet.org-published** package, `dotnet new install RodriOliveira.WebApiCoreSeed.Templates` and `dotnet new update` would become appropriate. **Neither command is a substitute for downloading the GitHub Release asset**, as no NuGet.org feed was selected.

## Options evaluated

Authentication, SQL Server, Redis and OpenTelemetry are **part of the maintained seed**. We intentionally do not expose `--auth`, `--database`, `--redis` or `--telemetry` generation-time flags yet: removing these components is architectural (services, references, endpoints, tests, migrations and configurations) and cannot safely be implemented by changing `appsettings.json` alone. The generated app can disable Redis and OpenTelemetry at **runtime** through configuration. Optional feature generation may be introduced later with per-combination tests.

## Quality checks

Run `bash scripts/templates/smoke-test.sh` from the repository to pack, inspect, install into an isolated CLI home, generate `SampleApi`, verify rename/secrets isolation, restore, build, run unit/integration tests and probe a started generated API. GitHub Actions runs the same script in [template-smoke](../.github/workflows/template-smoke.yml). The script requires Docker for integration tests and does not publish any package. To test an existing downloaded release asset, set `TEMPLATE_PACKAGE=/absolute/path/to/package.nupkg` before running the script; it will **not repack the source**. The [release workflow](../.github/workflows/release-v2.yml) uses this mode to verify the precise asset it publishes.
