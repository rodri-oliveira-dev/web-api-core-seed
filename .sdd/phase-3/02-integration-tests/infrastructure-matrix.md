# Infrastructure Matrix - 02 Integration Tests

| Dependencia | Imagem | Tag | Porta interna | Readiness | Connection string | Uso nos testes | Atualizacao |
| --- | --- | --- | --- | --- | --- | --- | --- |
| SQL Server | `mcr.microsoft.com/mssql/server` | `2022-CU14-ubuntu-22.04` | `1433` | Readiness do modulo Testcontainers + tentativa real de `SqlConnection.OpenAsync()` por ate 2 minutos | Gerada por Testcontainers e sobrescrita em `ConnectionStrings__DefaultConnection` | `ApplicationDbContext`, `MeuDbContext`, migrations, health, constraints e transacoes | Atualizar tag apenas por decisao registrada, com execucao completa da suite |
| Redis | `redis` | `7.4.2-alpine` | `6379` | Readiness do modulo Testcontainers | Gerada por Testcontainers e sobrescrita em `RedisCacheSettings__ConnectionString` | Cache da API, health e testes diretos de chave/expiracao | Atualizar tag apenas por decisao registrada, com execucao completa da suite |

## Observacoes

- Nao ha nomes fixos de containers.
- Nao ha portas fixas publicadas no host.
- Reutilizacao de containers nao foi habilitada nesta entrega.
- O cliente Redis usado para reset usa `allowAdmin=true`; a aplicacao recebe connection string sem admin mode.
