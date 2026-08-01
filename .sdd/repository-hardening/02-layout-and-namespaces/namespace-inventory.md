# Namespace Inventory

| Projeto | Assembly | Root namespace | Namespace encontrado | Namespace desejado | Acao | Risco |
| --- | --- | --- | --- | --- | --- | --- |
| `WebApiCoreSeed.Api` | `WebApiCoreSeed.Api` | default: `WebApiCoreSeed.Api` | `WebApiCoreSeed.Api.*` | `WebApiCoreSeed.Api.*` | Manter | Baixo; pasta permanece. |
| `WebApiCoreSeed.SampleRestaurant` | `WebApiCoreSeed.SampleRestaurant` | default: `WebApiCoreSeed.SampleRestaurant` | `WebApiCoreSeed.SampleRestaurant.*` | `WebApiCoreSeed.SampleRestaurant.*` | Manter namespaces; mover pasta do projeto | Medio; project references precisam ser atualizadas. |
| `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `WebApiCoreSeed.SampleRestaurant.Infrastructure` | default: `WebApiCoreSeed.SampleRestaurant.Infrastructure` | `WebApiCoreSeed.SampleRestaurant.Infrastructure.*` | `WebApiCoreSeed.SampleRestaurant.Infrastructure.*` | Manter namespaces; mover pasta do projeto | Medio; EF design-time e migrations devem continuar resolvendo. |
| `WebApiCoreSeed.Identity.Infrastructure` | `WebApiCoreSeed.Identity.Infrastructure` | default: `WebApiCoreSeed.Identity.Infrastructure` | `WebApiCoreSeed.Identity.Infrastructure.*` | `WebApiCoreSeed.Identity.Infrastructure.*` | Manter namespaces; mover pasta do projeto | Medio; API/testes/tooling referenciam o caminho antigo. |
| `WebApiCoreSeed.Tests` | `WebApiCoreSeed.Tests` | default: `WebApiCoreSeed.Tests` | `WebApiCoreSeed.Tests.*` | `WebApiCoreSeed.UnitTests.*` | Renomear projeto, arquivo `.csproj`, pasta e namespaces | Medio; filtros por namespace e discovery podem mudar. |
| `WebApiCoreSeed.IntegrationTests` | `WebApiCoreSeed.IntegrationTests` | default: `WebApiCoreSeed.IntegrationTests` | `WebApiCoreSeed.IntegrationTests.*` | `WebApiCoreSeed.IntegrationTests.*` | Mover de `test/` para `tests/` | Baixo; namespace ja distingue integracao. |
| `OpenApiGenerator` | `OpenApiGenerator` | default: `OpenApiGenerator` | Top-level statements, sem namespace declarado | Manter | Manter em `tools/OpenApiGenerator` | Baixo; apenas project reference precisa acompanhar Identity. |

## Namespaces de teste a atualizar

- `WebApiCoreSeed.Tests.Arquitetura` -> `WebApiCoreSeed.UnitTests.Arquitetura`.
- `WebApiCoreSeed.Tests.Integracao` -> `WebApiCoreSeed.UnitTests.Integracao`.
- `WebApiCoreSeed.Tests.Unitarios.*` -> `WebApiCoreSeed.UnitTests.Unitarios.*`.

## Namespaces produtivos

Todos os namespaces produtivos ativos ja usam o prefixo `WebApiCoreSeed`. Nao ha renomeacao publica planejada para API, modulo de sample, infraestrutura do sample ou infraestrutura de Identity.
