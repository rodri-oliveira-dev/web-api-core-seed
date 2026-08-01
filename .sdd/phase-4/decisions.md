# Decisions - Phase 4

| ID | Decision | Status | Rationale |
| --- | --- | --- | --- |
| D001 | Criar `phase/4-architecture-modernization` a partir de `phase/3-quality-and-safety` no commit `18af517adab5d21ae58ac9674da411244a5379b9`. | Accepted | A Fase 3 esta concluida localmente e ainda nao ha evidencia de integracao em `main`. |
| D002 | Usar um unico modulo de negocio inicial chamado `Restaurant`. | Accepted | O dominio ativo gira em torno de pratos, mesas, pedidos e atendentes de restaurante; criar varios modulos agora seria artificial. |
| D003 | Tratar `WebApiCoreSeed.SampleRestaurant` como nucleo do modulo `Restaurant`, com subestruturas `Domain` e `Application` no mesmo assembly nesta entrega. | Accepted | Reduz risco e evita churn de namespaces/projetos antes da separacao definitiva do dominio de exemplo no Prompt 2. |
| D004 | Tratar `WebApiCoreSeed.SampleRestaurant.Infrastructure` como adaptador `Infrastructure` do modulo `Restaurant`. | Accepted | O projeto ja implementa EF Core, DbContext, mappings e repositorios concretos para o dominio ativo. |
| D005 | Manter `WebApiCoreSeed.Api` como adaptador de entrada e composition root. | Accepted | A API contem controllers, autenticacao, autorizacao, Problem Details, OpenAPI, rate limiting, health checks e DI. |
| D006 | Nao redesenhar o repositorio generico, Unit of Work, CancellationToken, migrations ou paginacao neste prompt. | Accepted | Esses itens pertencem aos prompts seguintes da Fase 4. |
| D007 | Preservar nomes de assemblies e namespaces publicos nesta entrega. | Accepted | A reorganizacao fisica cria o limite modular sem churn amplo nem quebra de testes/contratos. |
| D008 | Expor consultas de pratos e mesas nas portas de entrada existentes, em vez de criar handlers novos. | Accepted | Remove acoplamento direto dos controllers a repositorios sem criar abstracoes cerimoniais antes dos prompts de portas de persistencia. |
| D009 | Trocar `Microsoft.Extensions.Logging.LogLevel` por `ELogLevel` no dominio. | Accepted | Logging tecnico nao deve vazar para Domain; os valores numericos foram preservados para manter o schema e dados existentes. |
| D010 | Usar testes arquiteturais por reflexao no projeto `WebApiCoreSeed.Tests`. | Accepted | A solucao simples cobre as regras atuais sem adicionar framework pesado. |
| D011 | Usar `WebApiCoreSeed.*` para componentes reutilizaveis e composition root, e `WebApiCoreSeed.SampleRestaurant.*` para o dominio demonstrativo. | Accepted | A separacao fica explicita para novos consumidores sem criar projetos ou abstracoes artificiais. |
| D012 | Renomear `MeuDbContext` para `SampleRestaurantDbContext` mantendo migrations antigas no projeto de infraestrutura do sample. | Accepted | Remove nome legado em codigo ativo e preserva ownership/movimentacao de migrations para o Prompt 6. |
| D013 | Preservar rotas HTTP de exemplo em portugues, como `/Pratos` e `/Mesas`. | Accepted | Elas pertencem ao contrato do sample; neutraliza-las artificialmente quebraria compatibilidade sem ganho para o seed reutilizavel. |
| D014 | Renomear a solution ativa para `WebApiCoreSeed.sln`. | Accepted | `RestauranteAPI.sln` carregava nome de negocio no tooling central do seed. |
| D015 | Remover o repositorio generico legado em vez de criar adaptador temporario. | Accepted | A interface espelhava `DbSet`, expunha predicados arbitrarios e aumentava a superficie sem expressar intencao do dominio. |
| D016 | Manter portas especificas por entidade persistida ate a modelagem de aggregates ser refinada. | Accepted | O modulo ainda preserva services legados; reduzir a superficie agora evita churn maior antes de Unit of Work e refinamento DDD. |
| D017 | Manter commit implicito nos metodos de escrita dos repositories concretos ate o Prompt 4. | Accepted | O objetivo deste prompt e remover o generic repository; a fronteira transacional sera tratada separadamente para preservar comportamento observavel. |
| D018 | Remover `IPedidoRepository.ObterPedidoItens` nesta entrega. | Accepted | O metodo nao tinha consumidor produtivo encontrado e delegava para consulta por id sem carregar itens. |
| D019 | Manter pagina de pratos sem ordenacao deterministica por enquanto. | Accepted | A paginacao final pertence ao Prompt 7; esta entrega preserva o contrato e comportamento legado. |
| D020 | Criar `ISampleRestaurantUnitOfWork` como porta de saida do modulo `SampleRestaurant`. | Accepted | O contrato explicita que o limite transacional pertence ao `SampleRestaurantDbContext` e evita prometer coordenacao generica entre DbContexts. |
| D021 | Repositorios de escrita registram alteracoes e nao retornam linhas afetadas. | Accepted | Linhas afetadas pertencem ao commit; repository deve expressar a intencao de persistencia e nao confirmar a transacao. |
| D022 | Nao adicionar transacoes explicitas no Prompt 04. | Accepted | Um unico `SaveChangesAsync` por caso de uso ja e atomico para o provider relacional usado; transacoes explicitas seriam redundantes no estado atual. |
| D023 | Manter `ApplicationDbContext` de Identity fora da Unit of Work do sample. | Accepted | Identity possui contexto e stores proprios, e nao ha fluxo atual que grave Identity e SampleRestaurant no mesmo caso de uso. |
| D024 | Nao integrar domain events ao commit no Prompt 04. | Accepted | Nao existem domain events, interceptors ou outbox no codigo ativo; introduzir isso seria escopo novo. |
| D025 | Usar `CancellationToken cancellationToken` como ultimo parametro em contratos internos do sample. | Accepted | A convencao torna a propagacao explicita, preserva a origem HTTP e evita esconder token em estado global. |
| D026 | Manter token com default `default` em portas internas e implementacoes. | Accepted | Reduz churn em callers nao HTTP e testes legados, enquanto controllers e novos fluxos cancelaveis passam token explicitamente. |
| D027 | Nao transformar `OperationCanceledException` em Problem Details 500 nem log de erro inesperado. | Accepted | Cancelamento cooperativo e comportamento esperado quando o cliente encerra a request ou a operacao e cancelada. |
| D028 | Registrar APIs de Identity usadas em Auth como sem suporte direto a token nesta etapa. | Accepted | `UserManager` e `SignInManager` expoem os metodos usados sem `CancellationToken`; adaptar stores de Identity seria escopo novo. |
| D029 | Criar `WebApiCoreSeed.Identity.Infrastructure` para `ApplicationDbContext` e migrations de Identity. | Accepted | Remove migrations da API sem misturar Identity com a infraestrutura do modulo demonstrativo `SampleRestaurant`. |
| D030 | Manter migrations do `SampleRestaurantDbContext` em `WebApiCoreSeed.SampleRestaurant.Infrastructure`. | Accepted | O schema do sample pertence ao adaptador de persistencia do proprio modulo. |
| D031 | Preservar o schema legado de Identity com max length 128 para chaves de login/token. | Accepted | O Identity/EF Core 10 detectava alteracao de modelo sem essa configuracao; fixar o max length evita migration nova e preserva a migration historica. |
| D032 | Manter offset pagination em `GET /api/v{version}/Pratos`. | Accepted | O endpoint e um catalogo de exemplo de volume moderado; navegacao por paginas e simplicidade sao mais relevantes do que consistencia forte entre paginas. |
| D033 | Ordenar pratos por `Titulo` ascendente e `Id` ascendente. | Accepted | `Titulo` e adequado para catalogo; `Id` torna a ordenacao unica quando titulos se repetem. |
| D034 | Rejeitar `PageSize > 50` em vez de truncar silenciosamente. | Accepted | Erro explicito torna o contrato previsivel e evita que clientes acreditem ter recebido o tamanho solicitado. |
| D035 | Trocar o envelope paginado para `items/page/pageSize/totalItems/totalPages/hasNextPage/hasPreviousPage`. | Accepted | A metadata fica consistente e autoexplicativa; a mudanca de formato foi documentada como breaking change. |
| D036 | Adicionar indice `IX_Pratos_Titulo_Id`. | Accepted | O indice apoia a ordenacao estavel usada antes de `Skip` e `Take`. |
