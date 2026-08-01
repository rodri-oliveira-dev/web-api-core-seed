# Current Layout

Estado observado antes da normalizacao:

```text
src/
|-- Identity.Infrastructure/
|   `-- WebApiCoreSeed.Identity.Infrastructure.csproj
|-- SampleRestaurant/
|   `-- WebApiCoreSeed.SampleRestaurant.csproj
|-- SampleRestaurant.Infrastructure/
|   `-- WebApiCoreSeed.SampleRestaurant.Infrastructure.csproj
`-- WebApiCoreSeed.Api/
    `-- WebApiCoreSeed.Api.csproj

test/
|-- WebApiCoreSeed.Tests/
|   `-- WebApiCoreSeed.Tests.csproj
`-- WebApiCoreSeed.IntegrationTests/
    `-- WebApiCoreSeed.IntegrationTests.csproj

tools/
`-- OpenApiGenerator/
    `-- OpenApiGenerator.csproj
```

## Problemas

- `test/` no singular contraria a convencao alvo.
- `WebApiCoreSeed.Tests` nao distingue claramente testes unitarios/leves de testes de integracao.
- Projetos de modulo ficam diretamente em `src/`, sem agrupamento por modulo/capacidade.
- Pastas fisicas `SampleRestaurant`, `SampleRestaurant.Infrastructure` e `Identity.Infrastructure` nao correspondem integralmente aos nomes dos projetos.
- Referencias ativas em CI, hooks e documentacao ainda apontam para o layout anterior.
