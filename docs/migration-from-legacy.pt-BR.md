# Guia de migração: .NET Core 3.1 para a aplicação .NET 10

[English](migration-from-legacy.md) | [Português (Brasil)](migration-from-legacy.pt-BR.md) | [Voltar ao README](../README.pt-BR.md) | [Baseline histórico](../LEGACY.md)

> Este documento é um **guia de comparação e planejamento**, não uma ferramenta automática de conversão de código ou banco de dados. Nenhuma implantação ou migração de dados em produção está implícita.

## Qual versão utilizar?

| Referência | Finalidade | Suporte |
| --- | --- | --- |
| [`v1.0.0-legacy`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/v1.0.0-legacy) | Snapshot intocado do código original no commit `6ce03d7f` | .NET Core 3.1 — sem suporte |
| [`legacy/netcoreapp3.1`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/legacy/netcoreapp3.1) | Código legado com aviso de fim de suporte no README | Histórico |
| [`main`](https://github.com/rodri-oliveira-dev/web-api-core-seed/tree/main) | Exemplo .NET 10 mantido | Desenvolvimento ativo |

O suporte ao .NET Core 3.1 encerrou-se em **13 de dezembro de 2022**. Não utilize a versão legada em produção.

## Breaking changes e mapeamento

| Área | Legado | Atual |
| --- | --- | --- |
| Solução | `RestauranteAPI.sln`; `src/DevIO.*` e `test/Pedidos.Test` | `WebApiCoreSeed.slnx`; `src/WebApiCoreSeed.Api`, `src/Modules/*` e `tests/*` |
| Hospedagem | ASP.NET Core 3.1 `Startup` / `Program` | .NET 10 `WebApplication`; `AddApiServices` / `UseApiPipeline` |
| Documentação HTTP | Swashbuckle / Swagger UI | OpenAPI nativo + Scalar e documentos versionados |
| Autenticação | Identity/JWT legado | Identity/JWT .NET 10; login v2; v1 existente, mas depreciado |
| Rate limiting | `AspNetCoreRateLimit` | Políticas nativas ASP.NET Core |
| Erros | Envelopes personalizados legados | `ProblemDetails` padronizado nos fluxos de erro |
| Organização | Camadas business/data e repositories genéricos | Núcleo `SampleRestaurant` com portas explícitas, infraestrutura e UoW |
| Persistência | Contextos EF Core 3.1 históricos | Contextos EF Core 10 separados em módulos de Infrastructure |
| Ambiente local | Serviços manuais com inconsistências históricas | `compose.yaml`, scripts de secrets, `--seed` explícito e determinístico |
| Observabilidade | Serilog / Seq opcional | Serilog, OpenTelemetry, OTLP/Seq opcionais |
| CI e testes | Validação histórica limitada | Testes unitários/integração, CodeQL, Dependabot e quality gates |

## Checklist de migração

1. **Inventariar contratos.** Registre rotas, métodos, issuer/audience JWT, formatos de resposta e consumidores. Compare com `docs/openapi/openapi-v1.json` e `openapi-v2.json`; o contrato de autenticação pode mudar.
2. **Preservar dados.** Faça backup do SQL Server legado e inventarie migrations específicas. Não renomeie colunas/tabelas por motivos de nomenclatura sem compatibilidade.
3. **Preparar o código atual.** Utilize o SDK .NET 10 de `global.json`. Configure variáveis/User Secrets. Paths e scripts antigos de `DevIO` não funcionam na árvore nova.
4. **Revisar evolução do esquema.** Migrations Identity e SampleRestaurant pertencem a projetos de infraestrutura separados. Revise SQL e testes de upgrade antes de atualizar um banco real; veja [comandos EF Core](development/ef-core-migrations.pt-BR.md).
5. **Validar segurança.** Reavalie CORS, secrets, claims, emissor/audiência JWT, rate limiting, cache e forwarded headers. Não carregue configurações permissivas do legado sem revisão.
6. **Comparar comportamentos.** Rode testes unitários, integração com Testcontainers e geração OpenAPI. Verifique login, endpoints protegidos, paginação e erros.
7. **Planejar implantação em separado.** O Compose deste repositório é para desenvolvimento. Defina rollback, rotação de secrets e observabilidade por ambiente de destino.

## Observações de compatibilidade

- Nomes históricos de migrations permanecem relevantes; o esquema atual preserva nomenclaturas SQL por compatibilidade quando necessário.
- O seed cria dados locais demonstrativos; **não** migra dados de clientes.
- Dois contextos podem usar um único SQL Server local, mas mantêm propriedade distinta de suas migrations.
- Empacotamento `dotnet new` [#22](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/22) e release [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23) continuam pendentes.
- A situação histórica de restore/build/test do legado consta em [LEGACY.md](../LEGACY.md); ausência de erro registrado não representa compatibilidade testada.

## Fontes de verdade

- [Arquitetura atual](architecture.pt-BR.md)
- [Análise histórica](../LEGACY.md)
- [Estratégia de migrations](../.sdd/phase-4/06-infrastructure-migrations/migration-inventory.md)
- [Quality gates](quality-gates.md)
- [Ambiente local](development/containerized-local-development.md)