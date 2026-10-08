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

| P5-D012 | Publicar um pacote NuGet de tipo `Template` com ID `RodriOliveira.WebApiCoreSeed.Templates`, identidade `RodriOliveira.WebApiCoreSeed.CSharp` e short name `webapi-seed`. | Aceita | Permite instalar a partir de um pacote `.nupkg` local, sem depender da release #23. |
| P5-D013 | Utilizar `sourceName: WebApiCoreSeed` e gerar novo `UserSecretsId` ao instanciar o projeto. | Aceita | Atualiza nomes, namespaces, projetos, Docker e referencias, evitando compartilhar o ID de segredos do repositorio de origem. |
| P5-D014 | Empacotar mediante allowlist de projetos, tests, ferramentas, configuracoes e scripts, excluindo CI, SDD, documentacao de administracao e artefatos binarios. | Aceita | Reduz risco de vazamento e evita exigir detalhes internos do repositório na aplicacao gerada. |
| P5-D015 | Adiar flags `--auth`, `--database`, `--redis` e `--telemetry` de geracao, mantendo switches runtime existentes. | Aceita | Remocao de componentes deve atingir contratos, DI, dependencias, migracoes e testes; alterar apenas JSON seria inconsistente. |
| P5-D016 | Criar gate independente de smoke de pacote: pack, inspecao, instalacao, criacao, restore, build, testes unit/integracao e API live. | Aceita | Verifica o artefato que o consumidor recebera, e nao apenas o checkout do repositorio. |
