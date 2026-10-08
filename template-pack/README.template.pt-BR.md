# WebApiCoreSeed

[English](README.md) | **Português (Brasil)**

Projeto gerado pelo template [Web API Core Seed](https://github.com/rodri-oliveira-dev/web-api-core-seed) para **.NET 10** (`dotnet new webapi-seed`).

Este projeto é um ponto de partida, **não uma aplicação pronta para produção**. Revise segurança, configuração e permissões antes de implantá-lo.

## Pré-requisitos

- SDK .NET definido em `global.json`.
- Docker Engine e Docker Compose v2.
- Senhas locais fortes do SQL Server, da assinatura JWT e do seed.

## Início rápido

```bash
cp .env.local.example .env.local
# Substitua SQLSERVER_SA_PASSWORD, JWT_SECRET e DEVELOPMENT_SEED_PASSWORD.
docker compose --env-file .env.local up --build -d
docker compose --env-file .env.local --profile tools up seed
curl -i http://localhost:8080/health/live
```

Para parar sem excluir dados: `docker compose --env-file .env.local down`.

Para executar no host, inicie SQL Server/Redis e configure User Secrets em `scripts/setup/configure-user-secrets.sh` (Bash) ou `.ps1` (PowerShell). Configure `ASPNETCORE_ENVIRONMENT=Development` e execute:

```bash
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
```

## Testes

```bash
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build
```

Os testes de integração exigem Docker/Testcontainers.

## Migrations e dados de desenvolvimento

O serviço `migrations` aplica os esquemas EF Core do Identity e SampleRestaurant. Para executar: `docker compose --env-file .env.local up migrations`.

O comando `--seed` é explícito, idempotente e bloqueado em Production. Para usar `dotnet ef`, passe `ConnectionStrings__DefaultConnection` na sessão do terminal: as factories de design-time não leem User Secrets da API.

## Configuração

- `AppSettings__Secret`: assinatura JWT obrigatória.
- `ConnectionStrings__DefaultConnection`: conexão SQL Server.
- `RedisCacheSettings__Enabled`: cache de resposta opcional.
- `OpenTelemetry__Enabled`: traces e métricas; OTLP configurado separadamente.
- `SeqSettings__Enabled`: log no Seq opcional.

Consulte `src/WebApiCoreSeed.Api/appsettings.json` e `compose.yaml`. A API possui `/health/live`, `/health/ready`, `/scalar` e endpoints `/api/v*/`.
