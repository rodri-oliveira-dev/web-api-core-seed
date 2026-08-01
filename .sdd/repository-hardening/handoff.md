# Handoff

## Estado entregue pelo Prompt 3

- Central Package Management hierarquico adotado:
  - `Directory.Packages.props`
  - `src/Directory.Packages.props`
  - `tests/Directory.Packages.props`
  - `tools/Directory.Packages.props`
- Arquivos filhos importam explicitamente o arquivo raiz.
- `ManagePackageVersionsCentrally`, `RestorePackagesWithLockFile` e `RestoreUseStaticGraphEvaluation` foram habilitados na raiz.
- `PackageReference` em `.csproj` nao possui mais `Version`; `PrivateAssets` e `IncludeAssets` foram preservados nos projetos.
- `packages.lock.json` foi gerado e deve ser mantido por projeto ativo.
- Divergencias de teste resolvidas:
  - `coverlet.collector` -> `10.0.1`
  - `Microsoft.NET.Test.Sdk` -> `18.8.1`
  - `xunit.runner.visualstudio` -> `3.1.5`
- Validacoes executadas com sucesso:
  - `dotnet restore WebApiCoreSeed.sln --force-evaluate`
  - `dotnet build WebApiCoreSeed.sln --configuration Release --no-restore`
  - `dotnet test WebApiCoreSeed.sln --configuration Release --no-build`
  - `dotnet list WebApiCoreSeed.sln package --include-transitive`
- Saude de pacotes:
  - nenhum pacote vulneravel reportado;
  - `xunit` `2.9.3` segue deprecated como `Legacy`;
  - `OpenTelemetry.Instrumentation.EntityFrameworkCore` `1.17.0-beta.1` segue reportado como `Nao encontrado nas fontes` pelo outdated.

## Proximo prompt

Prompt 4 deve padronizar build e estilo considerando que CPM e lock files agora fazem parte do contrato de restore. Antes de alterar CI/build, validar se `RestoreLockedMode` deve ser habilitado para ambientes de integracao continua e se warnings de analisadores existentes devem permanecer informativos ou virar erro.

## Riscos para acompanhar

- Nao remover `packages.lock.json` sem decisao explicita, pois agora fazem parte da reproducibilidade do restore.
- Nao voltar a declarar `Version` em `PackageReference`; novas versoes devem entrar no `Directory.Packages.props` do escopo correto.
- `tools/Directory.Packages.props` esta vazio por desenho porque o tooling atual usa apenas pacotes compartilhados na raiz.
- Migracao para `xunit.v3` permanece pendente e deve ser tratada separadamente.

## Estado entregue pelo Prompt 2

- Layout produtivo normalizado:
  - `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`
  - `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj`
  - `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj`
  - `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`
- Layout de testes normalizado:
  - `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`
  - `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`
- Namespaces de testes atualizados para `WebApiCoreSeed.UnitTests.*`.
- Solution, project references, CI, CODEOWNERS, hook de pre-push, docs de quality gates, AGENTS e skills locais foram atualizados para os novos caminhos.
- `tools/OpenApiGenerator` foi mantido e validado.
- Migrations permaneceram nos mesmos assemblies e sem pending model changes.
- OpenAPI foi regenerado e permaneceu sem diff.
- Smoke da API respondeu `/openapi/v1.json` e `/hc` com HTTP 200.

## Estado entregue pelo Prompt 1

- `.gitignore` modernizado e ajustado para a solution atual.
- `.gitattributes` criado com normalizacao de texto, finais de linha e classificacao de binarios.
- `.dockerignore` criado para futuro build containerizado sem excluir arquivos de restore/build.
- Artefatos ativos de Sonar removidos de `src/`.
- Caminhos pessoais historicos sanitizados.
- Inventario operacional documentado em `repository-inventory.md`.
- Baseline de validacao registrada em `validation-baseline.md` e `01-repository-hygiene/validation.md`.

## Proximo prompt

Prompt 3 deve adotar Central Package Management sem reintroduzir arquivos de analise removidos nos prompts anteriores. Antes de consolidar pacotes, validar:

- `git status --short`
- `git ls-files src tests tools`
- todos os `.csproj` ativos e `Directory.Build.props`
- versoes duplicadas de `PackageReference`
- impactos em `.sln`, `.csproj`, testes, CI, docs e SDD

## Riscos para acompanhar

- `src/WebApiCoreSeed.Api/Properties/launchSettings.json` nao existe hoje; se for criado em prompt futuro, nao sera bloqueado pelo `.gitignore`.
- A pasta `tests/` foi adotada no Prompt 2; nao voltar a usar `test/` para projetos ativos.
- Ocorrencias restantes de caminhos antigos em `.sdd/phase-*` e `LEGACY.md` sao historicas; nao usa-las como comandos atuais.
- Dockerfile da API ainda nao existe; `.dockerignore` foi preparado de forma portavel e conservadora.
