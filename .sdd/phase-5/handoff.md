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

## Prompt 02 - Encoding And Naming

- Branch: `refactor/normalize-encoding-and-naming`.
- Issue: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/35.
- Objetivo: normalizar codificacao UTF-8 e nomenclatura ativa sem quebrar contratos HTTP, OpenAPI, schema legado ou migrations historicas.
- Nomes ativos corrigidos:
  - `WebApiCoreSeed.SampleRestaurant.Intefaces` -> `WebApiCoreSeed.SampleRestaurant.Interfaces`.
  - `WebApiCoreSeed.Api.Extensions.Clains` -> `WebApiCoreSeed.Api.Extensions.Claims`.
  - `Loggin*` C# ativo -> `LogEntry*`.
- Persistencia:
  - `LogEntryMapping` preserva `builder.ToTable("Loggin")`.
  - `SampleRestaurantDbContextModelSnapshot` foi atualizado para o tipo ativo `LogEntry`.
  - Migrations historicas e designers foram preservados.
- OpenAPI:
  - Regenerado com alteracoes textuais apenas em descricoes.
- Validacao local:
  - Restore locked, build Release, unit tests, integration tests, architecture tests, migrations em banco vazio, upgrade legado, EF pending, OpenAPI JSON, vulnerabilidades e `git diff --check` passaram.
- PR: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/36.
- Checks remotos:
  - Primeira rodada passou build/test, CodeQL e Dependency Review.
  - SonarCloud Quality Gate falhou inicialmente por cobertura de codigo novo (`new_coverage` 66.0 abaixo do limiar 80).
  - Foram adicionados testes focados para `LogEntryValidation`, `LogEntryService`, `LogEntryRepository` e textos normalizados de Problem Details antes do novo push.
