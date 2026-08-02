# Requirements

## Goal

Adopt contextual CSF analyzers gradually only when the analyzer packages are available from a reproducible NuGet source.

## Mandatory package precondition

Before changing package files, confirm availability of:

- `CSF.Analyzers.Architecture`
- `CSF.Analyzers.Reliability`
- `CSF.Analyzers.Testing`

Accepted sources, in order:

1. NuGet.org.
2. Public feed that does not require credentials for restore.
3. Private feed explicitly configured by the repository owner.

Rejected sources:

- `ProjectReference` to an external clone.
- Absolute path.
- Locally generated package without reproducible origin.
- Silently versioned `.nupkg`.
- Floating GitHub Release URL.
- Token-required feed without consumer documentation.

## Intended adoption when unblocked

Install only:

- `CSF.Analyzers.Architecture`
- `CSF.Analyzers.Reliability`

Do not install initially:

- `CSF.Analyzers.Testing`

Reason: current tests use Moq and do not establish a policy based on NSubstitute or FluentAssertions.

## Acceptance criteria when unblocked

- Packages restored from a reproducible feed.
- Versions centralized by CPM.
- Analyzer references use `PrivateAssets=all`.
- Analyzer references include only analyzer/build assets needed by the package.
- Rules are calibrated per consuming project.
- Hexagonal architecture is represented in `ARC002`.
- Opt-in rules are not promoted without a baseline.
- No global suppressions hide problems.
- CI runs analyzers through the normal build.
- Adopted rules are documented.

## Blocked outcome

If the packages are unavailable, do not alter packages. Record the blocker, update SDD, and commit:

```text
docs: record CSF analyzer adoption blocker
```
