# Contract Design - Prompt 07

## Entrada

Query parameters:

| Parametro | Tipo | Default | Minimo | Maximo | Comportamento invalido |
| --- | --- | --- | --- | --- | --- |
| `PageNumber` | `int` | `1` | `1` | N/A | `400` Validation Problem Details |
| `PageSize` | `int` | `10` | `1` | `50` | `400` Validation Problem Details |

O indice inicial da pagina e `1`.

## Saida

Formato adotado:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 10,
  "totalItems": 0,
  "totalPages": 0,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

## Comportamentos

- Pagina dentro do intervalo retorna ate `pageSize` itens.
- Pagina apos o final retorna `items: []`, mantendo `page` solicitado e totais reais.
- Colecao vazia retorna `totalItems: 0`, `totalPages: 0`, `items: []`.
- `hasPreviousPage` e verdadeiro quando `page > 1` e existem itens totais.
- `hasNextPage` e verdadeiro quando `page < totalPages`.
- Nao ha filtros na listagem atual.
- Ordenacao e fixa: `Titulo` ascendente, `Id` ascendente.

## Erros

Entradas invalidas usam Validation Problem Details:

- `type`: `urn:problem:validation`
- `status`: `400`
- `errors`: chaves de campo geradas pelo ModelState
- `traceId`: presente
