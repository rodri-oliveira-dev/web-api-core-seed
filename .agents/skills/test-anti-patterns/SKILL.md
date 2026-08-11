---
name: test-anti-patterns
description: Use esta skill para auditar qualidade de testes .NET neste repositorio, encontrando anti-padroes como asserts fracos, ausencia de asserts, flakiness, over-mocking, acoplamento a implementacao, dependencia de ordem, sleeps, dados magicos e cobertura artificial. Nao use para escrever testes novos do zero ou migrar framework.
license: MIT
---

# Objetivo

Orientar uma revisao pragmatica de testes automatizados para aumentar confianca, diagnostico e manutencao.

Esta skill nao busca apenas mais testes. Ela busca testes que verifiquem comportamento relevante e falhem pelos motivos certos.

# Quando usar

- O usuario pedir auditoria, revisao ou melhoria de qualidade dos testes.
- Testes estiverem passando, mas dando pouca confianca.
- Houver suspeita de asserts fracos, excesso de mocks ou acoplamento a detalhes internos.
- Um prompt alterar testes de forma ampla.

# Quando nao usar

- Escrever testes novos do zero sem foco em auditoria.
- Rodar testes apenas para validar build.
- Medir cobertura pura sem avaliar qualidade.
- Migrar framework de testes.

# Anti-padroes criticos

- Sem assert significativo.
- Assert tautologico.
- Teste que apenas toca linhas sem validar comportamento.
- Assert fraco demais para o risco protegido.
- Excecao engolida.
- Excesso de mocks.
- Acoplamento a implementacao.
- Flakiness por tempo, ordem ou ambiente.
- Dados magicos sem intencao clara.

# Processo

1. Identifique o projeto de teste e o comportamento protegido.
2. Leia o codigo de producao relacionado quando necessario.
3. Classifique problemas por severidade.
4. Separe problema de teste ruim de problema de design no codigo produtivo.
5. Sugira o menor ajuste seguro.
6. Quando houver relacao com teste de integracao futuro, consulte `integration-tests-dotnet`.
7. Valide com o projeto de teste afetado quando houver mudanca.

# Criterio de qualidade

Um teste bom deixa claro qual comportamento protege, prepara dados intencionais, executa uma acao observavel e verifica resultado ou efeito com asserts relevantes.
