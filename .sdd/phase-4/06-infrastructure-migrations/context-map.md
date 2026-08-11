# Context Map - Prompt 06

| Contexto | Limite | Assembly de runtime | Assembly de migrations | Startup project | Connection string |
| --- | --- | --- | --- | --- | --- |
| `ApplicationDbContext` | Identity persistence | `WebApiCoreSeed.Identity.Infrastructure` | `WebApiCoreSeed.Identity.Infrastructure` | `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | `ConnectionStrings:DefaultConnection` |
| `SampleRestaurantDbContext` | SampleRestaurant persistence | `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj` | `ConnectionStrings:DefaultConnection` |

## Dependencias

- API referencia `WebApiCoreSeed.Identity.Infrastructure` e `WebApiCoreSeed.SampleRestaurant.Infrastructure` para composicao.
- `WebApiCoreSeed.Identity.Infrastructure` nao referencia API.
- `WebApiCoreSeed.SampleRestaurant.Infrastructure` continua referenciando apenas `WebApiCoreSeed.SampleRestaurant`.
- `WebApiCoreSeed.SampleRestaurant` continua sem dependencia de infraestrutura ou API.

## Observacoes

- Identity segue como capacidade imatura, mas sua persistencia deixa de estar fisicamente na API.
- O sample nao recebe migrations de Identity.
