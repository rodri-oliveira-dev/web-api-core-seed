#!/bin/sh
set -eu

repo_root="/src"
api_project="$repo_root/src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj"
identity_project="$repo_root/src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj"
sample_project="$repo_root/src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj"

if [ -z "${ConnectionStrings__DefaultConnection:-}" ]; then
  echo "ConnectionStrings__DefaultConnection is required." >&2
  exit 1
fi

echo "Applying Identity migrations..."
dotnet ef database update \
  --context ApplicationDbContext \
  --project "$identity_project" \
  --startup-project "$api_project" \
  --configuration Release \
  --no-build

echo "Applying SampleRestaurant migrations..."
dotnet ef database update \
  --context SampleRestaurantDbContext \
  --project "$sample_project" \
  --startup-project "$api_project" \
  --configuration Release \
  --no-build

echo "Migrations completed."
