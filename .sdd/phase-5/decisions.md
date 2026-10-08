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

| P5-D008 | Tratar `README.md` como guia principal em ingles e `README.pt-BR.md` como traducao estruturalmente alinhada. | Aceita | Garante onboarding navegavel em EN/PT-BR e reduz instrucoes historicas desatualizadas. Issue #21, PR #50. |
| P5-D009 | Registrar ADRs 0001-0003 como retrato das decisoes implementadas: portas explicitas e modularidade pragmatica; migrations por DbContext e seed explicito; cache compartilhado restrito e telemetria opt-in. | Aceita | Documenta limites reais sem inventar microsservicos, release ou pacote `dotnet new`. |
| P5-D010 | Exigir validacao automatizada de links internos, anchors e hierarquia dos guias bilingues via `scripts/validate-docs.py` no CI. | Aceita | Evita regressao silenciosa de navegacao e paridade do onboarding. |
| P5-D011 | Em `dotnet ef`, fornecer `ConnectionStrings__DefaultConnection` pela variavel de ambiente temporaria do terminal, sem alterar as factories de design-time neste PR documental. | Aceita | As factories do Identity e SampleRestaurant leem JSON + ambiente, mas nao os User Secrets da API; README e guias EF EN/PT-BR explicam a diferenca e uso local sem registrar secrets. Review #5461397982. |
