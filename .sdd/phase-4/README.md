# SDD Phase 4 - Modernizacao arquitetural

## Objetivo

Modernizar a estrutura interna do `web-api-core-seed` para um monolito modular pragmatico com arquitetura Hexagonal por modulo, preservando o comportamento observavel da API salvo quando uma issue pedir mudanca de contrato.

## Branch

```text
phase/4-architecture-modernization
```

## Issues relacionadas

- `#14` - Phase 4 architecture modernization
- `#15` - Repository and module governance for Phase 4
- `#16` - Prepare architecture documentation and dependency rules
- `#17` - Separate application and infrastructure concerns
- `#18` - Explicit persistence boundaries and unit of work
- `#19` - Cancellation, migrations and infrastructure ownership
- `#20` - Deterministic pagination and contract safety

## Sequencia dos prompts

```text
01 - Arquitetura modular Hexagonal
02 - Separacao do dominio de exemplo
03 - Portas de persistencia
04 - Unit of Work
05 - CancellationToken
06 - Migrations na infraestrutura
07 - Paginacao deterministica
```

## Regra SDD

Cada prompt deve seguir:

1. Specification
2. Discovery
3. Design
4. Development
5. Validation
6. Delivery

Nenhuma alteracao de codigo deve ocorrer antes de Specification, Discovery e Design do prompt vigente.

## Regras de entrega

- Usar a branch compartilhada `phase/4-architecture-modernization`.
- Criar um unico commit semantico por prompt quando houver alteracao versionavel.
- Nao fazer push automatico.
- Nao abrir Pull Request automaticamente.
- Nao fechar issues automaticamente.
- Registrar decisoes, validacoes, limitacoes e proximos passos nos arquivos versionados da fase.

## Handoff

Arquivos usados para transferencia entre chats:

- `.sdd/phase-4/status.md`
- `.sdd/phase-4/decisions.md`
- `.sdd/phase-4/handoff.md`
- `.sdd/phase-4/architecture-map.md`
- `.sdd/phase-4/module-catalog.md`
- `.sdd/phase-4/dependency-rules.md`
- relatorios nas pastas especificas de cada prompt

## Regra de leitura obrigatoria

Antes de alterar codigo, testes ou governanca, cada prompt deve ler este arquivo, `status.md`, `decisions.md`, `handoff.md`, o relatorio do prompt anterior quando existir, e a pasta especifica do prompt.
