# Naming Inventory - Prompt 02

| Nome | Caminho | Origem | Classificacao | Acao | Novo nome | Impacto | Compatibilidade |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `WebApiCoreSeed.sln` | raiz | Legado/POC | Reusable tooling com nome de negocio | Renomear | `WebApiCoreSeed.sln` | Workflows, workspace e docs operacionais | Sem impacto HTTP |
| `WebApiCoreSeed.Api` | `src/WebApiCoreSeed.Api` | Legado/POC | Composition Root | Renomear assembly/namespace/projeto | `WebApiCoreSeed.Api` | Usings, testes, OpenAPI generator, XML docs | Rotas preservadas |
| `WebApiCoreSeed.SampleRestaurant` | `src/SampleRestaurant` | Legado/POC | Sample | Renomear assembly/namespace/projeto | `WebApiCoreSeed.SampleRestaurant` | Dominio, aplicacao, testes unitarios, migrations snapshot | Contratos HTTP preservados |
| `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `src/SampleRestaurant.Infrastructure` | Legado/POC | Sample | Renomear assembly/namespace/projeto | `WebApiCoreSeed.SampleRestaurant.Infrastructure` | DbContext, mappings, repositorios, testes de integracao | Schema preservado |
| `WebApiCoreSeed.Api` | `src/WebApiCoreSeed.Api`, solution folder | POC anterior | Composition Root | Renomear diretorio | `WebApiCoreSeed.Api` | Project references e VS Code | Sem impacto runtime |
| `SampleRestaurant` | `src/SampleRestaurant` | POC anterior | Sample | Renomear diretorio | `SampleRestaurant` | Project references | Sem impacto runtime |
| `SampleRestaurant.Infrastructure` | `src/SampleRestaurant.Infrastructure` | POC anterior | Sample | Renomear diretorio | `SampleRestaurant.Infrastructure` | Project references | Sem impacto runtime |
| `Restaurant` | `Modules/SampleRestaurant` | Prompt 01 | Sample | Tornar explicito como exemplo | `SampleRestaurant` | Paths, testes arquiteturais, SDD | Sem impacto HTTP |
| `SampleRestaurantDbContext` | Data, API, testes, tool, migrations | Legado/POC | Sample Infrastructure | Renomear tipo | `SampleRestaurantDbContext` | DI, EF Core, migrations metadata, testes | Migration operations preservadas |
| `WebApiCoreSeed.Tests` | `test/WebApiCoreSeed.Tests` | Dominio de exemplo | Tests | Renomear projeto/namespace | `WebApiCoreSeed.Tests` | Solution, test namespaces, docs | Test discovery preservado |
| `SampleRestaurantDb` | appsettings e testes | Legado/sample | Sample Infrastructure | Renomear banco default | `SampleRestaurantDb` | Configuracao local e testes | Nao altera contratos HTTP |
| `WebApiCoreSeed` issuer/email test | Testes | Legado/sample | Tests | Neutralizar ou sample-explicitar | `WebApiCoreSeed` / `example.local` | JWT de teste | Sem impacto produtivo |
| `Sample Restaurant API` | OpenAPI ativo/baseline | Sample contract docs | Sample | Explicitar como exemplo | `Sample Restaurant API` | OpenAPI JSON | Mudanca documental registrada |
| `Datasul` | `LEGACY.md`, SDD antigo | Historico | Obsolete | Manter apenas historico | N/A | Nenhum codigo ativo | Contextual |
| `restaurante.sql` | `sql/restaurante.sql` | Script legado do sample | Sample/historico | Manter nesta entrega e documentar | Futuro | Fora do escopo ativo | Sem uso automatico |
