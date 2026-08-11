# Handoff

## Estado apos Prompt 6 adicional

- Layout final permanece em `src/`, `src/Modules/`, `tests/` e `tools/`.
- Solution ativa permanece `WebApiCoreSeed.slnx`.
- Projetos ativos permanecem:
  - `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj`;
  - `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant/WebApiCoreSeed.SampleRestaurant.csproj`;
  - `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure/WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj`;
  - `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure/WebApiCoreSeed.Identity.Infrastructure.csproj`;
  - `tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj`;
  - `tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`;
  - `tools/OpenApiGenerator/OpenApiGenerator.csproj`.
- Namespaces do core SampleRestaurant ainda preservam compatibilidade legada: `Models`, `Services`, `Interfaces`, `Intefaces`, `Notificacoes` e `Application.Contracts`.
- CPM permanece ativo por `Directory.Packages.props` raiz e arquivos por escopo em `src/`, `tests/` e `tools/`.
- Arquivos MSBuild consolidados permanecem `Directory.Build.props`, `Directory.Packages.props` e respectivos arquivos por escopo.
- Prompt 6 agora e `Ambiente local com Docker e User Secrets`.
- O registro anterior de CSF.Analyzers foi preservado em `.sdd/repository-hardening/07-csf-analyzers`.
- Nenhum package CSF.Analyzers foi instalado.
- Nenhuma regra CSF.Analyzers esta ativa.
- Nenhuma supressao global foi adicionada.

## Ambiente local conteinerizado

- Root `Dockerfile` criado com stages `runtime`, `restore`, `build`, `publish`, `migrations` e `final`.
- SDK image: `mcr.microsoft.com/dotnet/sdk:10.0`.
- Runtime image: `mcr.microsoft.com/dotnet/aspnet:10.0`.
- Final image expõe `8080` e executa como usuario `app`.
- Root `compose.yaml` criado com `api`, `migrations`, `sqlserver` e `redis`.
- SQL Server: `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`.
- Redis: `redis:7.4.2-alpine`.
- Volumes nomeados: `sqlserver-data` e `redis-data`.
- UserSecretsId preservado no projeto da API: `c52dbe85-d94e-4cc2-9856-529f22712174`.
- User Secrets sao usados somente para `dotnet run` no host.
- Compose usa `.env.local` ou variaveis do host; User Secrets nao sao montados em containers.
- A API nao aplica migrations no startup; o servico `migrations` aplica `ApplicationDbContext` e `SampleRestaurantDbContext` antes da API.
- Dockerfiles legados em `docker/` foram removidos por conterem senha fixa/tag flutuante e nao serem a stack oficial.
- Validacao usou `API_HTTP_PORT=18080` porque a porta local `8080` estava ocupada.
- Containers, rede e volumes de validacao foram removidos ao final.

## Validacao do Prompt 6 adicional

- `dotnet restore WebApiCoreSeed.slnx`: passou.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passou.
- `dotnet test WebApiCoreSeed.slnx --configuration Release --no-build`: passou com 53 testes unitarios/leves e 42 testes de integracao/container.
- OpenAPI generator: passou.
- `docker build --pull --tag web-api-core-seed:local .`: passou.
- Compose config/build/up com project name `web-api-core-seed-validation`: passou apos alterar a porta temporaria para `18080`.
- SQL Server e Redis ficaram saudaveis.
- Migrations concluiram com exit code `0`.
- HTTP smoke: health, OpenAPI, endpoint publico, Problem Details e endpoint protegido sem token validados.
- Persistencia: dado SQL e chave Redis sobreviveram a restart da API e das dependencias.
- Logs e image history nao exibiram senha, JWT secret nem connection string completa.
- Imagem final nao contem SDK, source, `.env.local`, `.git` ou User Secrets.

## Commands oficiais de ambiente local

```bash
cp .env.local.example .env.local
docker compose --env-file .env.local config
docker compose --env-file .env.local build
docker compose --env-file .env.local up -d
docker compose --env-file .env.local up -d sqlserver redis
docker compose --env-file .env.local logs -f api
docker compose --env-file .env.local down
docker compose --env-file .env.local down --volumes
```

## Bloqueio de analyzers

- `CSF.Analyzers.Architecture`: indisponivel no NuGet.org.
- `CSF.Analyzers.Reliability`: indisponivel no NuGet.org.
- `CSF.Analyzers.Testing`: indisponivel no NuGet.org.
- O repositorio nao possui `NuGet.Config` com feed publico/privado alternativo.
- A documentacao do repositorio `rodri-oliveira-dev/CSF.Analyzers` confirma que a publicacao no NuGet.org ainda nao esta habilitada.

## Regras avaliadas

- Reliability: `REL001`, `REL002`, `REL003`, `REL004`, `REL005`, `REL006`.
- Architecture: `ARC001`, `ARC002`, `ARC003`, `ARC004`, `ARC005`, `ARC006`.
- Testing: `TST001`, `TST002`.
- Matriz completa em `.sdd/repository-hardening/06-csf-analyzers/rule-applicability.md`.

## Warnings conhecidos

- O ultimo build completo registrado no Prompt 5 passou com 30 warnings `CA*` historicos.
- O Prompt 6 nao executou novo build porque foi bloqueado antes de alterar packages.

## Commands oficiais

Quando o bloqueio for resolvido, retomar com:

```bash
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore --no-incremental
dotnet test WebApiCoreSeed.slnx --configuration Release --no-build
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --filter "Architecture=ModularHexagonal"
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "Category=Integration"
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
```

## Commits da padronizacao

Esperados seis commits semanticos na Fase 4 hardening:

1. `chore: harden repository metadata`
2. `refactor: standardize repository layout`
3. `build: adopt central package management`
4. `build: standardize dotnet build settings`
5. `build: migrate solution to slnx`
6. `docs: record CSF analyzer adoption blocker`

## Limitacoes

- A Fase 4 hardening nao esta concluida funcionalmente porque a adocao dos analyzers depende de publicacao/feed reproduzivel.
- `CSF.Analyzers.Testing` continua fora do desenho inicial ate o projeto adotar NSubstitute ou FluentAssertions como politica de teste.
- `ARC002` deve usar namespaces reais, nao somente nomes de pastas.
- `REL003` deve permanecer desabilitada ate a politica de `AsNoTracking` ser confirmada.

## Proxima branch

Nao iniciar `phase/5-open-source-productization` ate o proprietario decidir se:

1. publica os pacotes CSF.Analyzers em NuGet.org;
2. configura feed reproduzivel documentado;
3. aceita seguir para a Fase 5 com esse bloqueio explicitamente pendente.

Branch seguinte planejada quando liberada:

```text
phase/5-open-source-productization
```
