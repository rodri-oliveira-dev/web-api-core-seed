# Discovery - 03 Security Hardening

## Commands

- `git status --short`: clean at start.
- `git branch --show-current`: `phase/3-quality-and-safety`.
- `git log -3 --oneline`: `e21a215`, `d8730d3`, `f35b72a`.
- `dotnet build --configuration Release`: initial concurrent execution was blocked by a lingering `testhost` file lock.
- `dotnet test --configuration Release --no-build`: passed with 36 existing tests and 18 integration tests.

## Grep Findings

- `AllowAnyOrigin`: active in `src/DevIO.Api/Configuration/ApiConfig.cs` for both Development and Production CORS policies.
- `AllowCredentials`: no active occurrence.
- `UseCors`: selected Development or Production policy in `HostingConfig`.
- `UseForwardedHeaders` / `ForwardedHeadersOptions`: no active implementation; phase 2 documented this as a proxy risk.
- `X-XSS-Protection`: active in `ApiConfig.AjustesSeguranca`; commented legacy entry in `web.config`.
- `Feature-Policy`: active in `ApiConfig.AjustesSeguranca`.
- `Content-Security-Policy`: active, but coupled to obsolete header block.
- `QueryString`: full query string enriched into Serilog request logging.
- `Request.Headers`: limited to cookie user-agent compatibility and rate-limit client id; custom Serilog middleware only whitelisted content/user-agent headers for error context.
- `HealthCheckUI`: UI package endpoint is disabled, but appsettings still contains legacy HealthChecks-UI config for `/hc`.
- `UseStatusCodePages`: writes Problem Details for non-exception status codes.

## Sensitive Data Risks

- Custom request middleware used `IHttpRequestFeature.RawTarget`, which includes the query string.
- Serilog request logging explicitly set `QueryString`.
- Default `appsettings.json` included local-looking SQL and JWT secret values.
- Login responses contain access tokens and user claims; these endpoints need no-store cache headers.
- Problem Details in production do not expose stack traces; development still returns exception messages by design.
