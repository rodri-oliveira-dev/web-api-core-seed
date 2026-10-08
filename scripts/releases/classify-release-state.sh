#!/usr/bin/env bash
# Read the (possibly paginated) GitHub releases JSON arrays from stdin.
# Output exactly one state: missing, published, draft, or invalid.
set -euo pipefail

tag="${1:?Usage: classify-release-state.sh <tag>}"

jq -s -r --arg tag "$tag" '
  [ .[][] | select(.tag_name == $tag) ] |
  if length == 0 then
    "missing"
  elif length != 1 then
    "invalid"
  elif .[0].draft == true or .[0].published_at == null then
    "draft"
  elif .[0].draft == false
    and .[0].prerelease == false
    and (.[0].published_at | type) == "string"
    and (.[0].published_at | length) > 0 then
    "published"
  else
    "invalid"
  end
'
