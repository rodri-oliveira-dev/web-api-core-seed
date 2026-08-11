# Package Matrix - 01 .NET 10 Migration

| Project | Package | Previous | New | Action | Justification | Risk | Future issue |
| --- | --- | ---: | ---: | --- | --- | --- | --- |
| API | AspNetCore.HealthChecks.Redis | 3.0.0 | 9.0.0 | Temporary compatibility | Latest stable package line observed; preserve Redis health check. | May still lag .NET 10-specific packages. | Later observability/runtime prompt |
| API | AspNetCore.HealthChecks.SqlServer | 3.0.0 | 9.0.0 | Temporary compatibility | Latest stable package line observed; preserve SQL health check. | Runtime health depends on external SQL Server. | Later observability/runtime prompt |
| API | AspNetCore.HealthChecks.UI | 3.0.9 | none | Removal/temporary disablement | Latest available UI package is 9.0.0 but failed runtime startup with EF Core 10 through its EF-backed storage. | `/hc-ui` unavailable until a compatible strategy is selected. | Later observability/runtime prompt |
| API | AspNetCore.HealthChecks.UI.Client | transitive | 9.0.0 | Temporary compatibility | Keep `UIResponseWriter` for `/hc` JSON output without enabling the web UI. | `/hc` still depends on external service health checks. | Later observability/runtime prompt |
| API | AspNetCore.HealthChecks.Uris | 3.0.0 | 9.0.0 | Temporary compatibility | Preserve URL health checks for Seq. | Package still not aligned to major 10. | Later observability/runtime prompt |
| API | AspNetCoreRateLimit | 3.0.5 | 5.0.0 | Temporary compatibility | Preserve current rate limiting behavior until native replacement prompt. | Package remains a legacy bridge. | `#7` |
| API | AutoMapper | 9.0.0 | 16.2.0 | Definitive update | Avoid known vulnerability in older AutoMapper and use current package line. | Requires modern registration overload. | Future dependency hardening |
| API | AutoMapper.Extensions.Microsoft.DependencyInjection | 7.0.0 | none | Removal | AutoMapper now provides DI registration directly. | Low after build validation. | Future dependency hardening |
| API | Microsoft.AspNetCore.Authentication.JwtBearer | 3.1.2 | 10.0.10 | Definitive update | Required for JWT auth outside shared framework. | JWT behavior should be smoke/regression tested later. | Auth hardening |
| API | Microsoft.AspNetCore.Identity.EntityFrameworkCore | 3.1.2 | 10.0.10 | Definitive update | Required for Identity EF stores. | EF/Identity schema compatibility requires database validation. | Data/auth validation |
| API | Microsoft.AspNetCore.Identity.UI | 3.1.2 | 10.0.10 | Definitive update | Preserve Identity UI package reference if still needed. | May be removable later if unused. | Future dependency cleanup |
| API | Microsoft.AspNetCore.Mvc.Versioning | 4.1.1 | 5.1.0 | Temporary compatibility | Preserve existing API versioning package family for this prompt. | Legacy package family superseded by `Asp.Versioning.*`. | `#8` |
| API | Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer | 4.1.1 | 5.1.0 | Temporary compatibility | Preserve current Swagger/version explorer integration. | Future migration needed. | `#8` |
| API | Microsoft.CodeAnalysis.FxCopAnalyzers | 2.9.8 | none | Removal | Obsolete analyzer package replaced by SDK analyzers. | Analyzer warning profile changes. | Future analyzer hardening |
| API | Microsoft.EntityFrameworkCore | 3.1.2 | 10.0.10 | Definitive update | Align EF runtime with .NET 10 package line. | Database behavior requires integration validation later. | Data validation |
| API | Microsoft.EntityFrameworkCore.Tools | 3.1.2 | 10.0.10 | Definitive update | Keep EF CLI design-time support aligned. | Migration commands not executed in this prompt. | Data validation |
| API | Microsoft.Extensions.Caching.Redis | 2.2.0 | none | Removal | Obsolete and duplicated by StackExchange Redis package. | `AddDistributedRedisCache` call must move to StackExchange implementation. | `#7` or cache cleanup |
| API | Microsoft.Extensions.Caching.StackExchangeRedis | 3.1.2 | 10.0.10 | Definitive update | Supported Redis distributed cache provider. | Redis not fully validated without service. | Cache validation |
| API | Microsoft.Extensions.DependencyInjection | 3.1.2 | none | Removal | API gets DI abstractions from shared framework/transitives. | Low. | None |
| API | Microsoft.Extensions.Logging.Debug | 3.1.2 | 10.0.10 | Definitive update | Preserve debug logger extension used by Program. | Low. | None |
| API | Microsoft.VisualStudio.Web.CodeGeneration.Design | 3.1.1 | none | Removal | Design-time scaffolding package is not needed for restore/build/run and pulled vulnerable transitives. | Scaffolding commands may require local re-add later. | Future dependency cleanup |
| API | Serilog.AspNetCore | 3.2.0 | 10.0.0 | Definitive update | Align ASP.NET Core logging integration with .NET 10. | Logging output should be observed in smoke run. | Observability |
| API | Serilog.Filters.Expressions | 2.1.0 | none | Removal | Legacy expression package replaced when needed. | Filter expression extension may need replacement. | Observability |
| API | Serilog.Expressions | none | 5.0.0 | Substitution | Provides expression filtering support for `Filter.ByExcluding`. | Expression syntax compatibility must be observed. | Observability |
| API | Serilog.Sinks.ColoredConsole | 3.0.1 | none | Removal | Legacy colored console sink replaced by console sink. | Console color specifics may differ. | Observability |
| API | Serilog.Sinks.Console | 3.1.1 | 6.1.1 | Definitive update | Supported console sink. | Low. | None |
| API | Serilog.Sinks.Seq | 4.0.0 | 9.1.0 | Definitive update | Preserve Seq sink with supported package. | Seq unavailable may affect runtime if enabled. | Observability |
| API | KubernetesClient | transitive `15.0.1` | 19.0.2 | Temporary transitive override | Health checks packages pulled vulnerable `15.0.1`; direct private override resolves vulnerability audit. | Should be removed if upstream package updates transitively. | Future dependency cleanup |
| API | Swashbuckle.AspNetCore | 5.0.0 | 6.9.0 | Temporary compatibility | Keep current Swagger implementation and old `Microsoft.OpenApi.Models` surface until OpenAPI prompt. | Reported outdated; latest 10.2.3 requires source/API changes deferred to future prompt. | `#8` |
| API | Swashbuckle.AspNetCore.Swagger | 5.0.0 | none | Removal | Aggregate package covers Swagger. | Low. | `#8` |
| API | Swashbuckle.AspNetCore.SwaggerGen | 5.0.0 | none | Removal | Aggregate package covers SwaggerGen. | Low. | `#8` |
| Business | FluentValidation | 8.6.1 | 12.1.1 | Definitive update | Update validation library to current stable line. | Cascade API changes require minimal source edit. | Validation hardening |
| Business | Microsoft.Extensions.Logging.Abstractions | 3.1.2 | 10.0.10 | Definitive update | Preserve logging abstractions with supported package. | Low. | None |
| Data | Microsoft.EntityFrameworkCore.SqlServer | 3.1.2 | 10.0.10 | Definitive update | Align provider with EF Core 10. | Requires later database integration validation. | Data validation |
| Tests | Bogus | 30.0.4 | 35.6.5 | Definitive update | Supported test data package. | Generated fake data can vary, existing tests use fixed data. | None |
| Tests | Microsoft.NET.Test.Sdk | 16.5.0 | 18.8.1 | Definitive update | Required for modern test execution. | Low. | None |
| Tests | Moq | 4.14.5 | 4.20.72 | Definitive update | Supported mocking package. | Low. | None |
| Tests | xunit | 2.4.0 | 2.9.3 | Definitive update | Keep xUnit v2 runner pattern with modern package. | Avoid xUnit v3 behavioral migration in this prompt. | Test modernization |
| Tests | xunit.runner.visualstudio | 2.4.0 | 3.1.5 | Definitive update | Modern Visual Studio/VSTest runner. | Low. | None |
| Tests | coverlet.collector | 1.2.0 | 10.0.1 | Definitive update | Modern collector compatible with .NET 10. | Coverage not collected in this prompt. | Future CI coverage |
