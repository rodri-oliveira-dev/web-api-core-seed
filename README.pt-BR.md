# Web API Core Seed

[English](README.md) | [**Português (Brasil)**](README.pt-BR.md)

[![CI](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/ci.yml/badge.svg)](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/ci.yml)
[![CodeQL](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/codeql.yml/badge.svg)](https://github.com/rodri-oliveira-dev/web-api-core-seed/actions/workflows/codeql.yml)

> **Estado do projeto:** aplicação de exemplo mantida em **.NET 10**. O repositório **ainda não é distribuído como template instalável via `dotnet new`**; essa evolução está na [#22](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/22). A versão histórica .NET Core 3.1 está fora de suporte; consulte [Versão legada](#versão-legada).

## Visão geral

Web API Core Seed é uma API REST de referência em ASP.NET Core que demonstra autenticação, domínio de restaurante modular, portas explícitas de persistência, SQL Server, cache de respostas opcional no Redis, OpenTelemetry e validações automatizadas. É uma **aplicação funcional** para estudo e referência, não um blueprint pronto para produção.

A solução ativa é `WebApiCoreSeed.slnx`. O ponto de entrada está em `src/WebApiCoreSeed.Api/Program.cs`. O domínio demonstrativo é separado dos elementos reutilizáveis de HTTP/hospedagem; veja [arquitetura e fluxo de requisição](docs/architecture.pt-BR.md).

## Pré-requisitos

- [SDK do .NET **10.0.401**](global.json) para execução no host, conforme `global.json`.
- Docker Engine com Compose v2 para SQL Server, Redis, stack completa e testes de integração (Testcontainers).
- Git e terminal (Bash ou PowerShell); `curl` é opcional para smoke tests.
- Senha forte local do SQL Server, segredo JWT e senha do seed. **Nunca os versione**.
- Opcional: ferramenta CLI `dotnet-ef` da série 10 para alterar o esquema manualmente.

Visual Studio 2019 e .NET Core 3.1 não são necessários para a solução mantida.

## Início rápido com Docker Compose

A partir da raiz do repositório:

```bash
git clone https://github.com/rodri-oliveira-dev/web-api-core-seed.git
cd web-api-core-seed
cp .env.local.example .env.local
```

Edite `.env.local` e substitua os **três** valores de credenciais (`SQLSERVER_SA_PASSWORD`, `JWT_SECRET` e `DEVELOPMENT_SEED_PASSWORD`). Use valores fortes. O arquivo é ignorado pelo Git.

```bash
docker compose --env-file .env.local config
docker compose --env-file .env.local up --build -d
docker compose --env-file .env.local ps
docker compose --env-file .env.local --profile tools up seed
curl -i http://localhost:8080/health/live
```

O primeiro `up` inicia SQL Server, Redis e API; o serviço `migrations` prepara os dois esquemas EF Core. O comando opcional `seed` popula dados de desenvolvimento e termina. A API estará em `http://localhost:8080` (ou na porta `API_HTTP_PORT` configurada). Veja o [desenvolvimento em contêineres](docs/development/containerized-local-development.md) para logs e modos de execução.

Para parar sem apagar os dados:

```bash
docker compose --env-file .env.local down
```

**Atenção:** `docker compose down --volumes` remove os volumes locais do SQL Server e Redis.

## Executar a API no host

É necessário o SDK .NET e um SQL Server local configurado. Inicie apenas as dependências:

```bash
docker compose --env-file .env.local up -d sqlserver redis
dotnet restore WebApiCoreSeed.slnx
```

Configure os User Secrets do ASP.NET Core usando `./scripts/setup/configure-user-secrets.sh` (Bash) ou `./scripts/setup/configure-user-secrets.ps1` (PowerShell). O script solicita `ConnectionStrings:DefaultConnection`, `AppSettings:Secret` e `DevelopmentSeed:User:Password` sem salvá-los no Git. Na conexão do host use `localhost,1433`, e não o DNS `sqlserver` do Compose; para Redis use `localhost:7001`.

No Bash, execute explicitamente em Development:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

No PowerShell, defina `$env:ASPNETCORE_ENVIRONMENT = "Development"` antes do mesmo comando `dotnet run`. A porta pode ser configurada por `ASPNETCORE_URLS`; `dotnet run` no host **não lê automaticamente** `.env.local`. Alternativamente, execute toda a aplicação pelo Compose.

## Executar os testes

```bash
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build
```

Os testes de integração inicializam contêineres isolados de SQL Server e Redis com Testcontainers; o Docker deve estar ativo. A pipeline também valida OpenAPI, vulnerabilidades de dependências, SonarCloud (contextos confiáveis) e CodeQL. Consulte [quality gates](docs/quality-gates.md).

## Aplicar e criar migrations

No primeiro setup local, o Compose aplica automaticamente migrations do **Identity e SampleRestaurant** antes de iniciar a API. Para executar o serviço explicitamente:

```bash
docker compose --env-file .env.local up migrations
```

Para alterações manuais com EF Core, instale `dotnet-ef` 10. **As factories de design-time não carregam User Secrets da API:** forneça a conexão local pela variável temporária `ConnectionStrings__DefaultConnection` no mesmo terminal dos comandos `dotnet ef`. Cada DbContext pertence ao respectivo projeto de Infrastructure. Consulte o [guia de migrations EF Core](docs/development/ef-core-migrations.pt-BR.md) para configurar com segurança em Bash/PowerShell e executar os comandos. Não execute migrations ou seed de desenvolvimento em produção sem um plano de implantação revisado.

## Gerar dados de exemplo (seed)

O seed é **explícito, idempotente e proibido em Production**. Após criar `.env.local` e iniciar a stack:

```bash
docker compose --env-file .env.local --profile tools up seed
```

Ou, depois de configurar os User Secrets e o ambiente Development no host:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed
```

PowerShell: defina `$env:ASPNETCORE_ENVIRONMENT = "Development"` antes de `dotnet run ... -- --seed`. O comando aplica ambos os conjuntos de migrations e cria/atualiza uma conta Identity (`DEVELOPMENT_SEED_EMAIL`, padrão `developer@example.local`) e registros do restaurante. Reexecuções não devem gerar duplicatas. Veja [detalhes do seed](docs/development/containerized-local-development.md#development-seed).

## Autenticação e exploração da API

- `POST /api/v2/entrar`: login anônimo com usuário/senha de desenvolvimento; retorna JWT. O login v1 ainda existe, mas está depreciado.
- `GET /api/v1/Pratos`: lista pública com paginação limitada; outros endpoints do restaurante podem exigir JWT Bearer e claims.
- `/scalar`: referência interativa da API; documentos OpenAPI também são gerados por versão.
- `/health/live`, `/health/ready`: liveness/readiness; `/hc` é um endpoint de compatibilidade.

Após rodar o `seed`:

```bash
curl -i http://localhost:8080/health/ready
curl -X POST http://localhost:8080/api/v2/entrar -H "Content-Type: application/json" -d '{"email":"developer@example.local","password":"<sua senha local do seed>"}'
```

Não publique tokens nem senhas em issues ou logs. Consulte os [contratos da API](docs/openapi/) e a [arquitetura](docs/architecture.pt-BR.md).

## Arquitetura

A API HTTP é o composition root. `SampleRestaurant` contém domínio, services de aplicação e portas explícitas; `SampleRestaurant.Infrastructure` implementa repositórios EF Core e Unit of Work. `Identity.Infrastructure` mantém o DbContext Identity e as migrations. A API contém controllers, autenticação, autorização, rate limiting, tratamento de erros, cache e telemetria. O **repositório genérico mencionado no projeto histórico não faz parte do design ativo do SampleRestaurant**.

Consulte [arquitetura](docs/architecture.pt-BR.md), [ADRs](docs/adr/) e o [guia de migração do legado](docs/migration-from-legacy.pt-BR.md).

## Configuração e observabilidade

- `ConnectionStrings:DefaultConnection` é obrigatório para SQL Server.
- `AppSettings:Secret`, `Emissor` e `ValidoEm` configuram assinatura, emissor e audiência do JWT.
- `RedisCacheSettings:Enabled` controla o cache distribuído de respostas. Apenas GETs anônimos elegíveis são cacheados; credenciais, respostas de erro e respostas com `Set-Cookie` são excluídas.
- `Cors:AllowedOrigins` usa origens explícitas (vazio por padrão).
- `OpenTelemetry:Enabled` ativa traces/métricas; exportação OTLP é desabilitada até configurar `OpenTelemetry:Otlp:Enabled` e endpoint.
- `SeqSettings:Enabled` habilita exportação opcional para Seq; Serilog emite logs no console.

Compose usa variáveis com dois sublinhados, como `AppSettings__Secret`. No host use User Secrets. Consulte [arquitetura](docs/architecture.pt-BR.md) e [desenvolvimento em contêineres](docs/development/containerized-local-development.md).

## Solução de problemas

| Sintoma | O que verificar |
| --- | --- |
| Compose não inicia | Substitua placeholders de `.env.local` e rode `docker compose --env-file .env.local config`. |
| SQL Server ou migrations falham | Rode `docker compose --env-file .env.local logs sqlserver migrations`; confira complexidade da senha e portas. |
| API no host falha na configuração | Confira User Secrets, `ASPNETCORE_ENVIRONMENT=Development` e conexão do host com SQL Server. |
| Login falha | Execute o seed, use as credenciais configuradas e confira a política de senha Identity. |
| Readiness unhealthy | Inspecione SQL Server/Redis e `docker compose --env-file .env.local logs api`. |
| Testes de integração falham | Inicie Docker Engine e verifique se Testcontainers consegue subir SQL Server e Redis. |
| Portas 8080/1433/7001 ocupadas | Altere a porta correspondente em `.env.local`. |

Outras dicas: [troubleshooting do ambiente local](docs/development/containerized-local-development.md#troubleshooting).

## Contribuição e suporte

Consulte [CONTRIBUTING.md](CONTRIBUTING.md), [Código de Conduta](CODE_OF_CONDUCT.md), [reporte de segurança](SECURITY.md) e [CHANGELOG.md](CHANGELOG.md). Para dúvidas de uso, abra um GitHub Discussion se estiver habilitado ou uma issue. **Não divulgue vulnerabilidades exploráveis publicamente.**

## Versão legada

O código .NET Core 3.1 original e intacto está preservado na tag [`v1.0.0-legacy`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/v1.0.0-legacy); a branch [`legacy/netcoreapp3.1`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/legacy/netcoreapp3.1) adiciona um aviso de fim de suporte. .NET Core 3.1 encerrou suporte em 13 de dezembro de 2022. Veja [LEGACY.md](LEGACY.md) e o [guia de migração](docs/migration-from-legacy.pt-BR.md).

## Roadmap

Próximas etapas: [empacotamento `dotnet new` (#22)](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/22) e [release v2.0.0 (#23)](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23). **Nenhuma dessas entregas é declarada concluída aqui.**

## Recursos para estudar

Para quem chega ao .NET de outro ecossistema: [fundamentos do ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/), [documentação EF Core](https://learn.microsoft.com/ef/core/), [testes de integração no ASP.NET Core](https://learn.microsoft.com/aspnet/core/test/integration-tests) e [Docker Compose](https://docs.docker.com/compose/).