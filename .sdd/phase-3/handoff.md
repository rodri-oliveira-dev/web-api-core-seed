# Handoff - Phase 3

## Estado atual

- Branch atual: `phase/3-quality-and-safety`.
- Branch-base: `phase/2-dotnet-10-migration`.
- Prompt atual: `04 - OpenTelemetry baseline`.
- Issue atual: `#10`.
- Status do prompt 01: concluido.
- Status do prompt 02: concluido.
- Status do prompt 03: concluido.
- Status do prompt 04: concluido.
- Commit do prompt 01: `test: strengthen existing unit test suite`.
- Commit do prompt 02: `test: add API and infrastructure integration tests`.
- Commit do prompt 03: `fix: harden API security defaults`.
- Commit do prompt 04: pendente ate delivery.

## Resultado do prompt 03

- CORS:
  - `AllowAnyOrigin` removido.
  - `Cors:AllowedOrigins` controla origins permitidas.
  - Producao fica fechada quando nenhuma origin e configurada.
  - `*` e rejeitado como origin literal.
  - Credenciais seguem desabilitadas por padrao.
- Forwarded headers:
  - `ForwardedHeaders:Enabled=false` por padrao.
  - Quando habilitado, usa apenas `KnownProxies` e `KnownNetworks`.
  - Em producao, habilitar sem proxy/rede conhecida falha no startup.
- Headers:
  - Removidos do middleware ativo: `X-XSS-Protection`, `Feature-Policy`.
  - Adicionados/reforcados: `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`, CSP, `X-Frame-Options`, HSTS fora de Development e no-store para respostas sensiveis.
- Logging:
  - Serilog request logging nao grava query string completa.
  - Middleware customizado deixou de usar `RawTarget` e passa a registrar somente `Request.Path`.
  - Headers sensiveis seguem fora da whitelist de logging.
- Health:
  - `/health/live`: status agregado.
  - `/health/ready`: readiness com detalhes em Development/Testing, status agregado em producao.
  - `/hc`: alias legado com status agregado.
- Limites:
  - `RequestLimits:TimeoutSeconds` default 30 segundos.
  - `RequestLimits:MaxRequestBodyBytes` default 10 MB.
- Testes:
  - Suite completa passou com 36 testes em `Pedidos.Test` e 26 em `WebApiCoreSeed.IntegrationTests`.
  - Novos cenarios cobrem CORS permitido/negado, headers modernos, headers obsoletos ausentes, logging sem token/query sensivel, health minimo, readiness e no-store em auth.

## Validacoes oficiais

```text
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet list package --vulnerable
```

Resultados:

- `dotnet restore`: passou.
- `dotnet build --configuration Release --no-restore`: passou.
- `dotnet test --configuration Release --no-build`: passou.
- `dotnet list package --vulnerable`: nenhum pacote vulneravel nas fontes atuais.
- Docker estava disponivel.
- Push: nao realizado.

## Resultado do prompt 04

- OpenTelemetry:
  - Registro central em `src/DevIO.Api/Configuration/OpenTelemetryConfig.cs`.
  - Configuracao por `OpenTelemetry:*`.
  - `service.name`: `web-api-core-seed-api`.
  - `service.namespace`: `rodri-oliveira-dev.web-api-core-seed`.
  - `service.version`: configuracao ou assembly informational version.
  - Ambiente: `OpenTelemetry:Environment` ou ASP.NET Core environment.
- Instrumentacoes:
  - ASP.NET Core traces/metrics.
  - HttpClient traces/metrics.
  - EF Core traces.
  - Runtime metrics.
  - Meters de ASP.NET Core, Kestrel, HTTP, name resolution e EF Core.
- Exporters:
  - OTLP traces/metrics opcional.
  - `OpenTelemetry:Otlp:Enabled`, `OpenTelemetry:Otlp:Endpoint`, `OpenTelemetry:Otlp:Protocol`.
  - Tambem le `OTEL_SERVICE_NAME`, `OTEL_EXPORTER_OTLP_ENDPOINT` e `OTEL_EXPORTER_OTLP_PROTOCOL`.
- Logs:
  - Serilog preservado como pipeline unico.
  - Console/file com `TraceId` e `SpanId`.
  - Seq opcional por `SeqSettings`.
- Dados excluidos:
  - Sem baggage customizado.
  - Sem labels de alta cardinalidade.
  - Sem enriquecimento SQL customizado.
  - Redacao de query forçada para instrumentacao ASP.NET Core/HttpClient.
- Redis:
  - Nao instrumentado neste prompt.
  - Motivo: pacote StackExchange.Redis pre-release e cache ativo nao expoe `IConnectionMultiplexer`.
- Testes:
  - `ObservabilityConfigurationTests` cobre startup desativado, startup com OTLP sem collector, span de request, correlacao de logs e ausencia de valores sensiveis em tags capturadas.
- Artefatos removidos:
  - `src/DevIO.Api/healthchecksdb`.
  - `src/DevIO.Api/teste.txt`.
- Validacoes finais:
  - `dotnet restore`: passou.
  - `dotnet build --configuration Release --no-restore`: passou com 21 avisos de analyzer existentes.
  - `dotnet test --configuration Release --no-build`: passou com 41 testes em `Pedidos.Test` e 26 em `WebApiCoreSeed.IntegrationTests`.
  - `dotnet list package`: passou.
  - `git grep -n -i "Datasul"` retorna apenas referencias historicas em `LEGACY.md` e SDD antigo.

## Breaking changes e configuracao de producao

- Aplicacoes browser precisam definir `Cors:AllowedOrigins`.
- Ambientes atras de proxy devem configurar `ForwardedHeaders:Enabled`, `KnownProxies` ou `KnownNetworks`.
- Valores locais de connection string/JWT em `appsettings.json` agora sao placeholders e devem ser sobrescritos fora do desenvolvimento local.

## Proxima issue

`#13` - executar o Prompt 5 da Fase 3, CI e quality gates.
