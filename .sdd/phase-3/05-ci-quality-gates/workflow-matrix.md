# Workflow Matrix - 05 CI Quality Gates

| Workflow | Trigger | Jobs | Dependencias | Duracao observada | Condicao de falha | Artifacts | Check name | Obrigatorio |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `ci.yml` | `pull_request` para `main`; `push` para `main` | `build-test-quality` | .NET SDK por `global.json`; Docker do runner; NuGet; Testcontainers | Local equivalente: build 7s; test total 38s; OpenAPI 8s | restore, build, testes, cobertura sem arquivo, OpenAPI invalido/dessincronizado ou auditoria vulneravel retornando non-zero | `test-results`, `coverage-results`, `openapi-contracts` | `Build, test and quality gates` | Sim |
| `codeql.yml` | `pull_request` para `main`; `push` para `main`; schedule segunda 08:37 UTC | `analyze` | .NET SDK por `global.json`; CodeQL C# | Nao executado localmente | build manual ou analise CodeQL falha | Code scanning upload do GitHub | `CodeQL analysis` | Sim |
| `dependency-review.yml` | `pull_request` para `main`, ignorando docs/imagens/SDD | `dependency-review` | Dependency Review Action | Nao executado localmente | nova dependencia vulneravel ou licenciada conforme politica da action; severidade moderada ou maior | Nenhum artifact versionado | `Review dependency changes` | Sim em PR |

## Informativo

- `dotnet list package --deprecated` roda no CI principal, mas nao introduz politica nova de bloqueio porque `xunit` 2.9.3 e reportado como `Legacy` nos testes existentes.
- `dotnet format --verify-no-changes` nao foi ativado como check enquanto houver divida de whitespace existente.
