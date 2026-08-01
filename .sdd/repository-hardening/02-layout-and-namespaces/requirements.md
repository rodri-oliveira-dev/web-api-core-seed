# Requirements

## Objetivo

Normalizar a organizacao fisica e nominal da solution ativa sem alterar arquitetura logica, regras de negocio, contratos HTTP, OpenAPI ou schema de banco.

## Escopo

- Mover projetos ativos de modulo para `src/Modules`.
- Renomear a pasta raiz de testes de `test/` para `tests/`.
- Renomear o projeto unitario/leves de `WebApiCoreSeed.Tests` para `WebApiCoreSeed.UnitTests`.
- Atualizar project references, solution, CI, hooks, VS Code, documentacao ativa e SDD de handoff/status.
- Preservar `WebApiCoreSeed.Api`, `WebApiCoreSeed.SampleRestaurant`, `WebApiCoreSeed.SampleRestaurant.Infrastructure` e `WebApiCoreSeed.Identity.Infrastructure` como assemblies.
- Preservar migrations existentes, IDs de migration, nomes de classes e operacoes.
- Manter `tools/OpenApiGenerator` quando nao houver ganho claro em mover ou renomear.

## Fora de escopo

- Separar Domain e Application em novos projetos.
- Alterar regras de negocio, controllers, contratos HTTP ou schema.
- Alterar pacotes ou target frameworks.
- Reescrever migrations antigas apenas por estetica.
- Introduzir SLNX, Central Package Management ou analyzers.

## Criterios de aceite

- `tests/` existe e substitui `test/` para projetos ativos.
- Projetos de teste distinguem Unit e Integration.
- Pastas de projetos correspondem aos respectivos projetos.
- Modulos de negocio ficam agrupados sob `src/Modules`.
- Namespaces de testes correspondem ao novo assembly unitario.
- Referencias vivas a caminhos anteriores sao removidas.
- Ocorrencias historicas remanescentes ficam documentadas como historicas.
- Migrations continuam associadas aos mesmos DbContexts.
- OpenAPI gerado nao muda.
- Testes arquiteturais continuam passando.
