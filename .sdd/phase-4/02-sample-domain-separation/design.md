# Design - Prompt 02

## Decisoes de nomenclatura

| Papel | Nome |
| --- | --- |
| Solution | `WebApiCoreSeed.sln` |
| Composition root/API | `WebApiCoreSeed.Api` |
| Dominio e aplicacao do exemplo | `WebApiCoreSeed.SampleRestaurant` |
| Infraestrutura do exemplo | `WebApiCoreSeed.SampleRestaurant.Infrastructure` |
| Modulo fisico | `Modules/SampleRestaurant` |
| DbContext do exemplo | `SampleRestaurantDbContext` |
| Testes unitarios/leves | `WebApiCoreSeed.Tests` |
| Testes de integracao | `WebApiCoreSeed.IntegrationTests` |

## Estrutura alvo

```text
src/
  WebApiCoreSeed.Api/
  SampleRestaurant/
    Modules/SampleRestaurant/
      Domain/
      Application/
  SampleRestaurant.Infrastructure/
    Modules/SampleRestaurant/
      Infrastructure/Persistence/
test/
  WebApiCoreSeed.Tests/
  WebApiCoreSeed.IntegrationTests/
tools/
  OpenApiGenerator/
```

## Compatibilidade

- Rotas publicas existentes serao preservadas, incluindo `/api/v{version}/Pratos`, `/api/v{version}/Mesas`, `/api/v1/entrar`, `/api/v2/entrar` e `/hc`.
- Payloads e status codes nao serao alterados por renomeacao interna.
- Migrations antigas permanecem na pasta atual do projeto de infraestrutura do sample; apenas namespaces, atributos `[DbContext]` e strings de snapshot serao ajustados.
- O titulo OpenAPI sera atualizado para `Sample Restaurant API`, deixando claro que se trata de dominio demonstrativo.
- `Datasul` ficara apenas em documentos historicos.

## Regras arquiteturais

- `WebApiCoreSeed.SampleRestaurant` nao pode referenciar `WebApiCoreSeed.Api` nem `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- `WebApiCoreSeed.SampleRestaurant.Infrastructure` referencia o nucleo do sample e nao referencia API.
- `WebApiCoreSeed.Api` referencia o nucleo e a infraestrutura do sample apenas para composicao.
- Nenhum tipo do dominio de exemplo sera movido para Shared Kernel.
