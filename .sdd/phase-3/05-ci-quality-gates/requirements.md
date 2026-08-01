# Requirements - 05 CI Quality Gates

## Objetivo

Criar gates confiaveis para Pull Requests e atualizacoes da branch padrao do repositorio `rodri-oliveira-dev/web-api-core-seed`.

## Escopo

- Restore da solution ativa `RestauranteAPI.sln`.
- Build Release com analyzers configurados por `Directory.Build.props` e `.editorconfig`.
- Testes unitarios do projeto `test/Pedidos.Test/Pedidos.Test.csproj`.
- Testes de integracao/container do projeto `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`.
- Cobertura via `coverlet.collector` e `XPlat Code Coverage`.
- Geracao e verificacao do contrato OpenAPI versionado.
- Auditoria NuGet vulneravel e relatorio NuGet deprecated.
- Dependency Review em Pull Requests.
- CodeQL para C#.
- Dependabot para NuGet e GitHub Actions.

## Fora de Escopo

- Release, deploy, publicacao NuGet, Aspire, secrets de cloud, Sonar, Terraform, ZAP completo, mutation testing obrigatorio e carga k6.

## Criterios de Aceite

- Workflows referenciam apenas arquivos existentes.
- Falhas de gate retornam status nao zero.
- Testes de integracao usam Docker do runner via Testcontainers, sem services duplicados.
- Artifacts de teste, cobertura e OpenAPI sao publicados.
- Permissoes usam `read` como padrao e ampliam apenas quando necessario.
- Branch protection recomendada e documentada sem configurar protecao via arquivo.
