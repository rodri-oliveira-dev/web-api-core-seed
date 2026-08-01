# Decisions - Phase 3

| ID | Decision | Status | Rationale |
| --- | --- | --- | --- |
| D001 | Criar `phase/3-quality-and-safety` a partir de `phase/2-dotnet-10-migration`. | Accepted | `main` ainda nao contem a Fase 2 localmente; a Fase 3 deve partir do commit final da Fase 2. |
| D002 | Manter cobertura como baseline informativa, sem threshold nesta entrega. | Accepted | O objetivo e confiabilidade dos testes, nao aumento artificial de linhas cobertas. |
| D003 | Auditar todos os testes do projeto `Pedidos.Test`, classificando os testes HTTP existentes como nao unitarios no inventario. | Accepted | O prompt pede todos os projetos de teste, mas a entrega tecnica deve focar a suite unitaria existente. |
| D004 | Preservar a linguagem de dominio em portugues nos DisplayNames e adotar nomes de metodo no formato `MetodoQuandoCondicaoDeveResultado`. | Accepted | A variante sem sublinhados mantem clareza e evita novos warnings `CA1707` dos analyzers. |
| D005 | Preferir `Notificador` real nos testes de service que precisam validar notificacoes. | Accepted | Mockar `TemNotificacao()` pode gerar falso positivo porque o service chama `Handle`, nao `TemNotificacao`. |
| D006 | Corrigir `PedidoPratoValidation.Observacao` para permitir texto opcional ate 1000 caracteres. | Accepted | O teste novo reproduziu que uma observacao curta era rejeitada; a regra e a mensagem existente indicam limite maximo, nao proibicao de preenchimento. |
| D007 | Nao adicionar threshold de cobertura neste prompt. | Accepted | A baseline ainda inclui codigo legado, migrations e testes HTTP; qualidade comportamental e mais importante que percentual isolado nesta entrega. |
| D008 | Criar projeto dedicado `WebApiCoreSeed.IntegrationTests` para testes com infraestrutura real. | Accepted | Mantem a suite unitaria separada e permite filtrar testes que exigem Docker por `Category=Integration` e `Category=Container`. |
| D009 | Usar Testcontainers com SQL Server `2022-CU14-ubuntu-22.04` e Redis `7.4.2-alpine`. | Accepted | Tags explicitas melhoram reprodutibilidade e evitam dependencia de servicos locais provisionados manualmente. |
| D010 | Compartilhar containers por collection xUnit e limpar SQL/Redis antes de cada teste. | Accepted | Reduz custo de execucao sem permitir dependencia de dados deixados por outro teste. |
| D011 | Sobrescrever configuracao de teste por variaveis de ambiente restauradas no teardown. | Accepted | O `Program` registra servicos cedo no hosting moderno; os overrides precisam existir antes de `AddApiServices` ler `builder.Configuration`. |
| D012 | Ignorar `Atendente.Email` e `Atendente.Telefone` no mapping EF atual. | Accepted | As migrations legadas de `Atendentes` nao possuem essas colunas e SQL Server real expos `Telefone` como tipo sem chave no EF Core 10. |
| D013 | Configurar CORS por `Cors:AllowedOrigins` e negar origins quando a lista estiver vazia. | Accepted | Producao nao deve usar wildcard por padrao; apps sem browser continuam podendo chamar a API sem depender de CORS. |
| D014 | Rejeitar literal `*` em `Cors:AllowedOrigins` e manter `AllowCredentials=false` por padrao. | Accepted | Evita a combinacao perigosa entre wildcard e credenciais e forca origins explicitas. |
| D015 | Manter forwarded headers desabilitados por padrao e exigir proxies/redes conhecidos quando habilitados em producao. | Accepted | A infraestrutura final ainda nao esta definida; confiar indiscriminadamente em `X-Forwarded-For` e `X-Forwarded-Proto` permitiria spoofing. |
| D016 | Substituir `Feature-Policy` por `Permissions-Policy` e remover `X-XSS-Protection` do middleware ativo. | Accepted | Ambos sao obsoletos; CSP e Permissions-Policy cobrem o comportamento moderno esperado. |
| D017 | Expor `/health/live` e `/hc` apenas com status agregado e reservar detalhes de `/health/ready` para Development/Testing. | Accepted | Health publico deve ser util para operacao sem revelar nomes/status internos em producao. |
| D018 | Configurar `RequestTimeouts` e limite de body via `RequestLimits`. | Accepted | Os limites ficam explicitos e parametrizaveis sem antecipar WAF, gateway ou infraestrutura de producao. |
| D019 | Centralizar OpenTelemetry em `AddApiOpenTelemetry` usando SDK oficial e OTLP opcional. | Accepted | Mantem observabilidade vendor-neutral sem exigir collector ou stack externa para startup local. |
| D020 | Manter Serilog como pipeline unico de logs e adicionar apenas correlacao `TraceId`/`SpanId`. | Accepted | Evita duplicar todos os logs entre Serilog e OpenTelemetry logs provider sem uma decisao operacional clara. |
| D021 | Usar `OpenTelemetry.Instrumentation.EntityFrameworkCore` `1.17.0-beta.1`. | Accepted | EF Core e criterio de aceite explicito; nao havia versao estavel disponivel no NuGet durante o prompt. |
| D022 | Nao adicionar instrumentacao Redis neste prompt. | Accepted | A instrumentacao StackExchange.Redis disponivel e pre-release e requer o `IConnectionMultiplexer`; a aplicacao ativa usa `AddStackExchangeRedisCache`, que nao expoe esse contrato sem redesenho. |
| D023 | Renomear configuracao ativa de Seq para `SeqSettings` e manter Seq opcional. | Accepted | Remove acoplamento de nomenclatura legado e preserva integracao opcional existente. |
