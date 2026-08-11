# Project Inventory - Prompt 05 SLNX Migration

## Active Projects

| Project | Path | Logical folder |
| --- | --- | --- |
| `WebApiCoreSeed.Api` | `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | `/src/WebApiCoreSeed.Api/` |
| `WebApiCoreSeed.SampleRestaurant` | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj` | `/src/Modules/SampleRestaurant/` |
| `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | `/src/Modules/SampleRestaurant/` |
| `WebApiCoreSeed.Identity.Infrastructure` | `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj` | `/src/Modules/Identity/` |
| `WebApiCoreSeed.UnitTests` | `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj` | `/tests/` |
| `WebApiCoreSeed.IntegrationTests` | `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj` | `/tests/` |
| `OpenApiGenerator` | `tools/OpenApiGenerator/OpenApiGenerator.csproj` | `/tools/` |

## Equivalence Check

`dotnet sln WebApiCoreSeed.sln list` and `dotnet sln WebApiCoreSeed.slnx list` returned the same seven project paths before `WebApiCoreSeed.sln` was removed.

## Duplicate Check

No duplicate project path was identified in the CLI list or generated `.slnx`.
