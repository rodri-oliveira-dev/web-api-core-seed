---
name: integration-tests-dotnet
description: Use esta skill para criar ou revisar testes de integracao .NET neste repositorio, incluindo WebApplicationFactory, fixtures, Testcontainers, isolamento e testes HTTP. Nao use para testes unitarios simples ou antes da base de integracao existir.
---

# Objetivo

Orientar testes de integracao da versao moderna do `web-api-core-seed` com foco em fidelidade, isolamento e custo de execucao.

No estado atual, o repositorio possui apenas testes unitarios em `test/Pedidos.Test`. Recursos como `WebApplicationFactory` e Testcontainers sao planejados para fases futuras e devem ser tratados como condicionais ate serem implementados.

# Quando usar

- Criar ou revisar testes HTTP com `WebApplicationFactory`.
- Avaliar uso de Testcontainers para SQL Server, Redis ou outra dependencia real.
- Testar pipeline HTTP, DI, serializacao, filtros, autenticacao, migrations, constraints ou transacoes.
- Ajustar fixtures, seeds, limpeza de dados ou ciclo de vida de dependencias de teste.

# Quando nao usar

- Testes unitarios puros de validators, services ou modelos.
- Validar build sem mudar estrategia de integracao.
- Criar dependencias externas caras sem decisao explicita.
- Executar testes sistemicos amplos sem infraestrutura versionada.

# Processo

1. Confirme se a base de integracao ja existe no repositorio.
2. Identifique se o risco exige HTTP real, DI completo, provider de banco real ou isolamento simples.
3. Use Testcontainers somente quando a dependencia real aumentar claramente a confianca.
4. Evite portas fixas inventadas e dependencias externas nao controladas.
5. Mantenha seeds e limpeza previsiveis.
6. Documente impacto quando a estrategia oficial de testes mudar.

# Validacao

Quando a infraestrutura existir, execute o projeto de teste afetado. Enquanto ela nao existir, registre a limitacao e nao crie comando ficticio.

# Restricoes

- Nao alterar testes apenas para ocultar falha real.
- Nao tornar Testcontainers requisito amplo sem decisao registrada.
- Nao introduzir sleeps arbitrarios.
- Nao versionar segredos ou configuracao local sensivel.
