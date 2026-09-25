# Verification Runbook: Destructive Command Guardrails

This skill documents behavioral policy rather than a generated artifact, so verification is manual review rather than running a validator against generated output. Run these checks whenever this skill or a skill it references is updated.

## 1. Generic command list matches current Docker CLI behavior

For each command in `references/docker-cli-destructive-commands.md` (`docker rm`, `docker container prune`, `docker kill`, `docker stop`, `docker system prune`, `docker rmi`/`docker image rm`, `docker image prune -a`, `docker network rm`, `docker network prune`, `docker builder prune`, `docker buildx rm`, `docker context rm`, standalone `docker volume rm`/`docker volume prune`), confirm the documented flags and behavior still match the installed Docker CLI's `--help` output:

```bash
for cmd in "rm" "container prune" "kill" "stop" "system prune" "rmi" "image prune" \
           "network rm" "network prune" "builder prune" "buildx rm" "context rm" \
           "volume rm" "volume prune"; do
  docker $cmd --help
done
```

Flag names, defaults, and what each flag deletes change occasionally between Docker releases. If `--help` output has drifted from what's documented, update `references/docker-cli-destructive-commands.md` and `SKILL.md`'s Core guidance section together.

## 2. Cross-skill index rows match each linked skill's actual guardrail content

For every row in `references/cross-skill-destructive-command-index.md` whose owning skill is not this one (currently `docker-compose-patterns` and `docker-sandboxes-lifecycle`), re-read that skill's actual destructive-command guidance and confirm:

- The command names in the index row still match what the owning skill documents (no renamed flags, no removed commands).

This is a manual check, not an automated one — nothing currently diffs the index against the owning skill's actual content, so drift only gets caught if this step is actually run. For the pending Docker Desktop row, confirm the PR referenced in `references/cross-skill-destructive-command-index.md` is still open and unmerged before leaving the "pending" note in place — update the row once that skill ships.

## 3. Tier 1 examples carry no Tier 2/3-style blocking-confirmation language

The container-lifecycle tiering model (`docker rm`, `docker rm -f`, `docker container prune`, `docker kill`) splits into Tier 1 (state-and-proceed, no blocking confirmation) and Tier 2 (same confirmation bar as the flat-rule commands). The automated `eval-checks.yaml` checks mostly only assert coarse existence (a "### Tier 1" heading exists, a "### Tier 2" heading exists, specific commands are mentioned) — grep has no concept of proximity or section-scoping in general, so most of this distinction must be checked by hand. One narrow exception is automated: `ddg-kill-not-listed-under-tier1` uses a cross-line `file_must_not_match` pattern to catch the specific, common mistake of `docker kill` text appearing between the Tier 1 and Tier 2 headings — that single coarse guard doesn't cover the rest of the distinction below, which still needs manual review:

- For every example listed under the Tier 1 heading in `SKILL.md` (container already stopped, or created/started by the agent itself this session, no known unpersisted state at risk, one specific identified container — e.g. `docker rm <stopped-container>`, `docker rm -f <agent's-own-just-created-test-container>`), confirm the surrounding text does NOT say to ask for confirmation or wait for the user before acting. It should describe stating what was done, not asking permission first.
- For every example listed under the Tier 2 heading, and for every flat-rule command (volumes, images, networks, builder/buildx, context, `docker system prune`), confirm the surrounding text DOES require explicit confirmation before running the command, with no exception carved out.
- Specifically confirm `docker kill` is documented only under Tier 2, never listed as a Tier 1 example — it has no stopped-container exception.
- Specifically confirm `docker container prune` is documented only under Tier 2, never listed as a Tier 1 example — it always sweeps every stopped container on the host, so it can never target "one specific, identified container" the way Tier 1 requires.
- Specifically confirm an unscoped sweep (e.g. `docker rm -f $(docker ps -aq)`, "force remove all containers") is documented as Tier 2 even when every example nearby is Tier 1 — scope (single named container vs. sweep), not container state alone, is what should gate the tier for a sweep.

If any Tier 1 example reads like it requires the user to wait for permission, or any Tier 2/3 example reads like it can proceed without confirmation, fix the prose before merging — the automated checks will not catch this misplacement.

## 4. `docker network rm -f` and `docker context rm -f` are documented with distinct semantics

These two `-f` flags do different things and must not be conflated. Read both entries in `references/docker-cli-destructive-commands.md` side by side and confirm they still use language that makes the difference unambiguous (one suppresses a "not found" error only and does not override in-use protection, the other genuinely forces removal of an in-use resource). A reader skimming just the `-f` flag name should not come away assuming both behave the same way.

## 5. Standalone volume content does not contradict `docker-compose-patterns`'s Compose-scoped content

`docker volume rm`/`docker volume prune` now have two homes: this skill covers the standalone (non-Compose) case, and `docker-compose-patterns` covers the Compose-context case. Before merging changes to either skill's volume guidance:

- Confirm this skill's guidance (in `SKILL.md` and `references/docker-cli-destructive-commands.md`) is scoped to "no Compose project in play" and doesn't duplicate or override `docker-compose-patterns`'s Compose-specific advice (e.g. `docker compose down -v` behavior, anonymous volumes tied to Compose-managed containers).
- Confirm `docker-compose-patterns`'s `SKILL.md` "Destructive commands" section is narrowed to the Compose-context case only, and its cross-reference to this skill (for the standalone case) is accurate and not circular.
- Confirm neither skill claims sole ownership of `docker volume rm`/`docker volume prune` in a way that would make an agent miss the other skill's applicable case (Compose project present vs. absent).

## 6. No destructive command is documented as safe without confirmation

Grep this skill's files for the command list and confirm every one is paired with a confirmation requirement, not a "safe to run automatically" statement:

```bash
grep -n "docker rm -f\|docker container prune\|docker kill\|docker system prune\|docker rmi\|docker image rm\|docker image prune -a\|docker network rm\|docker network prune\|docker builder prune\|docker buildx rm\|docker context rm\|docker volume rm\|docker volume prune" \
  skills/docker-destructive-guardrails/SKILL.md \
  skills/docker-destructive-guardrails/references/*.md
```

Every match should appear alongside guidance to state what will be lost and get explicit confirmation — never alongside language suggesting the command is safe to run unprompted.

## Example dialogues

### Bad: agent runs the destructive command unprompted

> **User:** Docker's taking up a lot of disk space, can you clean things up?
>
> **Agent:** Sure, running `docker system prune -a --volumes` now... Done, freed 12GB.

This is wrong even though it "worked": the agent deleted every unused image and every unused anonymous volume without saying what would be removed or waiting for the user to agree.

### Good: agent states impact and waits for confirmation

> **User:** Docker's taking up a lot of disk space, can you clean things up?
>
> **Agent:** I can reclaim space, but there are a few options with different levels of risk:
>
> - `docker container prune` — removes stopped containers only. Low risk.
> - `docker image prune` — removes dangling (untagged) images only. Low risk.
> - `docker image prune -a` — removes *all* images not referenced by any container, running or stopped, including tagged ones you might want to reuse. You'd need to re-pull or rebuild them later.
> - `docker system prune -a --volumes` — the most aggressive option. This also deletes every unused *anonymous* volume (named volumes, like database data, aren't touched — those need a separate `docker volume rm`).
>
> Want me to start with the low-risk options (containers + dangling images), or do you want the more aggressive cleanup?

The agent names what each option removes, orders them by risk, and does not run anything until the user picks one.
