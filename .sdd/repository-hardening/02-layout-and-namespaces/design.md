# Design

## Abordagem

A normalizacao deve ser fisica e nominal, preservando os limites logicos criados na Fase 4.

- Nao dividir Domain e Application em novos projetos.
- Nao alterar portas, casos de uso, repositories, controllers, configuracao funcional ou migrations.
- Usar `git mv` para preservar historico dos arquivos.
- Atualizar referencias usando os novos caminhos relativos.
- Renomear apenas o projeto unitario/leves para `WebApiCoreSeed.UnitTests`.

## Projetos produtivos

- `WebApiCoreSeed.Api` permanece em `src/WebApiCoreSeed.Api`.
- `WebApiCoreSeed.SampleRestaurant` vai para `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant`.
- `WebApiCoreSeed.SampleRestaurant.Infrastructure` vai para `src/Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- `WebApiCoreSeed.Identity.Infrastructure` vai para `src/Modules/Identity/WebApiCoreSeed.Identity.Infrastructure`.

## Testes

- `test/` passa a ser `tests/`.
- `WebApiCoreSeed.Tests` passa a ser `WebApiCoreSeed.UnitTests`.
- `WebApiCoreSeed.IntegrationTests` mantem nome e namespace.
- Testes arquiteturais continuam no projeto unitario porque validam regras de dependencia por reflexao sem infraestrutura real.

## Tooling

`OpenApiGenerator` permanece em `tools/OpenApiGenerator`. O nome atual identifica bem uma ferramenta operacional, e a pasta ja corresponde ao projeto. A unica alteracao necessaria e atualizar references para os projetos movidos.

## Riscos e mitigacoes

| Risco | Mitigacao |
| --- | --- |
| Caminhos antigos no CI ou hooks | `rg` final para referencias vivas antigas. |
| Solution perder projetos ou pastas virtuais | `dotnet sln list` e build da solution. |
| Migrations perderem associacao | Factories e `MigrationsAssembly(typeof(...).Assembly.FullName)` preservados; validar EF CLI. |
| OpenAPI mudar por efeito colateral | Regenerar contratos e comparar `docs/openapi/openapi-v*.json`. |
