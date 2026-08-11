# Requirements - 02 Integration Tests

## Objetivo

Criar uma baseline de testes de integracao para a issue `#12`, usando `WebApplicationFactory<Program>` para inicializar a API e Testcontainers para provisionar dependencias reais descartaveis.

## Escopo

- Projeto dedicado `WebApiCoreSeed.IntegrationTests`.
- API executada no ambiente `Testing`.
- SQL Server real para `ApplicationDbContext` e `MeuDbContext`.
- Redis real para cache, testes diretos e health check.
- Migrations aplicadas automaticamente antes dos testes.
- Testes HTTP para contratos criticos da API.
- Testes especificos de persistencia e Redis quando o risco esta na infraestrutura.

## Fora de escopo

- Aspire.
- Docker Compose.
- EF Core InMemory ou SQLite como substitutos.
- OpenTelemetry completo.
- CI/gates.
- Reescrita da suite unitaria existente.
