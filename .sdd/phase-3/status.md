# Status - Phase 3

| Prompt | Status |
| --- | --- |
| 01 - Testes unitarios | concluido |
| 02 - Testes de integracao | concluido |
| 03 - Seguranca | concluido |
| 04 - OpenTelemetry | pendente |
| 05 - CI e gates | pendente |

## Estado inicial do prompt 01

- Branch atual: `phase/3-quality-and-safety`
- Branch-base determinada: `phase/2-dotnet-10-migration`
- SHA inicial: `f35b72a2af01d46d07379d2b969b0e2f9c1c1196`
- Fase 2: concluida localmente em `.sdd/phase-2/status.md`
- Solution ativa: `RestauranteAPI.sln`
- Target framework ativo: `net10.0`
- Working tree inicial: limpa
- Baseline inicial: `dotnet test --configuration Release` passou com 34 testes

## Resultado do prompt 01

- Testes auditados: 34
- Testes finais: 36
- Testes unitarios finais: 23
- Testes HTTP existentes mantidos: 13
- Build/test final: passou
- Cobertura geral: 29,15% de linhas e 17,66% de branches
- Push: nao realizado

## Resultado do prompt 02

- Projeto criado: `test/WebApiCoreSeed.IntegrationTests`.
- Testes de integracao adicionados: 18.
- Containers usados: SQL Server `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04` e Redis `redis:7.4.2-alpine`.
- API inicializada com `WebApplicationFactory<Program>` em ambiente `Testing`.
- Migrations aplicadas automaticamente para `ApplicationDbContext` e `MeuDbContext`.
- Isolamento: collection xUnit compartilhada, sem paralelismo interno, reset SQL/Redis antes de cada teste.
- Build/test final: passou.
- Docker: disponivel.
- Push: nao realizado.

## Resultado do prompt 03

- CORS passou a usar `Cors:AllowedOrigins` e producao fica fechada quando nenhuma origin e configurada.
- `AllowAnyOrigin` foi removido do codigo ativo.
- Forwarded headers ficam desabilitados por padrao e so confiam em proxies/redes configurados quando `ForwardedHeaders:Enabled=true`.
- Headers obsoletos `X-XSS-Protection` e `Feature-Policy` foram removidos do middleware ativo.
- Foram adicionados/reforcados `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`, CSP com `frame-ancestors 'none'`, `X-Frame-Options` e no-store para respostas sensiveis.
- Request logging nao registra mais query string completa nem raw target.
- Health foi separado em `/health/live` e `/health/ready`; `/hc` permanece como alias legado minimalista.
- Request timeout e tamanho maximo de body ficaram explicitos por `RequestLimits`.
- Testes de integracao finais: 26.
- Build/test final: passou.
- `dotnet list package --vulnerable`: nenhum pacote vulneravel nas fontes atuais.
- Push: nao realizado.
