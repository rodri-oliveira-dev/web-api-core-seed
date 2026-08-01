# AGENTS.md

## Objetivo

Este repositorio e um seed educacional e reutilizavel de Web API. O estado legado em .NET Core 3.1 foi preservado na Fase 1 para permitir comparacao durante a modernizacao incremental para .NET 10.

O trabalho deve ser pequeno, correto, reproduzivel e alinhado ao estado real do repositorio. Nao declare como existente algo que ainda esta apenas no roadmap.

## Fontes de Verdade

Leia somente o que for relevante para a tarefa, com prioridade para:

1. `README.md`
2. `LEGACY.md`
3. `.sdd/`
4. `RestauranteAPI.sln`
5. `src/DevIO.Api/Restaurante.IO.Api.csproj`
6. `src/DevIO.Business/Restaurante.IO.Business.csproj`
7. `src/DevIO.Data/Restaurante.IO.Data.csproj`
8. `test/Pedidos.Test/Pedidos.Test.csproj`
9. `.editorconfig`
10. `.vscode/`
11. `.github/`
12. `.githooks/`
13. `scripts/setup/`

## Estado Atual

- A solution atual e `RestauranteAPI.sln`.
- Todos os projetos ainda miram `netcoreapp3.1`.
- O projeto da API e `src/DevIO.Api/Restaurante.IO.Api.csproj`.
- Existem bibliotecas Business e Data em `src/DevIO.Business/` e `src/DevIO.Data/`.
- O projeto de testes atual e `test/Pedidos.Test/Pedidos.Test.csproj`.
- A arquitetura atual e a arquitetura legada em camadas do projeto original; ela ainda nao e um monolito modular moderno.
- O legado usa ASP.NET Core 3.1, Entity Framework Core 3.1, SQL Server, Redis, Identity, JWT, Swagger, health checks, rate limiting por pacote e Serilog.
- A versao historica esta preservada por `legacy/netcoreapp3.1` e `v1.0.0-legacy`.
- O ambiente registrado na Fase 1 nao tinha SDK/runtime .NET Core 3.1 e tinha bloqueio local de restore por cache NuGet invalido.

## Direcao Planejada

As seguintes decisoes sao direcao de modernizacao, nao implementacao concluida:

- migrar para .NET 10;
- evoluir para monolito modular;
- aplicar arquitetura Hexagonal dentro dos modulos;
- separar dominio de exemplo de componentes reutilizaveis;
- manter Controllers como adaptadores HTTP;
- usar Entity Framework Core e SQL Server no desenho moderno;
- usar Redis quando houver necessidade real;
- adotar Problem Details, rate limiting nativo e OpenAPI moderno;
- adicionar OpenTelemetry;
- ampliar testes unitarios;
- criar testes de integracao com Testcontainers quando a dependencia real justificar;
- criar testes HTTP com `WebApplicationFactory`;
- avaliar Aspire como orquestracao local opcional;
- preparar template instalavel por `dotnet new`.

## SDD

Toda mudanca relevante deve seguir:

1. Specification
2. Discovery
3. Design
4. Development
5. Validation
6. Delivery

Prompts podem ser executados em chats separados. O contexto compartilhado deve ser persistido em arquivos versionados, principalmente em `.sdd/`.

Antes de iniciar um prompt da Fase 2, leia `.sdd/phase-2/README.md`, `.sdd/phase-2/status.md`, `.sdd/phase-2/decisions.md`, `.sdd/phase-2/handoff.md` e a pasta especifica do prompt.

Atualize `status.md`, `handoff.md` e `decisions.md` quando a tarefa criar decisao, bloqueio, validacao ou proximo passo relevante. Cada prompt deve terminar com um unico commit semantico quando houver alteracao versionavel, salvo instrucao explicita diferente.

## Arquitetura

As regras abaixo sao direcionais para a modernizacao:

- dominio nao depende de infraestrutura;
- casos de uso nao dependem de ASP.NET Core;
- adaptadores implementam portas;
- modulos representam capacidades de negocio;
- um modulo nao acessa diretamente a infraestrutura interna de outro;
- Shared Kernel deve permanecer minimo;
- nao crie abstracoes sem beneficio comprovado;
- preserve comportamento observavel salvo quando a tarefa pedir uma mudanca funcional.

## Testes

A estrategia planejada e:

- testes unitarios sem infraestrutura para regras de dominio e aplicacao;
- testes de integracao com Testcontainers quando SQL Server, Redis ou outra dependencia real forem parte do risco;
- testes HTTP com `WebApplicationFactory` para pipeline, DI, serializacao, filtros e contratos;
- poucos testes sistemicos com Aspire apenas quando a orquestracao local existir e justificar o custo;
- testes arquiteturais quando a modularizacao moderna existir;
- nao altere testes apenas para faze-los passar.

## Skills do Codex

Antes de uma tarefa especializada, verifique `.agents/skills/` e use somente skills cujo `description` corresponda ao pedido.

As skills complementam este arquivo. Em caso de conflito, este `AGENTS.md` e os arquivos SDD da fase vigente prevalecem.

## Git

- Nunca trabalhe diretamente em `main`.
- Use uma branch por fase ou por entrega relevante.
- Para a Fase 2, use `phase/2-dotnet-10-migration`.
- Faca um commit semantico por prompt quando houver alteracao versionavel.
- Nao faca push sem solicitacao explicita.
- Nao abra Pull Request sem solicitacao explicita.
- Nao mova tags ou branches legadas.
- Revise `git diff` e `git status` antes do commit.
