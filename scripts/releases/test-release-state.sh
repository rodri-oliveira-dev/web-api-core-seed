#!/usr/bin/env bash
# Regression test: authenticated GitHub API lists draft releases (and assets).
# Never treat an uploaded but unpublished release as a completed release.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
classifier="$root/scripts/releases/classify-release-state.sh"

assert_state() {
  local expected="$1" payload="$2" label="$3" actual
  actual="$(printf '%s\n' "$payload" | bash "$classifier" v2.0.0)"
  if [[ "$actual" != "$expected" ]]; then
    echo "FAILED $label: expected $expected, got $actual" >&2
    exit 1
  fi
  echo "PASS $label => $actual"
}

assert_state missing '[]' 'release absent'
assert_state missing '[{"tag_name":"v1.0.0-legacy","draft":false,"prerelease":false,"published_at":"2026-10-07T00:00:00Z"}]' 'unrelated release'
assert_state published '[{"tag_name":"v2.0.0","draft":false,"prerelease":false,"published_at":"2026-10-08T00:00:00Z","assets":[{"name":"template.nupkg"}]}]' 'published release'
assert_state draft '[{"tag_name":"v2.0.0","draft":true,"prerelease":false,"published_at":null,"assets":[{"name":"template.nupkg"},{"name":"SHA256SUMS.txt"}]}]' 'draft release with already uploaded assets'
assert_state draft '[{"tag_name":"v2.0.0","draft":false,"prerelease":false,"published_at":null,"assets":[{"name":"template.nupkg"}]}]' 'unpublished despite draft=false'
assert_state draft '[{"tag_name":"v2.0.0","draft":true,"prerelease":false,"published_at":"2026-10-08T00:00:00Z"}]' 'draft flag overrides timestamp'
assert_state invalid '[{"tag_name":"v2.0.0","draft":false,"prerelease":true,"published_at":"2026-10-08T00:00:00Z"}]' 'prerelease not final'
assert_state invalid '[{"tag_name":"v2.0.0","draft":false,"prerelease":false,"published_at":""}]' 'empty published timestamp'
assert_state invalid '[{"tag_name":"v2.0.0","draft":true,"prerelease":false,"published_at":null},{"tag_name":"v2.0.0","draft":false,"prerelease":false,"published_at":"2026-10-08T00:00:00Z"}]' 'ambiguous duplicate releases'
assert_state published '[{"tag_name":"v1.0.0-legacy","draft":false,"prerelease":false,"published_at":"2026-10-07T00:00:00Z"}]
[{"tag_name":"v2.0.0","draft":false,"prerelease":false,"published_at":"2026-10-08T00:00:00Z"}]' 'multiple REST pages'

echo "Release state guard regression tests passed"
