# Handoff - Phase 5

## Estado Atual

- Branch: `feat/idempotent-development-seed`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/33.
- Pull Request: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/34.
- Objetivo do prompt 01: adicionar seed de desenvolvimento explicito, seguro, deterministico e idempotente.

## Implementacao

- API continua como composition root.
- `Program.cs` roteia `--seed` para `DevelopmentSeedRunner` e nao inicia o pipeline HTTP neste modo.
- `DevelopmentSeedConfiguration` bloqueia `Production` e valida credenciais externas.
- `DevelopmentSeedIdentitySeeder` cria/atualiza usuario por `UserManager<IdentityUser>`.
- `DevelopmentSeedSampleRestaurantSeeder` faz upsert por GUIDs deterministicas no `SampleRestaurantDbContext`.
- Nao ha `EnsureCreated`, `HasData`, JWT emitido pelo seed ou senha real versionada.

## Validacao Local

- Restore locked, build, unit tests, integration tests, seed isolado duas vezes, bloqueio em Production, OpenAPI, vulnerabilidades, deprecated e `git diff --check` foram executados.
- Resultado completo em `.sdd/phase-5/01-development-seed/validation.md`.

## Delivery

- Commit semantico criado e enviado.
- PR aberto para `main` com `Closes #33`.
- Checks remotos passaram: Build/test, CodeQL, Dependency Review e SonarCloud Quality Gate.
- Merge nao realizado.
