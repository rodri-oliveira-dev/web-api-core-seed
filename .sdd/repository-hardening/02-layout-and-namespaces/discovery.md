# Discovery

## Comandos de inventario

Foram executados comandos equivalentes aos solicitados, adaptados para PowerShell/Windows quando necessario:

```bash
git ls-files src test tools
rg --files -g '*.csproj' -g '*.sln' -g '*.props' -g '*.targets'
git grep -n 'namespace '
git grep -n 'src\'
git grep -n 'src/'
git grep -n 'test\'
git grep -n 'test/'
git grep -n 'WebApiCoreSeed.Tests'
rg -n '<ProjectReference|AssemblyName|RootNamespace|MigrationsAssembly|WebApiCoreSeed.Tests|test/|test\\|src/SampleRestaurant|src\\SampleRestaurant|src/Identity.Infrastructure|src\\Identity.Infrastructure'
```

`Get-ChildItem` tambem foi usado para listar diretorios de primeiro e segundo niveis e arquivos versionados de `.vscode`, `.github`, `.githooks`, `scripts`, `docs` e `.sdd`.

## Diretorios de primeiro nivel

| Diretorio | Observacao |
| --- | --- |
| `.agents/` | Skills e metadados locais do repositorio. |
| `.github/` | Workflows, CODEOWNERS e template de PR. |
| `.githooks/` | Hook local de pre-push. |
| `.sdd/` | Historico SDD e contexto de repository hardening. |
| `.vscode/` | Tasks, launch e settings. |
| `docker/` | Dockerfiles legados/auxiliares. |
| `docs/` | OpenAPI e quality gates. |
| `scripts/` | Setup local. |
| `sql/` | Script SQL legado. |
| `src/` | Projetos produtivos. |
| `test/` | Projetos de teste ativos antes desta tarefa. |
| `tools/` | Ferramentas auxiliares. |

## Diretorios relevantes de segundo nivel antes da mudanca

| Diretorio | Observacao |
| --- | --- |
| `src/WebApiCoreSeed.Api/` | API e composition root. |
| `src/SampleRestaurant/` | Dominio e aplicacao do sample. |
| `src/SampleRestaurant.Infrastructure/` | Persistencia EF Core do sample. |
| `src/Identity.Infrastructure/` | Persistencia EF Core Identity. |
| `test/WebApiCoreSeed.Tests/` | Testes unitarios, arquiteturais e testes leves HTTP/contrato. |
| `test/WebApiCoreSeed.IntegrationTests/` | Testes de integracao/container. |
| `tools/OpenApiGenerator/` | Gerador local de contratos OpenAPI. |

## Projetos encontrados

| Projeto | Caminho antes | Assembly | Root namespace |
| --- | --- | --- | --- |
| `WebApiCoreSeed.Api` | `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | default: `WebApiCoreSeed.Api` | default: `WebApiCoreSeed.Api` |
| `WebApiCoreSeed.SampleRestaurant` | `src/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj` | default: `WebApiCoreSeed.SampleRestaurant` | default: `WebApiCoreSeed.SampleRestaurant` |
| `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `src/SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj` | default: `WebApiCoreSeed.SampleRestaurant.Infrastructure` | default: `WebApiCoreSeed.SampleRestaurant.Infrastructure` |
| `WebApiCoreSeed.Identity.Infrastructure` | `src/Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj` | default: `WebApiCoreSeed.Identity.Infrastructure` | default: `WebApiCoreSeed.Identity.Infrastructure` |
| `WebApiCoreSeed.Tests` | `test/WebApiCoreSeed.Tests/WebApiCoreSeed.Tests.csproj` | default: `WebApiCoreSeed.Tests` | default: `WebApiCoreSeed.Tests` |
| `WebApiCoreSeed.IntegrationTests` | `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj` | default: `WebApiCoreSeed.IntegrationTests` | default: `WebApiCoreSeed.IntegrationTests` |
| `OpenApiGenerator` | `tools/OpenApiGenerator/OpenApiGenerator.csproj` | default: `OpenApiGenerator` | default: `OpenApiGenerator` |

Nenhum `.csproj` ativo declara `AssemblyName` ou `RootNamespace` explicitamente.

## Project references antes da mudanca

| Projeto | Referencias |
| --- | --- |
| `WebApiCoreSeed.Api` | `src/Identity.Infrastructure`, `src/SampleRestaurant`, `src/SampleRestaurant.Infrastructure` |
| `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `src/SampleRestaurant` |
| `WebApiCoreSeed.Tests` | API, Identity Infrastructure, SampleRestaurant |
| `WebApiCoreSeed.IntegrationTests` | API, Identity Infrastructure, SampleRestaurant Infrastructure |
| `OpenApiGenerator` | API, Identity Infrastructure |

## Referencias vivas encontradas

- `WebApiCoreSeed.sln` referencia `src/SampleRestaurant`, `src/SampleRestaurant.Infrastructure`, `src/Identity.Infrastructure`, `test/WebApiCoreSeed.Tests` e `test/WebApiCoreSeed.IntegrationTests`.
- `.github/workflows/ci.yml` referencia os dois projetos em `test/`.
- `.github/CODEOWNERS` referencia `/test/`.
- `.githooks/pre-push` classifica `test/*` como impacto .NET.
- `docs/quality-gates.md` referencia os dois projetos em `test/`.
- `AGENTS.md` e algumas skills locais referenciam o projeto unitario anterior.
- `tools/OpenApiGenerator/OpenApiGenerator.csproj` referencia `src/Identity.Infrastructure`.

## Referencias historicas

Muitas ocorrencias em `.sdd/phase-1`, `.sdd/phase-2`, `.sdd/phase-3` e `.sdd/phase-4` documentam estados anteriores como `src/DevIO.*`, `test/Pedidos.Test`, `test/WebApiCoreSeed.Tests`, `src/SampleRestaurant` e `src/Identity.Infrastructure`. Essas referencias sao historicas e nao devem ser reescritas em massa, exceto arquivos ativos de mapa/handoff quando servirem como referencia corrente.

## Observacoes

- As migrations ja estao nos projetos de infraestrutura corretos e usam namespaces coerentes com seus assemblies.
- As factories design-time usam `MigrationsAssembly(typeof(DbContext).Assembly.FullName)`, portanto acompanham a pasta do projeto sem exigir alteracao de nome de migration.
- `OpenApiGenerator` ja esta agrupado em `tools/` e seu nome descreve a ferramenta; mover para outra pasta ou renomear para incluir prefixo nao agrega valor nesta tarefa.
