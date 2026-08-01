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
