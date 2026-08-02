# Handoff

## Estado entregue pelo Prompt 5

- Solution ativa migrada para `WebApiCoreSeed.slnx` pelo comando oficial do SDK.
- O arquivo de solution anterior foi removido somente depois da equivalencia entre as listas de projetos da solution antiga e da nova.
- A `.slnx` preserva 7 projetos ativos:
  - API;
  - SampleRestaurant business;
  - SampleRestaurant infrastructure;
  - Identity infrastructure;
  - unit tests;
  - integration tests;
  - OpenApiGenerator.
- Folders logicos preservados: `/src/`, `/src/WebApiCoreSeed.Api/`, `/src/Modules/`, `/src/Modules/SampleRestaurant/`, `/src/Modules/Identity/`, `/tests/` e `/tools/`.
- Referencias ativas atualizadas em `AGENTS.md`, `README.md`, VS Code, workspace, GitHub workflows, CODEOWNERS, template de PR, hooks, docs operacionais, skills locais e factories EF design-time.
- `ApplicationDbContextFactory` e `SampleRestaurantDbContextFactory` agora localizam a raiz pelo arquivo SLNX.
- OpenAPI generator atualizou `docs/openapi/openapi-v1.json` e `docs/openapi/openapi-v2.json`; os contratos foram mantidos sincronizados com a saida atual do gerador.

## Validacoes do Prompt 5

- `dotnet restore WebApiCoreSeed.slnx`: passou.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passou com 30 warnings `CA*` historicos e 0 erros.
- `dotnet test WebApiCoreSeed.slnx --configuration Release --no-build`: passou com 95 testes.
- Testes unitarios isolados: 53 aprovados.
- Testes de integracao isolados com `Category=Integration`: 42 aprovados.
- Testes arquiteturais com `Architecture=ModularHexagonal`: 7 aprovados.
- OpenAPI generator: passou e regenerou contratos.
- JSON OpenAPI: sintaticamente valido.
- Workflow YAML: sintaticamente valido com PyYAML.
- `scripts/setup/configure-git-hooks.ps1 -Check`: passou.
- `.githooks/pre-push`: passou usando SLNX via Git for Windows shell.
- `git diff --check`: passou; manteve apenas aviso de normalizacao LF para `README.md`.
- Varredura final: nenhuma referencia ativa ao arquivo antigo fora de SDD historico ou documentacao do proprio Prompt 5.

## Proximo prompt

Prompt 6 deve adotar CSF.Analyzers sem reverter a migracao SLNX. Use `WebApiCoreSeed.slnx` em restore, build, test, `dotnet list` e qualquer novo gate.

## Riscos para acompanhar

- Nao recriar o arquivo antigo da solution ao usar ferramentas antigas de IDE.
- Alguns SDDs de prompts anteriores ainda citam comandos e estados antigos; trate-os como historico, nao como instrucao operacional atual.
- Os 30 warnings `CA*` permanecem divida conhecida para prompt proprio.
- O aviso de normalizacao de `README.md` vem da politica de LF e nao bloqueou `git diff --check`.
