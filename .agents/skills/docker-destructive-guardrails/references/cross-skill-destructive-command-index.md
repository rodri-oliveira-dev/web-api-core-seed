# Cross-Skill Destructive Command Index

A single-page index of destructive or irreversible Docker commands documented across all Docker skills, so agents and reviewers can see the full picture without hunting through every skill. Each row's detail lives in the owning skill — this table only tracks what exists and where. Every command below requires explicit user confirmation before running, except the narrow Tier 1 exception in `docker-destructive-guardrails` — see that skill's Core guidance for exactly which cases qualify.

| Command | What's lost | Owning skill |
|---|---|---|
| `docker rm` | Stopped container and its writable layer; low-risk removal (Tier 1 in the guardrail model) | `docker-destructive-guardrails` |
| `docker rm -f` | Container removed without graceful shutdown; un-persisted in-container state | `docker-destructive-guardrails` |
| `docker container prune` | All stopped containers on the host at once | `docker-destructive-guardrails` |
| `docker kill` | Container killed via SIGKILL with no grace period; always Tier 2 | `docker-destructive-guardrails` |
| `docker system prune` (esp. `-a`/`--volumes`) | Stopped containers, unused networks, dangling/all unused images, build cache, and (with `--volumes`) unused *anonymous* volume data | `docker-destructive-guardrails` |
| `docker rmi` / `docker image rm` | A specific image | `docker-destructive-guardrails` |
| `docker image prune -a` | All images not used by an existing container, including tagged ones | `docker-destructive-guardrails` |
| `docker network rm` | A specifically named network's configuration | `docker-destructive-guardrails` |
| `docker network prune` | All unused user-defined networks and their configuration | `docker-destructive-guardrails` |
| `docker builder prune` (esp. `-a`) | Build cache; with `-a`, also internal helper/frontend images and cache shared with other build outputs | `docker-destructive-guardrails` |
| `docker buildx rm` | A builder instance's configuration/state (not its build cache) | `docker-destructive-guardrails` |
| `docker context rm` | Local context configuration (endpoint, TLS references) for a Docker host | `docker-destructive-guardrails` |
| `docker volume rm` / `docker volume prune` (standalone, no Compose project in play) | Volume data, directly | `docker-destructive-guardrails` |
| `docker compose down -v` / `docker compose down --volumes` | Named volumes and their data (e.g. database state) | `docker-compose-patterns` |
| `docker volume rm` / `docker volume prune` (a Compose project's volumes) | Volume data, directly | `docker-compose-patterns` |
| `docker compose rm -v` | Anonymous volumes attached to removed containers | `docker-compose-patterns` |
| `sbx rm` / `sbx prune` | Sandbox containers, Git worktrees, state, and sandbox-scoped secrets; for a clone-mode sandbox, any unfetched commits too | `docker-sandboxes-lifecycle` |
| Docker Desktop destructive commands | Pending — see PR #14, not yet merged. Do not assume content until that skill ships. | *pending* |

## Notes

- This table is a routing aid, not a replacement for the owning skill's detail. Read the owning skill (`references/docker-cli-destructive-commands.md` in this skill, or the equivalent reference in `docker-compose-patterns`) before advising on or running any of these commands.
