# Release operations — v2.0.1

[Release notes](v2.0.1.md) | [README](../../README.md) | [Português](release-checklist.pt-BR.md)

## Release mechanics

The `release-v2.yml` workflow executes on the **merge commit pushed to `main`** (and supports `workflow_dispatch` for safe retries **only when manually dispatched from `main`**). It does not publish on pull requests or other branches. Its **read-only `build-verify` job** checks the exact commit's quality gates, packs the template and runs its smoke suite without a write-capable token. The separate **`publish` job** downloads the verified package and notes, creates the release with narrowly scoped write permissions, and verifies the public assets. Actions are pinned to full commit SHAs. The quality-gate polling shares a single 25-minute deadline, and any failed, skipped, neutral or stale gate is rejected.

1. Check for an existing `v2.0.1` GitHub Release or tag; never overwrite, delete or move an existing tag.
2. Require `ci`, `codeql`, and `template-smoke` to conclude **success on that exact commit**. `ci` also enforces the SonarCloud Quality Gate in trusted contexts.
3. Run `dotnet pack` to create `RodriOliveira.WebApiCoreSeed.Templates.2.0.1.nupkg`.
4. Run `TEMPLATE_PACKAGE=<absolute .nupkg path> bash scripts/templates/smoke-test.sh`; this tests the **prebuilt asset**, not an independently repacked package.
5. Generate `SHA256SUMS.txt`; publish GitHub Release `v2.0.1` on the validated commit with notes from `docs/releases/v2.0.1.md` and both artifacts.
6. Download the uploaded assets, check the SHA-256 and confirm the tag points at the verified source commit.

**Idempotency and failed uploads:** the preflight enumerates GitHub Releases (including authenticated draft entries) and classifies them as missing, published, draft or invalid. A **draft release is an error**, even when its `.nupkg` and checksum assets are already present: complete or remove the interrupted draft manually before retrying. The workflow never treats draft assets as a successful publication. Published releases are verified again via their explicit `draft=false`, `prerelease=false` and non-empty `published_at` metadata **before downloading assets**; only then is publication considered complete. Existing tags are never moved. Later `main` pushes do not move `v2.0.1`. The state parser has regression fixtures in `scripts/releases/test-release-state.sh` and runs in CI.

**Version alignment:** package version is `2.0.1`, matching the GitHub Release and documentation. The preserved legacy tag remains separate.

## Verify manually after publication

```bash
gh release view v2.0.1 --repo rodri-oliveira-dev/web-api-core-seed
gh release download v2.0.1 --repo rodri-oliveira-dev/web-api-core-seed --dir ./release-check
cd release-check
sha256sum --check SHA256SUMS.txt
dotnet new install ./RodriOliveira.WebApiCoreSeed.Templates.2.0.1.nupkg
dotnet new webapi-seed -n SampleApi
```

**If the release workflow fails**, inspect its GitHub Actions run; do not manually create/retag `v2.0.1` without understanding the blocker. The v2.0.1 workflow does not modify the historical v2.0.0 tracking issue. Its release is complete only after checking the published assets.

## Repository About / branding (requires repository administration)

The GitHub Actions `GITHUB_TOKEN` cannot edit repository metadata requiring `Administration: write` via its normal configurable permissions. After publication, update the repository's **About** section with administrator privileges:

- **Description:** `Reusable .NET 10 ASP.NET Core Web API seed and dotnet new template — modular architecture, Identity, EF Core, SQL Server, Redis and OpenTelemetry.`
- **Website:** `https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.1`
- **Topics:** `dotnet`, `dotnet-10`, `aspnetcore`, `webapi`, `dotnet-template`, `clean-architecture`, `hexagonal-architecture`, `ef-core`, `redis`, `opentelemetry`, `docker`
- Replace obsolete topics `netcore31`, `iprate`, `datasul`; add a release badge/link if desired.

## Licensing and publication rights

At release preparation, no `LICENSE` file exists. Before advertising the project as open source or redistributing third-party code, the owner must select a license and verify provenance. **Do not invent or apply a license without that decision.** A public GitHub Release without a license is a public source distribution, not a grant of open-source rights.

## Announcing the patch release

Once the release is visible and checksum passes, link [release v2.0.1](https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.1) in the repository README, GitHub About section and an optional professional announcement. Point contributors to `CONTRIBUTING.md`, report security concerns privately via `SECURITY.md`, and mention that NuGet.org publication is **not** included.

## Post-release follow-up

If artifact or tests fail, use a new patch version (for example `v2.0.2`) rather than mutating the released tag/assets. Apply repo tag protection/immutability in repository settings if available; the workflow itself never force-updates a tag.
