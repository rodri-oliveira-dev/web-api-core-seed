# Permissions Matrix - 05 CI Quality Gates

Padrao: `contents: read`.

| Workflow | contents | security-events | pull-requests | actions | checks | packages | id-token | Justificativa |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `ci.yml` | `read` | none | none | none | none | none | none | Checkout, restore, build, testes, OpenAPI e artifacts usam o token padrao sem escrita explicita. |
| `codeql.yml` | `read` | `write` | none | none | none | none | none | CodeQL precisa escrever resultados em code scanning. |
| `dependency-review.yml` | `read` | none | `read` | none | none | none | none | Dependency Review em PR precisa ler o diff de dependencias do Pull Request. |

## Permissoes Nao Concedidas

- `actions`: nao necessario.
- `checks`: nao necessario.
- `packages`: nao necessario porque restore usa NuGet publico.
- `id-token`: nao ha federacao cloud.
- `pull-requests: write`: removido porque o resumo comentado no PR nao e indispensavel para o gate.
