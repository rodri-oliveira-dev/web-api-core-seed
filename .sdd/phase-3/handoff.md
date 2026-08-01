# Handoff - Phase 3

## Estado atual

- Branch atual: `phase/3-quality-and-safety`.
- Branch-base: `phase/2-dotnet-10-migration`.
- Prompt atual: `03 - Security hardening`.
- Issue atual: `#9`.
- Status do prompt 01: concluido.
- Status do prompt 02: concluido.
- Status do prompt 03: concluido.
- Commit do prompt 01: `test: strengthen existing unit test suite`.
- Commit do prompt 02: `test: add API and infrastructure integration tests`.
- Commit do prompt 03: `fix: harden API security defaults`.

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

## Breaking changes e configuracao de producao

- Aplicacoes browser precisam definir `Cors:AllowedOrigins`.
- Ambientes atras de proxy devem configurar `ForwardedHeaders:Enabled`, `KnownProxies` ou `KnownNetworks`.
- Valores locais de connection string/JWT em `appsettings.json` agora sao placeholders e devem ser sobrescritos fora do desenvolvimento local.

## Proxima issue

`#10` - executar o Prompt 4 da Fase 3, iniciando OpenTelemetry.
