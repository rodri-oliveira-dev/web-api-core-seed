#!/bin/sh
set -eu

script_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
repo_root=$(CDPATH= cd -- "$script_dir/../.." && pwd)
api_project="$repo_root/src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj"

if [ ! -f "$api_project" ]; then
  echo "API project not found: $api_project" >&2
  exit 1
fi

user_secrets_id=$(dotnet msbuild "$api_project" -getProperty:UserSecretsId)
if [ -z "$user_secrets_id" ]; then
  echo "UserSecretsId is not configured for the API project. No new ID was generated." >&2
  exit 1
fi

read_secret() {
  prompt="$1"
  printf "%s" "$prompt" >&2
  stty -echo
  IFS= read -r value
  stty echo
  printf "\n" >&2

  if [ -z "$value" ]; then
    echo "Value cannot be empty." >&2
    exit 1
  fi

  printf "%s" "$value"
}

echo "Configuring User Secrets for src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj"
connection_string=$(read_secret "ConnectionStrings:DefaultConnection: ")
jwt_secret=$(read_secret "AppSettings:Secret: ")

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$connection_string" --project "$api_project" >/dev/null
dotnet user-secrets set "AppSettings:Secret" "$jwt_secret" --project "$api_project" >/dev/null

echo "User Secrets configured without printing secret values."
