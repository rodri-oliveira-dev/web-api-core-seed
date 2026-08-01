# Design - 05 CI Quality Gates

## Organizacao

Foram adotados tres workflows:

- `ci.yml`: gate principal de qualidade funcional e contrato.
- `codeql.yml`: analise estatica de seguranca com permissao propria.
- `dependency-review.yml`: revisao de dependencias em PR.

Essa separacao deixa cada check com nome claro para branch protection e evita conceder `security-events: write` ao CI principal.

## CI Principal

Sequencia:

1. Checkout.
2. Setup .NET pelo `global.json`.
3. Cache NuGet por manifests.
4. Restore.
5. Build Release.
6. Testes de `Pedidos.Test` com TRX e cobertura.
7. Testes de `WebApiCoreSeed.IntegrationTests` com TRX e cobertura.
8. Geracao OpenAPI.
9. Parse JSON dos contratos versionados.
10. `git diff --exit-code` dos contratos versionados.
11. Auditoria de pacotes vulneraveis.
12. Relatorio de pacotes deprecated.
13. Upload de artifacts.

## Testes

- Unit: `test/Pedidos.Test/Pedidos.Test.csproj`.
- Integration/Container: `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`.
- Nao ha `services` no workflow porque Testcontainers controla SQL Server e Redis.
- Nao ha `continue-on-error`; falhas bloqueiam o check.

## OpenAPI

O contrato e versionado em `docs/openapi/openapi-v1.json` e `docs/openapi/openapi-v2.json`.

O CI gera novamente os arquivos, valida JSON e falha quando ha diff nao commitado.

## Dependencias

- `dotnet list package --vulnerable` e bloqueante quando o SDK retorna erro.
- `dotnet list package --deprecated` e informativo; o comando retorna sucesso mesmo listando `xunit` 2.9.3 como legado.
- Dependency Review falha PRs com severidade moderada ou maior.

## Formatacao

`dotnet format --verify-no-changes` nao foi colocado como gate porque falha hoje por whitespace existente. A decisao e registrada para uma entrega futura de cleanup.
