---
name: dotnet-service-change
description: Use esta skill ao alterar a aplicacao .NET deste repositorio, incluindo API, Business, Data, EF Core, autenticacao, configuracao ou testes relacionados. Nao use para mudancas apenas em governanca, CI/CD puro ou documentacao sem impacto tecnico.
---

# Objetivo

Orientar alteracoes pequenas e seguras na solution `RestauranteAPI.sln`, respeitando o estado atual em .NET Core 3.1 e a modernizacao incremental planejada para .NET 10.

# Quando usar

- Alteracoes em Controllers, filtros, middlewares, Swagger, autenticacao ou configuracao da API.
- Alteracoes em services, validadores, modelos de dominio legado ou interfaces.
- Alteracoes em EF Core, DbContext, mappings, migrations ou repositories.
- Ajustes de comportamento que exijam testes unitarios ou, no futuro, testes de integracao.
- Migracao tecnica da solution quando o prompt pedir explicitamente.

# Quando nao usar

- Mudancas apenas em `.agents/`, `AGENTS.md`, prompts ou SDD.
- Revisoes puramente de automacao ou hooks.
- Documentacao geral sem impacto em contrato, arquitetura, setup local ou comportamento.

# Processo

1. Leia `AGENTS.md`, `.sdd/phase-2/` e a documentacao relevante.
2. Localize a solution e os projetos reais antes de editar.
3. Confirme o framework alvo atual e nao altere target framework sem prompt especifico.
4. Verifique impacto em contrato HTTP, autenticacao, EF Core, Redis, logs e documentacao.
5. Aplique a menor mudanca coerente com os padroes existentes.
6. Preserve comportamento observavel salvo quando a tarefa pedir mudanca funcional.
7. Se a tarefa envolver testes de integracao, consulte `integration-tests-dotnet`.
8. Revise o diff e confirme que nao houve refactor ou formatacao fora do escopo.
9. Valide com restore, build e testes proporcionais quando o ambiente permitir.

# Validacao

Comandos baseline, a partir da raiz:

```bash
dotnet restore RestauranteAPI.sln
dotnet build RestauranteAPI.sln --no-restore
dotnet test test/Pedidos.Test/Pedidos.Test.csproj --no-build
```

Se a validacao for bloqueada por ambiente legado, registre o bloqueio sem alterar SDK, target framework ou pacotes.

# Restricoes

- Nao altere migrations antigas sem necessidade explicita.
- Nao altere testes apenas para faze-los passar.
- Nao introduza segredos, URLs ou portas inventadas.
- Nao crie abstracoes sem beneficio comprovado.
- Nao faca push nem abra Pull Request sem pedido explicito.
