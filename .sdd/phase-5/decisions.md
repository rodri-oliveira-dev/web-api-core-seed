# Decisions - Phase 5

| ID | Decisao | Status | Justificativa |
| --- | --- | --- | --- |
| P5-D001 | Usar `dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed` como interface unica do seed. | Aceita | Mantem a API como composition root e evita endpoint administrativo ou seed automatico. |
| P5-D002 | Bloquear o seed em `Production` antes de migrations e persistencia. | Aceita | O seed e exclusivamente local/desenvolvimento. |
| P5-D003 | Exigir senha por configuracao externa em `DevelopmentSeed:User:Password`. | Aceita | Evita segredo versionado e usa User Secrets ou variaveis locais. |
| P5-D004 | Usar GUIDs deterministicas para dados do `SampleRestaurant`. | Aceita | Garante upsert previsivel sem depender de `AnyAsync()` na tabela inteira. |
| P5-D005 | Usar transacao implicita de um unico `SaveChangesAsync` para o SampleRestaurant. | Aceita | A fronteira atomica real e local a um DbContext; transacao explicita gerava warning com MARS nas connection strings locais. |
| P5-D006 | Criar a issue #33 porque `ISSUE_URL` veio como placeholder e nao havia issue existente com o titulo esperado. | Aceita | Permite PR com `Closes #33` conforme entrega obrigatoria. |
| P5-D007 | Criar a issue #35 porque `ISSUE_URL` veio como placeholder e nao havia issue existente com o titulo esperado. | Aceita | Permite PR com `Closes #35` conforme entrega obrigatoria. |
