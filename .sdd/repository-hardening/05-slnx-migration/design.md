# Design - Prompt 05 SLNX Migration

## Approach

1. Use the .NET SDK migration command:

   ```bash
   dotnet sln WebApiCoreSeed.sln migrate
   ```

2. Compare project lists from the original `.sln` and generated `.slnx`.
3. Remove `WebApiCoreSeed.sln` with `git rm` only after equivalence is confirmed.
4. Update active repository references to `WebApiCoreSeed.slnx`.
5. Preserve historical SDD references when they describe prior completed prompts.
6. Validate restore, build, tests, OpenAPI generation, hooks and workflow syntax using `.slnx`.

## Notes

- The generated `.slnx` is XML and stores normalized slash-separated project paths.
- The migration preserved logical folders and the `Any CPU`, `x64` and `x86` platforms.
- EF design-time factories locate the repository root by solution file; they must now search for `WebApiCoreSeed.slnx`.
