# Release operations - v2.0.1

[Release notes](v2.0.1.md) | [README](../../README.md) | [Português](release-checklist.pt-BR.md)

## Publishing contract

The existing `.github/workflows/release-v2.yml` workflow runs on pushes to `main` and can be retried manually from `main`. It does not publish from pull requests. Version **2.0.1** is published as a **GitHub Release asset** only; no NuGet.org push is configured.

1. Merge the release preparation PR after `ci`, `codeql`, and `template-smoke` pass.
2. On the resulting `main` SHA, wait for those same three required push checks to pass. Any failure or stale/neutral result blocks publication.
3. Check that neither tag `v2.0.1` nor an inconsistent/draft release is present. Never overwrite or move the published `v2.0.0` tag.
4. Pack the `2.0.1` template, verify package/version alignment and inspect the packaged `global.json` (`10.0.403`) and `Dockerfile` (`sdk:10.0.403`, `aspnet:10.0.14`).
5. Install and smoke-test the **exact** versioned `.nupkg` that will be uploaded, including the generated application's build, tests and live health probe.
6. Publish release `v2.0.1` with notes from `docs/releases/v2.0.1.md`, the verified NuGet template file and `SHA256SUMS.txt`.
7. Download the public assets, validate their checksum and verify that `v2.0.1` points to the exact tested commit.

**Idempotency:** An existing published `v2.0.1` is reverified, not recreated. Draft/invalid releases or conflicting tags fail closed and require manual inspection. The old v2.0.0 release is immutable. This patch workflow has no historical issue-closing side effect and does not require issue write permission.

## Post-publication verification

```bash
gh release view v2.0.1 --repo rodri-oliveira-dev/web-api-core-seed
gh release download v2.0.1 --repo rodri-oliveira-dev/web-api-core-seed --dir ./release-check
cd release-check
sha256sum --check SHA256SUMS.txt
dotnet new install ./RodriOliveira.WebApiCoreSeed.Templates.2.0.1.nupkg
dotnet new webapi-seed -n SampleApi
```

Generated templates only affect newly created projects. Operators must separately patch existing .NET runtimes or rebuild/redeploy their containers. See the [Microsoft October 2026 OOB advisory](https://devblogs.microsoft.com/dotnet/dotnet-october-2026-oob-security-updates/).

## Distribution and licensing

GitHub Release assets remain the only configured distribution channel. The repository has no `LICENSE` file; public source distribution does not grant third-party reuse rights. Licensing and code provenance require owner review before presenting this project as open-source licensed.

For repository About metadata, use the current release URL `https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.1`. If release verification fails, investigate the workflow rather than moving tags; ship subsequent fixes in a new version.
