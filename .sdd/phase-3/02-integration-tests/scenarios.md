# Scenarios - 02 Integration Tests

## API

- Endpoint publico valido retorna `200`.
- Payload invalido retorna `ValidationProblemDetails`.
- Recurso inexistente retorna Problem Details `404`.
- Endpoint protegido sem token retorna Problem Details `401`.
- Token sem permissao retorna Problem Details `403`.
- Regra de dominio invalida retorna Problem Details `400`.
- Rate limit publico retorna Problem Details `429`.
- `/hc` retorna healthy para SQL Server e Redis.
- OpenAPI responde para `v1` e `v2`.

## SQL Server

- Migrations de Identity e dominio aparecem como aplicadas em banco vazio.
- `Prato` persiste e pode ser consultado.
- FK de `Pedido` impede referencias inexistentes.
- Indice unico de `AspNetUsers.NormalizedUserName` e aplicado.
- Transacao pode ser revertida.
- Funcao nativa `DATEDIFF`/`SYSUTCDATETIME` confirma execucao em SQL Server real.

## Redis

- Escrita e leitura de chave.
- Chave inexistente retorna nulo.
- Expiracao remove a chave dentro do timeout esperado.
