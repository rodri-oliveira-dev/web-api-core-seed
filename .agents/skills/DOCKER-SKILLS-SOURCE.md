# Docker Skills Source

The Docker-related agent skills below use `docker/skills` as their pinned provenance baseline:

- `docker-build-strategies`
- `docker-compose-patterns`
- `docker-destructive-guardrails`

Source repository: https://github.com/docker/skills
Source revision: `ddbf34bfd8be2fed3fe69dddd6c7590b42d45320`
License: Apache License 2.0
Vendored license: `.agents/skills/licenses/docker-skills-APACHE-2.0.txt`

The initial import came from that exact revision. The copies in this repository include review-driven local hardening and documentation corrections on top of the pinned baseline, so they are intentionally not byte-for-byte identical to upstream. Keep those local deltas minimal and review them again whenever the pinned revision advances.

Repository-specific rules in `AGENTS.md` and the active SDD remain authoritative. The Docker skills provide specialized guidance for image builds, Compose configuration and destructive Docker operations.

Updates are manual: review upstream changes, reconcile the documented local hardening, and advance the pinned source revision intentionally.
