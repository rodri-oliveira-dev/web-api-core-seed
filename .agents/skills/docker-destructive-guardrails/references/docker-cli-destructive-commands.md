# Docker CLI Destructive Commands

Detailed breakdown of the generic Docker CLI commands this skill owns. For each command: what it deletes, the exact flags that widen the blast radius, and the safer/scoped alternative to reach for first.

Note: there is no top-level `docker prune` alias. Each resource type has its own subcommand — `docker container prune`, `docker image prune`, `docker network prune`, `docker volume prune`, `docker system prune`, and `docker builder prune` (a true alias of `docker buildx prune` — `docker builder prune --help` prints `Usage: docker buildx prune`). Don't assume a bare `docker prune` exists.

## Container lifecycle: `docker rm`, `docker rm -f`, `docker container prune`, `docker kill`

These four commands remove or kill containers, but each widens the blast radius differently. See `SKILL.md`'s Core guidance for the Tier 1/Tier 2 confirmation policy that applies to each — this section covers the exact flag behavior only.

```
docker rm <container>              # stopped container only, fails on a running one
docker rm -f <container>           # widens to a running container too, via SIGKILL
docker rm -f -v <container>        # + removes anonymous volumes attached to it
docker container prune             # removes all stopped containers on the host; no flag widens this to running ones
docker kill <container>            # SIGKILL (or --signal <sig>) with no grace period, always
```

- `docker rm` (no `-f`) on a stopped container is a normal, low-risk, irreversible removal — the container was already stopped, so there's no live process being interrupted.
- `-f`/`--force` widens `docker rm` to also work on a running container, sending it SIGKILL with no graceful shutdown; adding `-v` additionally removes any anonymous volumes attached to that container.
- `docker container prune` only ever removes stopped containers — no flag widens its scope to running ones.
- `docker kill` always sends SIGKILL (or a custom signal via `-s`/`--signal`) with no grace period; it only applies to running containers, so there's no "already stopped, low-risk" case the way there is for `docker rm`.

**Safer alternative**: target one specific, identified container by name or ID rather than sweeping every container on the host.

```bash
docker rm <name>          # only the container that's actually stuck or finished
docker rm -f <name>       # only if it's genuinely stuck and won't respond to docker stop
```

If several containers appear stuck, list them (`docker ps -a`) and confirm with the user which ones are safe to remove before force-removing more than one.

## `docker stop`

```
docker stop <container>                # SIGTERM, then SIGKILL after the timeout
docker stop -t <seconds> <container>   # custom grace period before SIGKILL
docker stop -s <signal> <container>    # custom initial signal instead of SIGTERM
```

- Does not remove the container — it can be restarted afterward with `docker start`. This is why it sits outside the Tier 1/Tier 2 removal model in `SKILL.md`'s Core guidance, even though Docker Agent's runtime safety classifier still flags it as low-risk destructive.
- Sends `SIGTERM` (or a custom signal via `-s`/`--signal`) and waits `-t`/`--timeout` seconds (default 10) before force-killing with `SIGKILL`. Any unpersisted in-container state (e.g. an in-memory cache, an unflushed write) is lost at that point, the same as with `docker kill`.

**Safer alternative**: this is already the lower-risk alternative to `docker kill` for a graceful shutdown — prefer it over `docker kill` unless the container is unresponsive to `SIGTERM`.

## `docker system prune`

```
docker system prune           # stopped containers, unused networks, dangling images, build cache
docker system prune -a        # + all images not referenced by any container, running or stopped
docker system prune --volumes # + unused anonymous volumes and their data
docker system prune -a --volumes  # everything above, combined
```

- Without flags, this already deletes stopped containers permanently — any container-local state not committed to an image or volume is gone.
- `-a`/`--all` widens image deletion from "dangling only" to "any image not referenced by any container, running or stopped," including tagged images that were deliberately pulled or built.
- `--volumes` prunes unused *anonymous* volumes only — named volumes (e.g. database data mounted via a `volumes:` entry) are never touched by `system prune`. Deleting a named volume requires a separate, explicit `docker volume rm` and its own confirmation.
- `docker system prune -a --volumes` is still the widest-blast-radius form of this command and must never be run without first listing what `docker system df` reports would be reclaimed, and getting explicit confirmation.

**Safer alternative**: inspect first, then prune narrowly.

```bash
docker system df                # see what's actually consuming space, by category
docker container prune          # stopped containers only
docker image prune              # dangling (untagged) images only, no -a
docker builder prune            # build cache not tied to existing images
```

Only add `-a` to `docker image prune` or `--volumes` to any prune command after the user has confirmed the specific resources named are safe to delete.

## `docker rmi` / `docker image rm`

```
docker rmi <image>              # or: docker image rm <image> — same command, two names
docker rmi -f <image>           # force removal
docker rmi --no-prune <image>   # skip removing now-untagged parent image layers
```

- `docker rmi` and `docker image rm` are the same command under two names.
- `-f`/`--force`'s own `--help` text only says "Force removal of the image." It's commonly understood to override protection for an image with multiple tags or one referenced by a stopped (not running) container, but that exact override behavior isn't spelled out in the `--help` text itself — treat it as documented-by-convention rather than confirmed, and get explicit confirmation before using `-f` on an image that might still be tagged or referenced elsewhere.
- `--no-prune` skips the usual cleanup of now-untagged parent image layers.

**Safer alternative**: remove without `-f` first; only add `-f` after confirming with the user that no other tag or stopped container still needs the image.

## `docker image prune -a`

```
docker image prune       # dangling (untagged) images only
docker image prune -a    # any image not used by an existing container
```

- `-a`/`--all` removes tagged images too, not just dangling ones. An image that was pulled for later use, or built as a base for future work, is deleted if no container — running or stopped — still references it.
- Removed images must be re-pulled or rebuilt, which can be slow for large images or on a metered connection.

**Safer alternative**: run without `-a` first, and add a filter for age or label if more aggressive cleanup is genuinely needed.

```bash
docker image prune                              # dangling only, no risk to tagged images
docker image prune --filter "until=168h"        # images older than 7 days, still requires confirmation before -a
```

## `docker network rm`

```
docker network rm <name>
docker network rm -f <name>
```

- Only removes the specifically named network(s) passed as arguments — there's no sweep behavior the way `docker network prune` has.
- `-f`/`--force` on `docker network rm` only suppresses a "network does not exist" error if the network is already gone — it does **not** override in-use protection. A network still attached to a running container is not force-removed by `network rm -f`. This is different from `docker context rm -f` below, which genuinely forces removal of an in-use context — don't assume both `-f` flags behave the same way just because they share a name.

**Safer alternative**: this is already the scoped, targeted alternative to `docker network prune` below — confirm the network name with the user before running it.

## `docker network prune`

```
docker network prune
```

- Removes every user-defined bridge/overlay network not currently attached to a running container. Networks with static IP assignments, custom subnets, or `external: true` references from stopped-but-not-deleted Compose projects are deleted along with that configuration.

**Safer alternative**: remove a specific network by name with `docker network rm <name>` above, once confirmed unused.

## `docker builder prune`

```
docker builder prune           # cache not associated with any existing image
docker builder prune -a        # + internal helper/frontend images and cache shared with other build outputs
```

- Without `-a`/`--all`, this is relatively low-risk — it only removes cache that no current image depends on.
- `-a`/`--all` also removes build cache beyond what's dangling — including BuildKit's internal helper/frontend images and cache shared with other build outputs. Treat a full `-a` wipe as forcing a cold rebuild for any consumer of that cache (local, CI, teammates using a shared cache backend).
- `docker builder prune` is a true alias of `docker buildx prune` (see the top note) — the same cache is affected either way.

**Safer alternative**: run without `-a` unless the user has confirmed a full cache wipe is worth the next full rebuild.

## `docker buildx rm`

```
docker buildx rm <builder>
docker buildx rm --all-inactive
docker buildx rm --keep-daemon <builder>
docker buildx rm --keep-state <builder>
```

- Removes a builder *instance* — its configuration/registration and, unless kept, its daemon/state — a different resource from the build cache covered by `docker builder prune` above.
- `--all-inactive` widens this to every inactive builder at once, not just the one named.
- `--keep-daemon`/`--keep-state` reduce what's deleted, but the builder's registration/reference itself is still removed either way.

**Safer alternative**: remove one named builder at a time rather than `--all-inactive`, and confirm with the user which builder(s) are no longer needed.

## `docker context rm`

```
docker context rm <name>
docker context rm -f <name>
```

- Deletes the local context entry (endpoint URL, TLS material references, metadata) for connecting to a Docker host. It does not delete remote resources on that host, but it does delete the local configuration for reaching it.
- `-f`/`--force` on `docker context rm` genuinely forces removal even if the context is currently in use (e.g. it's the active context) — see `docker network rm` above, whose same-named flag behaves differently.
- If the context was set up interactively (e.g. with a one-time token, a manually copied TLS bundle, or a since-rotated credential), it may not be trivially re-creatable.

**Safer alternative**: use `docker context update <name>` to fix a misconfigured endpoint or TLS setting instead of deleting and recreating the context.

## `docker volume rm`

Standalone case only — no Compose project in play. If a `compose.yaml` is present and the volume belongs to that project, use `docker-compose-patterns` instead (`docker compose down -v`, Compose-managed anonymous volumes); this section does not duplicate or override that guidance.

```
docker volume rm <name>
docker volume rm -f <name>
```

- `docker volume rm`'s own `--help` text says plainly: "You cannot remove a volume that is in use by a container" — without force, it fails safely on an in-use volume.
- Whether `-f`/`--force` truly force-removes an in-use volume, or merely suppresses a "no such volume" error the way `docker network rm -f` does, is **not** disambiguated by `--help` text alone. Treat this as an open question rather than asserting either behavior, and get explicit confirmation before using `-f` on a volume that might still be attached to a container.

**Safer alternative**: run without `-f` first; if it fails because the volume is in use, identify and stop/remove the container using it (with its own confirmation) before retrying, rather than reaching for `-f`.

## `docker volume prune`

Standalone case only — no Compose project in play. For Compose-managed anonymous volumes, use `docker-compose-patterns` (`docker compose down -v`) instead.

```
docker volume prune              # unused anonymous volumes only
docker volume prune -a           # + unused named volumes too
```

- Default scope (no flags) is unused *anonymous* volumes only — this matches `docker system prune --volumes`'s scope documented above.
- `-a`/`--all` widens this to unused named volumes too, confirmed by its own help text: "Remove all unused volumes, not just anonymous ones." A named volume holding database state that just isn't currently attached to a running container is deleted by `-a` just as readily as a throwaway anonymous one.

**Safer alternative**: run without `-a` first, and confirm with the user which named volumes (if any) are genuinely safe to delete before adding `-a`.
