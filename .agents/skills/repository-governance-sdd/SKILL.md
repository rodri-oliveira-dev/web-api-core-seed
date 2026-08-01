---
name: repository-governance-sdd
description: Use esta skill para tarefas SDD de governanca do repositorio, criacao ou revisao de skills Codex, AGENTS.md, handoff, decisoes, prompts e documentacao de processo. Nao use para implementar codigo de producao ou testes de aplicacao.
---

# Objetivo

Conduzir tarefas de governanca usando Specification-Driven Development: Specification, Discovery, Design, Development, Validation e Delivery.

Esta skill ajuda o Codex a separar regras globais, fluxos especificos e documentacao auxiliar sem aumentar desnecessariamente o contexto.

# Quando usar

- Criar, revisar ou reorganizar skills em `.agents/skills/`.
- Ajustar `AGENTS.md` para orientacoes globais, roteamento e convencoes compartilhadas.
- Escrever ou revisar prompts SDD, politicas de uso do Codex ou governanca de automacao assistida.
- Avaliar se uma decisao deve ir para `.sdd/phase-2/decisions.md` ou para documentacao operacional.
- Preparar handoff entre chats.

# Quando nao usar

- Implementar comportamento na aplicacao.
- Alterar testes de aplicacao.
- Corrigir pipelines sem trabalho de governanca.
- Criar scripts sem criterio deterministico, seguro e repetivel.

# Processo

1. Especifique a intencao da mudanca.
2. Leia `AGENTS.md`, `.sdd/` e `.agents/` quando existirem.
3. Descubra o estado real do repositorio antes de propor artefatos.
4. Classifique o que pertence a regra global, skill, SDD, script ou documentacao.
5. Decida antes de editar e registre decisoes duraveis.
6. Crie poucos artefatos, com um proposito claro.
7. Evite duplicar no `AGENTS.md` fluxos longos que pertencem a skills ou SDD.
8. Valide estrutura, nomes, links locais, ausencia de segredos e escopo.
9. Revise diff, execute validacoes proporcionais e faca commit semantico quando solicitado.

# Validacao

- Confirme frontmatter das skills com `name` e `description`.
- Confirme nomes em kebab-case.
- Confirme ausencia de referencias a arquivos inexistentes.
- Valide JSON, YAML e scripts quando forem criados ou alterados.
- Execute `git status` e revise o diff antes de finalizar.

# Restricoes

- Nao criar skills genericas demais.
- Nao criar muitas skills para possibilidades raras.
- Nao alterar codigo de producao nem testes de aplicacao.
- Nao fazer push nem abrir Pull Request sem pedido explicito.
