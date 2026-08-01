# Target Layout

Estrutura alvo adotada:

```text
src/
|-- WebApiCoreSeed.Api/
|   `-- WebApiCoreSeed.Api.csproj
`-- Modules/
    |-- Identity/
    |   `-- WebApiCoreSeed.Identity.Infrastructure/
    |       `-- WebApiCoreSeed.Identity.Infrastructure.csproj
    `-- SampleRestaurant/
        |-- WebApiCoreSeed.SampleRestaurant/
        |   `-- WebApiCoreSeed.SampleRestaurant.csproj
        `-- WebApiCoreSeed.SampleRestaurant.Infrastructure/
            `-- WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj

tests/
|-- WebApiCoreSeed.UnitTests/
|   `-- WebApiCoreSeed.UnitTests.csproj
`-- WebApiCoreSeed.IntegrationTests/
    `-- WebApiCoreSeed.IntegrationTests.csproj

tools/
`-- OpenApiGenerator/
    `-- OpenApiGenerator.csproj
```

## Decisoes de layout

- `WebApiCoreSeed.Api` permanece direto em `src/`, pois e composition root e entrada HTTP.
- `SampleRestaurant` fica agrupado como modulo de negocio sob `src/Modules/SampleRestaurant`.
- `Identity` fica agrupado sob `src/Modules/Identity` como infraestrutura transversal/autenticacao, preservando o assembly `WebApiCoreSeed.Identity.Infrastructure`.
- `OpenApiGenerator` permanece em `tools/OpenApiGenerator`, pois ja esta em tooling e a renomeacao nao melhora descobribilidade nem arquitetura.
- A solution deve refletir o agrupamento por solution folders `src`, `Modules`, `SampleRestaurant`, `Identity`, `tests` e `tools`.
