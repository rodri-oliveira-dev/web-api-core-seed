# Requirements - 01 Unit Test Baseline

## Objetivo

Auditar e corrigir a suite de testes unitarios existente para que cada teste execute o comportamento descrito, tenha asserts relevantes e falhe quando o comportamento protegido for quebrado.

## Escopo

- Inventariar todos os testes do projeto `Pedidos.Test`.
- Corrigir falsos positivos e nomes inconsistentes.
- Fortalecer assertions e verificacoes de efeitos colaterais.
- Remover dependencia desnecessaria de relogio real nos testes unitarios.
- Adicionar testes unitarios ausentes para fluxos relevantes de service e validators.
- Gerar baseline de cobertura com a infraestrutura disponivel.
- Documentar limitacoes da cobertura.

## Fora de escopo

- Testcontainers.
- Novos testes de integracao HTTP.
- Alteracoes de seguranca.
- OpenTelemetry.
- Workflows completos de CI.
- Arquitetura Hexagonal ou modularizacao.
- Aspire.
- Refatoracoes amplas de producao.

## Criterios de aceite

- Cada teste executa o comportamento descrito pelo nome.
- Nenhum teste passa apenas porque um mock nao foi chamado.
- Assertions relevantes verificam resultado ou efeito observavel.
- A suite executa em .NET 10.
- A cobertura e gerada e registrada sem threshold arbitrario.
- Mudancas produtivas, se existirem, sao pequenas, testadas e documentadas.
