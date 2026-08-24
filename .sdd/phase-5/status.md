# Status - Phase 5

| Prompt | Status |
| --- | --- |
| 01 - Development seed deterministico e idempotente | PR aberto; checks remotos passaram |
| 02 - Normalize UTF-8 encoding and active code naming | PR aberto; SonarCloud coverage remediation em validacao |

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

## Prompt 02

- Branch: `refactor/normalize-encoding-and-naming`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/35.
- Nomes ativos corrigidos: `Intefaces` -> `Interfaces`, `Clains` -> `Claims`, `Loggin*` -> `LogEntry*`.
- Identificador legado preservado: tabela `Loggin`.
- OpenAPI: regenerado com mudancas textuais em descricoes 400/401/429.
- Unit tests: 124 passed after coverage remediation.
- Integration tests: 54 passed.
- Architecture tests: 8 passed explicitamente.
- EF pending model changes: sem alteracoes pendentes nos dois DbContexts.
- Vulnerabilidades: nenhuma nas fontes atuais.
- PR: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/36.
- Checks remotos: primeira rodada passou build/test, CodeQL e Dependency Review, mas SonarCloud falhou por `new_coverage` 66.0 abaixo do limiar 80; testes focados adicionados antes do novo push.
