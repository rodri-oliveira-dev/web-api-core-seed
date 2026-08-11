# SDD Phase 3 - Qualidade, seguranca e observabilidade

## Objetivo

Fase 3 estabelece uma baseline confiavel de qualidade, seguranca e observabilidade para a solution modernizada em .NET 10, sem antecipar modularizacao, arquitetura Hexagonal, Aspire ou testes com infraestrutura real.

## Branch

```text
phase/3-quality-and-safety
```

## Issues relacionadas

- `#9` - Phase 3 quality and safety
- `#10` - Preparacao e governanca da Fase 3
- `#11` - Review and fix the existing unit tests
- `#12` - Integration tests baseline
- `#13` - Security, observability and CI gates

## Sequencia dos prompts

```text
01 - Unit test baseline
02 - Integration test baseline
03 - Security review
04 - OpenTelemetry baseline
05 - CI and quality gates
```

## Regra de leitura obrigatoria

Cada prompt da Fase 3 deve ler este arquivo, `status.md`, `decisions.md`, `handoff.md` e a pasta especifica do prompt antes de alterar codigo, testes ou governanca.

## Regras de entrega

- Usar Specification-Driven Development: Specification, Discovery, Design, Development, Validation e Delivery.
- Criar um unico commit semantico por prompt quando houver alteracao versionavel.
- Nao fazer push automatico.
- Registrar decisoes, validacoes, limitacoes e proximos passos em arquivos versionados.

## Handoff

Arquivos usados para transferencia entre chats:

- `.sdd/phase-3/status.md`
- `.sdd/phase-3/decisions.md`
- `.sdd/phase-3/handoff.md`
- relatorios nas pastas especificas de cada prompt
