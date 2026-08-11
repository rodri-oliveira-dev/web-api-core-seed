# Rule Applicability

This table records the applicability assessment from CSF.Analyzers documentation and current repository discovery. Because packages are unavailable, chosen severities are documented as planned adoption values, not active configuration.

| ID | Package | Behavior | Default severity | Applicability | False positive potential | Consumer project | Planned severity | Justification |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `REL001` | Reliability | Reports `Task.Run`/`Task.Factory.StartNew` in ASP.NET request flow. | Warning, enabled | Applies to API request handlers/controllers. | Legacy adapters may intentionally offload CPU or isolate blocking work. | API | Info initially | Inventory first in a legacy API; promote after diagnostics are known. |
| `REL002` | Reliability | Reports discarded `Task`/`ValueTask` fire-and-forget in ASP.NET request flow. | Warning, enabled | Applies to API request flow. | Explicit observed background infrastructure may be missed by heuristic. | API | Warning after clean baseline; info on first run if noisy | High operational risk, but first run must be measured. |
| `REL003` | Reliability | Suggests `AsNoTracking()` for EF Core read-only materialized queries. | Info, opt-in | Applies to EF Core query projects only after read tracking policy is confirmed. | Queries may intentionally return tracked entities for later mutation. | API and SampleRestaurant Infrastructure | None initially | Policy not yet confirmed; prompt requires info or disabled. |
| `REL004` | Reliability | Reports EF Core query materialization before filter/projection/paging. | Warning, enabled | Applies to EF Core operational repositories and API code. | In-memory post-processing can be intentional when provider cannot translate logic. | API and SampleRestaurant Infrastructure | Info initially | Useful but should be baselined before warning in legacy query code. |
| `REL005` | Reliability | Reports concurrent EF Core operations on same `DbContext`. | Warning, enabled | Applies to EF Core operational code. | Analyzer is conservative but may miss externally serialized flows. | API and SampleRestaurant Infrastructure | Warning after first run | Runtime failure risk is high; no broad suppression planned. |
| `REL006` | Reliability | Reports scoped dependency capture in hosted services. | Warning, enabled | Applies when hosted services exist or are added. | Classes implementing hosted abstractions but not registered could be intentional. | API if hosted services exist | Warning | Lifetime bug prevention; currently likely low/no emission. |
| `ARC001` | Architecture | Requires explicit `[Authorize]`, `[AllowAnonymous]`, `RequireAuthorization()` or `AllowAnonymous()` for endpoints. | Warning, enabled | Applies to API controllers/minimal endpoints. | Health, metrics, OpenAPI and special public endpoints may need allowlists. | API | Info initially | Security relevant, but existing API must be inventoried before warning. |
| `ARC002` | Architecture | Forbids infrastructure namespaces inside configured core namespaces. | Warning, enabled | Applies to SampleRestaurant core project with calibrated legacy namespaces. | Namespace-based policy may flag intentional compatibility shims. | SampleRestaurant core | Info initially | Hexagonal boundary should be represented, but current namespaces are still legacy. |
| `ARC003` | Architecture | Reports command verbs in literal HTTP route segments. | Info, opt-in | Could apply to API if resource-route policy is adopted. | Portuguese domain routes and compatibility routes may be contractual. | API | None initially | Opt-in design rule; no accepted route naming baseline yet. |
| `ARC004` | Architecture | Reports public setters in domain entities. | Info, opt-in | Could apply to domain models if DDD encapsulation policy is adopted. | Legacy EF/serialization models may require public setters. | SampleRestaurant core | None initially | Opt-in DDD rule; current models are legacy/anemic-compatible. |
| `ARC005` | Architecture | Reports duplicate MSBuild properties also centralized in `Directory.Build.props`. | Info, opt-in | Could apply after CPM and props baseline; repository already consolidated in prompts 3-4. | Intentional project-level overrides may be valid. | All projects | None initially | Evaluate separately with baseline because it reports MSBuild `AdditionalFiles`. |
| `ARC006` | Architecture | Reports domain entities exposed directly in HTTP contracts. | Info, opt-in | Could apply to API if DTO/domain separation policy is enforced. | Legacy contracts may intentionally expose domain-shaped models. | API | None initially | Opt-in architectural rule; requires contract baseline before activation. |
| `TST001` | Testing | Reports broad NSubstitute `Arg.Any`, `*AnyArgs` in positive setups/asserts. | Info, opt-in | Not applicable; tests use Moq. | N/A in current stack. | None | None | Do not install Testing without NSubstitute policy. |
| `TST002` | Testing | Reports `Excluding*` inside FluentAssertions `BeEquivalentTo()` options. | Info, opt-in | Not applicable; no FluentAssertions dependency. | N/A in current stack. | None | None | Do not install Testing without FluentAssertions policy. |

## Planned `ARC002` shape when unblocked

The future configuration should be based on real namespaces:

```ini
dotnet_diagnostic.ARC002.core_namespace_patterns = WebApiCoreSeed.SampleRestaurant.Models*;WebApiCoreSeed.SampleRestaurant.Services*;WebApiCoreSeed.SampleRestaurant.Notificacoes*;WebApiCoreSeed.SampleRestaurant.Intefaces*;WebApiCoreSeed.SampleRestaurant.Interfaces*;WebApiCoreSeed.SampleRestaurant.Application*
dotnet_diagnostic.ARC002.forbidden_namespace_patterns = Microsoft.EntityFrameworkCore;Microsoft.AspNetCore;StackExchange.Redis;Microsoft.Extensions.Caching;WebApiCoreSeed.SampleRestaurant.Infrastructure*;WebApiCoreSeed.Identity.Infrastructure*
dotnet_diagnostic.ARC002.allowed_namespace_patterns =
dotnet_diagnostic.ARC002.ignore_tests = true
```

This is documentation only. It was not added to `.editorconfig` because the package precondition failed.
