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
