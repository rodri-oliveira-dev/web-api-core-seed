# Codex Skills

Skills importadas e adaptadas a partir de `rodri-oliveira-dev/poc-arquitetura` no SHA `9029163f1a795a1bb18f138dd8fa9179f13f544e`.

Use uma skill apenas quando a descricao corresponder ao pedido atual. Em caso de conflito, prevalecem `AGENTS.md` e os arquivos SDD da fase em execucao.

| Skill | Status | Quando usar | Quando nao usar | Dependencias |
| --- | --- | --- | --- | --- |
| `repository-governance-sdd` | pronta | Governanca, SDD, `AGENTS.md`, skills, handoff, decisoes e documentacao de processo. | Codigo de aplicacao ou testes de produto. | `.sdd/`, `AGENTS.md` |
| `dotnet-service-change` | parcialmente aplicavel | Mudancas na solution .NET atual ou na migracao tecnica, respeitando o estado real. | Governanca pura ou mudancas sem impacto tecnico. | `WebApiCoreSeed.sln`, projetos reais |
| `dotnet-refactoring-engineer` | parcialmente aplicavel | Refatoracao, revisao e melhoria de codigo .NET/C# com comportamento preservado. | Reescrita ampla ou mudanca de framework sem prompt especifico. | Projetos e testes reais |
| `integration-tests-dotnet` | planejada | Criar ou revisar testes de integracao quando a Fase 2 introduzir base moderna para isso. | Testes unitarios simples ou antes da infraestrutura de teste existir. | Futuro `WebApplicationFactory` e Testcontainers |
| `test-anti-patterns` | pronta | Auditar qualidade dos testes atuais ou futuros. | Medir cobertura pura ou escrever testes do zero sem foco em anti-padroes. | Projeto `test/WebApiCoreSeed.Tests` |

## Notas

- Skills adiadas e excluidas estao registradas em `.sdd/phase-2/bootstrap-tooling/source-manifest.md`.
- O aviso de terceiros necessario para `test-anti-patterns` esta em `THIRD-PARTY-NOTICES.md`.
- Skills marcadas como parcialmente aplicaveis devem tratar recursos modernos como condicionais ate que sejam implementados.
