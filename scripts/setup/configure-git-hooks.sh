#!/usr/bin/env sh
set -eu

expected_hooks_path=".githooks"
required_hook="pre-push"
check=false
force=false

usage() {
  cat <<'USAGE'
Usage: scripts/setup/configure-git-hooks.sh [--check] [--force]

Configures the local repository with core.hooksPath=.githooks.

Options:
  --check   Verify configuration without changing it.
  --force   Replace an existing local core.hooksPath value.
  -h, --help

To remove:
  git config --local --unset core.hooksPath
USAGE
}

while [ "$#" -gt 0 ]; do
  case "$1" in
    --check)
      check=true
      ;;
    --force)
      force=true
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "Unknown option: $1" >&2
      usage >&2
      exit 2
      ;;
  esac
  shift
done

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "Error: current directory is not inside a Git repository." >&2
  exit 1
fi

repo_root="$(git rev-parse --show-toplevel)"
hooks_dir="$repo_root/$expected_hooks_path"
hook_path="$hooks_dir/$required_hook"
current_hooks_path="$(git -C "$repo_root" config --local --get core.hooksPath || true)"

validate_hook_file() {
  if [ ! -d "$hooks_dir" ]; then
    echo "Error: hooks directory not found: $expected_hooks_path" >&2
    return 1
  fi

  if [ ! -f "$hook_path" ]; then
    echo "Error: required hook not found: $expected_hooks_path/$required_hook" >&2
    return 1
  fi

  case "$(uname -s 2>/dev/null || printf unknown)" in
    Linux|Darwin|FreeBSD|OpenBSD|NetBSD)
      if [ ! -x "$hook_path" ]; then
        if [ "$check" = true ]; then
          echo "Error: hook is not executable: $expected_hooks_path/$required_hook" >&2
          return 1
        fi

        chmod +x "$hook_path"
        echo "Executable bit applied to $expected_hooks_path/$required_hook."
      fi
      ;;
  esac
}

if [ "$check" = true ]; then
  status=0

  if [ "$current_hooks_path" = "$expected_hooks_path" ]; then
    echo "OK: core.hooksPath is configured as $expected_hooks_path."
  else
    echo "Error: core.hooksPath is '${current_hooks_path:-<unset>}', expected '$expected_hooks_path'." >&2
    status=1
  fi

  if ! validate_hook_file; then
    status=1
  fi

  exit "$status"
fi

validate_hook_file

if [ -z "$current_hooks_path" ]; then
  git -C "$repo_root" config --local core.hooksPath "$expected_hooks_path"
  echo "Configured local core.hooksPath=$expected_hooks_path."
  echo "Remove with: git config --local --unset core.hooksPath"
  exit 0
fi

if [ "$current_hooks_path" = "$expected_hooks_path" ]; then
  echo "Local core.hooksPath is already configured as $expected_hooks_path."
  echo "Remove with: git config --local --unset core.hooksPath"
  exit 0
fi

if [ "$force" != true ]; then
  echo "Error: local core.hooksPath already points to '$current_hooks_path'." >&2
  echo "Re-run with --force only if you want to replace it with '$expected_hooks_path'." >&2
  exit 1
fi

git -C "$repo_root" config --local core.hooksPath "$expected_hooks_path"
echo "Replaced local core.hooksPath."
echo "Previous value: $current_hooks_path"
echo "New value: $expected_hooks_path"
echo "Remove with: git config --local --unset core.hooksPath"
