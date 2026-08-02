# Severity Profile

This profile is proposed for the next retry after package availability is resolved. It is not active in the repository.

## Principles

- Start default-enabled rules as inventory where the legacy surface is likely to produce noise.
- Do not enable opt-in rules without a documented baseline.
- Do not create global suppressions.
- Keep Testing analyzer absent until the test stack adopts NSubstitute or FluentAssertions policies.

## Planned initial profile

| Rule | API | SampleRestaurant core | SampleRestaurant infrastructure | Identity infrastructure | Tests | Reason |
| --- | --- | --- | --- | --- | --- | --- |
| `REL001` | Info | N/A | N/A | N/A | N/A | Request-flow inventory first. |
| `REL002` | Info or Warning after first run | N/A | N/A | N/A | N/A | Fire-and-forget is high risk, but legacy baseline decides promotion. |
| `REL003` | None | N/A | None | None | N/A | `AsNoTracking` policy not confirmed. |
| `REL004` | Info | N/A | Info | N/A | N/A | EF query performance inventory first. |
| `REL005` | Warning after first run | N/A | Warning after first run | N/A | N/A | Concurrent `DbContext` use is a runtime reliability issue. |
| `REL006` | Warning | N/A | N/A | N/A | N/A | Hosted service lifetime rule should stay active where hosted services exist. |
| `ARC001` | Info initially | N/A | N/A | N/A | N/A | Authorization decisions need endpoint inventory. |
| `ARC002` | N/A | Info initially | N/A | N/A | None | Hexagonal boundary must be calibrated against real legacy namespaces. |
| `ARC003` | None | N/A | N/A | N/A | N/A | Route design policy not baselined. |
| `ARC004` | N/A | None | N/A | N/A | N/A | DDD entity encapsulation policy not baselined. |
| `ARC005` | None | None | None | None | None | AdditionalFiles/MSBuild rule should be evaluated in a separate baseline pass. |
| `ARC006` | None | N/A | N/A | N/A | N/A | HTTP contract/domain separation policy not baselined. |
| `TST001` | N/A | N/A | N/A | N/A | None | Tests use Moq, not NSubstitute. |
| `TST002` | N/A | N/A | N/A | N/A | None | Tests do not use FluentAssertions. |

## No active `.editorconfig` changes

No severity was added to `.editorconfig` in this prompt because no package can be restored from an accepted feed.
