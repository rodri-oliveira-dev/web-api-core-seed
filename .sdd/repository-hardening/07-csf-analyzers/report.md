# Report

## Summary

CSF.Analyzers adoption is blocked because the required packages are not available from NuGet.org and no other reproducible, documented feed is configured for this repository.

## Evidence

- `CSF.Analyzers.Architecture`: NuGet.org flat-container `404`, NuGet search `totalHits=0`.
- `CSF.Analyzers.Reliability`: NuGet.org flat-container `404`, NuGet search `totalHits=0`.
- `CSF.Analyzers.Testing`: NuGet.org flat-container `404`, NuGet search `totalHits=0`.
- Repository sources: only `nuget.org`.
- Analyzer repository docs say NuGet.org publication remains disabled/commented in release workflow until explicit publication setup exists.

## Changes made

- Created Prompt 6 SDD documentation under `.sdd/repository-hardening/06-csf-analyzers/`.
- Updated repository-hardening status, decisions and handoff.
- Did not change package references, CPM, lock files or analyzer severity configuration.

## Rule adoption recommendation when unblocked

- Install `CSF.Analyzers.Architecture` in API and SampleRestaurant core.
- Install `CSF.Analyzers.Reliability` in API and SampleRestaurant infrastructure.
- Do not install `CSF.Analyzers.Testing` until tests adopt NSubstitute or FluentAssertions policies.
- Start `ARC001`, `ARC002`, `REL001` and `REL004` as inventory in legacy areas.
- Keep `REL003`, `ARC003`, `ARC004`, `ARC005`, `ARC006`, `TST001` and `TST002` disabled until each policy has a baseline.

## Blocker

Package publication or feed configuration is required before implementation.

Accepted next unblock paths:

1. Publish packages to NuGet.org.
2. Configure a public feed that requires no hidden credentials for restore.
3. Configure a private feed explicitly documented by the owner for consumers of this repository/template.

## Commit

Use:

```text
docs: record CSF analyzer adoption blocker
```
