# Requirements - Prompt 01

## Issue

`#17 - [Phase 4] Separate application and infrastructure concerns`

Interpretacao desta entrega:

```text
Adopt a pragmatic modular Hexagonal Architecture
```

## Objetivos

- Identificar modulos de negocio reais.
- Criar uma estrutura proporcional de modulo, dominio, aplicacao e infraestrutura.
- Manter a API como adaptador de entrada e composition root.
- Fazer controllers dependerem de casos de uso/portas de entrada para o dominio, nao de repositorios.
- Registrar regras de dependencia e adicionar testes arquiteturais.
- Preservar rotas, payloads, status codes, autenticacao, autorizacao, Problem Details, rate limiting, health checks e OpenAPI.

## Fora de escopo

- Separacao definitiva do dominio de exemplo.
- Remocao do repositorio generico.
- Redesenho de Unit of Work.
- Propagacao completa de `CancellationToken`.
- Movimentacao final de migrations.
- Reforma da paginacao.
- Aspire, AppHost, empacotamento `dotnet new`, publicacao NuGet, deploy, cloud, microsservicos, mensageria e Sonar.
