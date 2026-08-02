# Design

## Outcome for this prompt

The adoption is blocked exclusively by package unavailability. Therefore the design implemented in this prompt is documentation-only:

- Record requirements.
- Record package availability evidence.
- Record rule applicability and planned severities.
- Record validation and handoff.
- Do not change packages, project files, lock files or analyzer configuration.

## Future package design when unblocked

Centralize versions through CPM:

```xml
<PackageVersion Include="CSF.Analyzers.Architecture" Version="<published-version>" />
<PackageVersion Include="CSF.Analyzers.Reliability" Version="<published-version>" />
```

Analyzer references should use:

```xml
<PackageReference Include="CSF.Analyzers.Architecture">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

and the same shape for `CSF.Analyzers.Reliability`, adjusted only if the published package composition requires less.

## Future installation scope

| Project | Architecture | Reliability | Testing |
| --- | --- | --- | --- |
| API | Yes | Yes | No |
| SampleRestaurant core | Yes | No | No |
| SampleRestaurant infrastructure | Not initially, except later `ARC005` baseline | Yes | No |
| Identity infrastructure | Not initially, except later `ARC005` baseline | Not initially | No |
| Unit tests | No | No | No |
| Integration tests | No | No | No |

## Future `ARC002` boundary

Use actual namespaces, including legacy names that currently represent core:

- Core/domain/application: `WebApiCoreSeed.SampleRestaurant.Models*`, `WebApiCoreSeed.SampleRestaurant.Services*`, `WebApiCoreSeed.SampleRestaurant.Notificacoes*`, `WebApiCoreSeed.SampleRestaurant.Intefaces*`, `WebApiCoreSeed.SampleRestaurant.Interfaces*`, `WebApiCoreSeed.SampleRestaurant.Application*`.
- Forbidden infrastructure/framework namespaces: `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore`, `StackExchange.Redis`, `Microsoft.Extensions.Caching`, `WebApiCoreSeed.SampleRestaurant.Infrastructure*`, `WebApiCoreSeed.Identity.Infrastructure*`.

## CI behavior when unblocked

No dedicated CI step is needed if packages are normal analyzer `PackageReference`s and the existing CI build remains:

```bash
dotnet build "$SOLUTION" --configuration Release --no-restore
```

The analyzer execution should happen as part of build.
