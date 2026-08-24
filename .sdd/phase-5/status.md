# Status - Phase 5

| Prompt | Status |
| --- | --- |
| 01 - Development seed deterministico e idempotente | PR aberto; checks remotos passaram |

## Prompt 01

- Branch: `feat/idempotent-development-seed`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/33.
- Comando: `dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed`.
- Primeira execucao isolada: 5 mudancas Identity e 11 mudancas SampleRestaurant.
- Segunda execucao isolada: 0 mudancas Identity e 0 mudancas SampleRestaurant.
- Unit tests: 113 passed.
- Integration tests: 53 passed.
- OpenAPI: regenerado sem diff.
- Vulnerabilidades: nenhuma nas fontes atuais.
- Deprecated: `xunit 2.9.3` nos projetos de teste, fora do escopo desta issue.
- PR: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/34.
- Checks remotos: Build/test, CodeQL, Dependency Review e SonarCloud Quality Gate passaram.
