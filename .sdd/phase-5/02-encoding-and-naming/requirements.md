# Requirements - Encoding And Naming

## Specification

Normalize UTF-8 text and active naming defects without breaking public HTTP contracts, OpenAPI route/property names, legacy database schema, migration IDs, or historical references.

## Scope

- Correct mojibake in active API/user-facing messages.
- Correct active code namespaces and folders named `Intefaces` and `Clains`.
- Correct active C# naming around `Loggin` using `LogEntry`.
- Preserve the legacy database table `Loggin` through explicit EF Core mapping.
- Preserve historical migrations and migration IDs.
- Correct active documentation terminology such as `FluentValidator` to `FluentValidation`.
- Regenerate and validate OpenAPI after active text corrections.
- Add automated checks for known mojibake and active naming regressions.

## Compatibility Requirements

- Do not rename legacy tables or columns for spelling only.
- Do not change routes, JSON property names, status codes, authentication behavior, or authorization policy names.
- Do not regenerate historical migrations.
- Keep current EF Core model snapshots aligned with the active model to avoid pending model changes.
- Keep files in UTF-8 without BOM and LF line endings according to `.editorconfig` and `.gitattributes`.
- Treat historical documents and legacy SQL as reference material, not active code.

## Issue

- Supplied issue value: `ISSUE_URL`.
- Created issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/35.
- Reason: no existing issue with the expected title was found, and the supplied value was a placeholder.
