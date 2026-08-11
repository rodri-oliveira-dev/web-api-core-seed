# Validation - 04 Native Rate Limiting

## Initial Validation

| Command | Result |
| --- | --- |
| `git status --short` | clean |
| `git branch --show-current` | `phase/2-dotnet-10-migration` |
| `git log -3 --oneline` | `e56d29a`, `24f701d`, `b8593c5` |
| `dotnet build --configuration Release` | passed with existing analyzer warnings |
| `dotnet test --configuration Release --no-build` | passed, 27 tests |

## Development Validation

| Command | Result |
| --- | --- |
| `dotnet build --configuration Release` | passed with existing analyzer warnings |
| `dotnet test --configuration Release --no-build` | passed, 32 tests |

During test development, public endpoint checks initially exposed that the existing integration fixture was still using Redis-backed response caching for `GET /api/v1/Pratos`. The fixture now replaces `RedisCacheSettings` in DI so HTTP tests do not depend on external Redis.

## Final Validation

| Command | Result | Time |
| --- | --- | --- |
| `dotnet restore` | passed | 3.3s |
| `dotnet build --configuration Release --no-restore` | passed, 30 existing analyzer warnings, 0 errors | 3.31s |
| `dotnet test --configuration Release --no-build` | passed, 32 tests | 3s test duration |
| `dotnet list package` | passed; `AspNetCoreRateLimit` absent | 6.8s |

## Final Legacy Search Checks

```text
git grep -n "AspNetCoreRateLimit" -- src test README.md AGENTS.md
```

Result: no active findings.

```text
git grep -n "IpRateLimit" -- src test README.md AGENTS.md
```

Result: no active findings.

```text
git grep -n "ClientRateLimit" -- src test README.md AGENTS.md
```

Result: no active findings.

```text
git grep -n "ForwardedHeaders" -- src test README.md AGENTS.md
```

Result: no active findings.

```text
git grep -n "RemoteIpAddress" -- src test README.md AGENTS.md
```

Result:

```text
src/DevIO.Api/Configuration/RateLimitConfig.cs:139:var remoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
```

This is the documented anonymous fallback. It does not trust forwarded headers and the raw value is not logged.

## Smoke And Regression Scope

Covered by `WebApplicationFactory` integration host:

- normal public API request below the limit;
- protected endpoint without token;
- protected endpoint with token and independent user partition;
- health endpoint exempt from API limits;
- controlled sequence that returns `429`;
- new anonymous partition proving recovery without waiting;
- Swagger JSON.

Smoke/regression command:

```text
dotnet test --configuration Release --no-build --filter "FullyQualifiedName~ProblemDetailsContractTests"
```

Result: passed, 11 tests, 3s test duration.

An additional process-based local smoke was attempted, but the local shell policy rejected the background process command before the API started. The test-host smoke above exercises the ASP.NET Core pipeline without requiring SQL Server or Redis.
