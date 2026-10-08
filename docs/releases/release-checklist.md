# Release operations — v2.0.0

[Release notes](v2.0.0.md) | [README](../../README.md) | [Português](release-checklist.pt-BR.md)

## Release mechanics

The `release-v2.yml` workflow executes on the **merge commit pushed to `main`** (and supports `workflow_dispatch` for safe retries). It does not publish on pull requests or branches.

1. Check for an existing `v2.0.0` GitHub Release or tag; never overwrite, delete or move an existing tag.
2. Require `ci`, `codeql`, and `template-smoke` to conclude **success on that exact commit**. `ci` also enforces the SonarCloud Quality Gate in trusted contexts.
3. Run `dotnet pack` to create `RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg`.
4. Run `TEMPLATE_PACKAGE=<absolute .nupkg path> bash scripts/templates/smoke-test.sh`; this tests the **prebuilt asset**, not an independently repacked package.
5. Generate `SHA256SUMS.txt`; publish GitHub Release `v2.0.0` on the validated commit with notes from `docs/releases/v2.0.0.md` and both artifacts.
6. Download the uploaded assets, check the SHA-256 and confirm the tag points at the verified source commit.

**Idempotency and failed uploads:** the preflight enumerates GitHub Releases (including authenticated draft entries) and classifies them as missing, published, draft or invalid. A **draft release is an error**, even when its `.nupkg` and checksum assets are already present: complete or remove the interrupted draft manually before retrying. The workflow never treats draft assets as a successful publication and never closes #23 in that case. Published releases are verified again via their explicit `draft=false`, `prerelease=false` and non-empty `published_at` metadata **before downloading assets**; only then can #23 close. Existing tags are never moved. Later `main` pushes do not move `v2.0.0`. The state parser has regression fixtures in `scripts/releases/test-release-state.sh` and runs in CI.

**Version alignment:** package version is `2.0.0`, matching the GitHub Release and documentation. The preserved legacy tag remains separate.

## Verify manually after publication

```bash
gh release view v2.0.0 --repo rodri-oliveira-dev/web-api-core-seed
gh release download v2.0.0 --repo rodri-oliveira-dev/web-api-core-seed --dir ./release-check
cd release-check
sha256sum --check SHA256SUMS.txt
dotnet new install ./RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg
dotnet new webapi-seed -n SampleApi
```

**If the release workflow fails**, inspect its GitHub Actions run; do not manually create/retag `v2.0.0` without understanding the blocker. The release workflow closes #23 itself **only after downloading and verifying published assets**. The PR deliberately references #23 without an auto-closing keyword, preventing premature closure if publication fails.

## Repository About / branding (requires repository administration)

The GitHub Actions `GITHUB_TOKEN` cannot edit repository metadata requiring `Administration: write` via its normal configurable permissions. After publication, update the repository's **About** section with administrator privileges:

- **Description:** `Reusable .NET 10 ASP.NET Core Web API seed and dotnet new template — modular architecture, Identity, EF Core, SQL Server, Redis and OpenTelemetry.`
- **Website:** `https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.0`
- **Topics:** `dotnet`, `dotnet-10`, `aspnetcore`, `webapi`, `dotnet-template`, `clean-architecture`, `hexagonal-architecture`, `ef-core`, `redis`, `opentelemetry`, `docker`
- Replace obsolete topics `netcore31`, `iprate`, `datasul`; add a release badge/link if desired.

## Licensing and publication rights

At release preparation, no `LICENSE` file exists. Before advertising the project as open source or redistributing third-party code, the owner must select a license and verify provenance. **Do not invent or apply a license without that decision.** A public GitHub Release without a license is a public source distribution, not a grant of open-source rights.

## Announcing the relaunch

Once the release is visible and checksum passes, link [release v2.0.0](https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.0) in the repository README, GitHub About section and an optional professional announcement. Point contributors to `CONTRIBUTING.md`, report security concerns privately via `SECURITY.md`, and mention that NuGet.org publication is **not** included.

## Post-release follow-up

If artifact or tests fail, use a new patch version (for example `v2.0.1`) rather than mutating the released tag/assets. Apply repo tag protection/immutability in repository settings if available; the workflow itself never force-updates a tag.
