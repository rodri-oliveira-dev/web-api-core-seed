#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
work_dir="$(mktemp -d)"
api_pid=""
package_file=""

cleanup() {
  if [[ -n "$api_pid" ]]; then
    kill "$api_pid" 2>/dev/null || true
    wait "$api_pid" 2>/dev/null || true
  fi
  if [[ -n "$package_file" ]]; then
    dotnet new uninstall "$package_file" >/dev/null 2>&1 || true
  fi
  rm -rf "$work_dir"
}
trap cleanup EXIT

mkdir -p "$work_dir/packages" "$work_dir/dotnet-home" "$work_dir/generated"
export DOTNET_CLI_HOME="$work_dir/dotnet-home"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=true

echo "== Pack the template NuGet =="
dotnet pack "$repo_root/template-pack/WebApiCoreSeed.Templates.csproj" --configuration Release --output "$work_dir/packages" --nologo

shopt -s nullglob
packages=("$work_dir"/packages/RodriOliveira.WebApiCoreSeed.Templates.*.nupkg)
if [[ "${#packages[@]}" != 1 ]]; then
  echo "Expected exactly one .nupkg, found ${#packages[@]}" >&2
  exit 1
fi
package_file="${packages[0]}"

echo "== Validate NuGet package contents (deny repository-only assets) =="
python3 - "$package_file" <<'PY'
import sys
from zipfile import ZipFile

with ZipFile(sys.argv[1]) as pkg:
    files = [item.filename for item in pkg.infolist()]
required = {
    "content/.template.config/template.json",
    "content/WebApiCoreSeed.slnx",
    "content/README.md",
    "content/.env.local.example",
    "content/.gitignore",
    "content/compose.yaml",
    "content/src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj",
    "content/tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj",
}
missing = required.difference(files)
if missing:
    print("Packed projects:", sorted(item for item in files if item.endswith(".csproj"))[:25], file=sys.stderr)
    raise SystemExit(f"Missing packaged files: {sorted(missing)}")
for filename in files:
    if filename.startswith(("content/.github/", "content/.sdd/", "content/.agents/",
                            "content/template-pack/", "content/.git/")):
        raise SystemExit(f"Repository-only path leaked into template: {filename}")
    if "/bin/" in filename or "/obj/" in filename:
        raise SystemExit(f"Build artifacts leaked into template: {filename}")
print(f"Package layout passed: {len(files)} entries")
PY

echo "== Install and instantiate the packaged template =="
dotnet new install "$package_file"
dotnet new webapi-seed --name SampleApi --output "$work_dir/generated" --no-restore

generated="$work_dir/generated"
solution="$generated/SampleApi.slnx"
api_project="$generated/src/SampleApi.Api/SampleApi.Api.csproj"
unit_project="$generated/tests/SampleApi.UnitTests/SampleApi.UnitTests.csproj"
integration_project="$generated/tests/SampleApi.IntegrationTests/SampleApi.IntegrationTests.csproj"
test -f "$solution"
test -f "$api_project"
test -f "$unit_project"
test -f "$integration_project"
test -f "$generated/.env.local.example"
test -f "$generated/README.md"

for forbidden in .github .sdd .agents template-pack .template.config AGENTS.md CHANGELOG.md; do
  if [[ -e "$generated/$forbidden" ]]; then
    echo "Repository-only file present in generated output: $forbidden" >&2
    exit 1
  fi
done

python3 - "$generated" <<'PY'
import sys
from pathlib import Path

root = Path(sys.argv[1])
source = "WebApiCoreSeed"
for path in [root / "SampleApi.slnx", root / "Dockerfile", root / "scripts/docker/apply-migrations.sh"]:
    data = path.read_text(encoding="utf-8")
    if source in data:
        raise SystemExit(f"Unreplaced project identifier in: {path}")

project = root / "src/SampleApi.Api/SampleApi.Api.csproj"
text = project.read_text(encoding="utf-8")
if "c52dbe85-d94e-4cc2-9856-529f22712174" in text:
    raise SystemExit("Template retained the source project's UserSecretsId")
if "<UserSecretsId>" not in text:
    raise SystemExit("The generated API is missing its UserSecretsId")
print("Identifier replacement and unique UserSecretsId passed")
PY

echo "== Restore, build and test generated solution =="
dotnet restore "$solution"
dotnet build "$solution" --configuration Release --no-restore
dotnet test "$unit_project" --configuration Release --no-build
dotnet test "$integration_project" --configuration Release --no-build

echo "== Start generated API and probe liveness =="
port="$(python3 - <<'PY'
import socket
with socket.socket() as sock:
    sock.bind(("127.0.0.1", 0))
    print(sock.getsockname()[1])
PY
)"
(
  cd "$generated"
  ASPNETCORE_ENVIRONMENT=Development \
  ASPNETCORE_URLS="http://127.0.0.1:$port" \
  ConnectionStrings__DefaultConnection="Server=127.0.0.1;Database=SampleApi;Integrated Security=true;TrustServerCertificate=true;" \
  AppSettings__Secret="local-template-smoke-test-only-secret-123456789" \
  AppSettings__Emissor="SampleApi" \
  AppSettings__ValidoEm="http://127.0.0.1:$port" \
  RedisCacheSettings__Enabled="false" \
  OpenTelemetry__Enabled="false" \
  SeqSettings__FilePath="" \
  dotnet run --project "$api_project" --configuration Release --no-build --no-launch-profile >"$work_dir/api.log" 2>&1
) &
api_pid="$!"

healthy=false
for _ in $(seq 1 50); do
  if curl --silent --fail --max-time 2 "http://127.0.0.1:$port/health/live" >/dev/null; then
    healthy=true
    break
  fi
  if ! kill -0 "$api_pid" 2>/dev/null; then
    break
  fi
  sleep 1
done
if [[ "$healthy" != "true" ]]; then
  echo "Generated API did not respond on /health/live:" >&2
  tail -80 "$work_dir/api.log" >&2
  exit 1
fi
echo "Generated API liveness probe passed"

echo "== Uninstall package =="
dotnet new uninstall "$package_file"
echo "Template smoke test passed"
