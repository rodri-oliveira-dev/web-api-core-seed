# Diagnostics Baseline

## Result

No CSF analyzer diagnostics baseline was produced because the analyzer packages cannot be restored from an accepted NuGet source.

## Why no synthetic baseline was created

The prompt explicitly rejects:

- `ProjectReference` to a clone of `CSF.Analyzers`.
- Local `.nupkg` consumption without reproducible origin.
- Silently versioned `.nupkg`.
- Floating GitHub Release URL.

Running analyzers from any of those paths would create a non-reproducible baseline and would not represent what CI/template consumers can restore.

## Existing non-CSF warning context

Prompt 5 recorded:

- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passed with 30 existing `CA*` warnings and 0 errors.
- `dotnet test WebApiCoreSeed.slnx --configuration Release --no-build`: passed with 95 tests.

Those existing warnings remain outside the scope of this blocked analyzer adoption.

## Future baseline process

When packages are published to an accepted feed:

1. Add `CSF.Analyzers.Architecture` to API and SampleRestaurant core only.
2. Restore and build.
3. Record diagnostics.
4. Calibrate `ARC001`/`ARC002`.
5. Add `CSF.Analyzers.Reliability` to API and SampleRestaurant infrastructure only if restore/build remains reproducible.
6. Restore and build again.
7. Record diagnostics.
8. Fix real violations.
9. Register localized exceptions only after confirming false positives in rule documentation.
