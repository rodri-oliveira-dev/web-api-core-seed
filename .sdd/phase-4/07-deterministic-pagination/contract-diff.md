# Contract Diff - Prompt 07

| Endpoint | Mudanca | Classificacao |
| --- | --- | --- |
| `GET /api/v{version}/Pratos` | Response paginado troca `data` por `items`. | Breaking change / mudanca de formato |
| `GET /api/v{version}/Pratos` | Response paginado troca `pageNumber` por `page`. | Breaking change / mudanca de formato |
| `GET /api/v{version}/Pratos` | Response paginado troca `totalItens` por `totalItems`. | Breaking change / mudanca de formato |
| `GET /api/v{version}/Pratos` | Response passa a incluir `pageSize`, `hasNextPage` e `hasPreviousPage`. | Compatível para consumidores tolerantes a campos novos |
| `GET /api/v{version}/Pratos` | `PageSize > 50` passa de truncamento silencioso para `400` Validation Problem Details. | Breaking change / mudanca de limite |
| `GET /api/v{version}/Pratos` | `PageNumber <= 0` e `PageSize <= 0` passam a retornar `400` Validation Problem Details. | Breaking change / validacao nova |
| `GET /api/v{version}/Pratos` | Ordenacao fixa por `Titulo`, `Id`. | Mudanca de comportamento deterministica |

## Sem mudanca

- Query params continuam `PageNumber` e `PageSize`.
- Default continua `PageNumber=1` e `PageSize=10`.
- Maximo documentado continua `PageSize=50`.
- Endpoint continua publico e rate limited.
