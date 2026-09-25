#!/usr/bin/env bash
# Verify Compose configuration. Run from the project root.
# Usage: bash scripts/verify-compose.sh [--help]
set -euo pipefail

usage() {
    echo "Usage: bash scripts/verify-compose.sh [--help]"
    echo "Validates compose.yaml with docker compose config --quiet (no rendered configuration)."
}

if [[ "${1:-}" == "--help" && $# == 1 ]]; then
    usage
    exit 0
fi

if (( $# != 0 )); then
    usage >&2
    exit 2
fi

docker compose config --quiet
