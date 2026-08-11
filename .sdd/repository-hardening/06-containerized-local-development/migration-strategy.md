# Migration Strategy

The API does not apply migrations during startup. The Compose stack uses a one-shot `migrations` service.

## Execution

The script `scripts/docker/apply-migrations.sh`:

1. Requires `ConnectionStrings__DefaultConnection`.
2. Applies `ApplicationDbContext` migrations.
3. Applies `SampleRestaurantDbContext` migrations.
4. Stops immediately on failure through `set -eu`.
5. Does not print the connection string.

## Commands

The service runs:

```bash
dotnet ef database update --context ApplicationDbContext --project src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build
dotnet ef database update --context SampleRestaurantDbContext --project src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj --startup-project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj --configuration Release --no-build
```

The API depends on `migrations` with `condition: service_completed_successfully`.
