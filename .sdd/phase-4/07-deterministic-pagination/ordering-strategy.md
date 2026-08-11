# Ordering Strategy - Prompt 07

| Endpoint | Campo principal | Campo de desempate | Direcao | Estabilidade | Indice relacionado | Comportamento com dados novos |
| --- | --- | --- | --- | --- | --- | --- |
| `GET /api/v{version}/Pratos` | `Titulo` | `Id` | Ascendente | Estavel para linhas existentes; `Id` torna a ordenacao unica quando titulos se repetem | `IX_Pratos_Titulo_Id` | Novos pratos sao posicionados conforme `Titulo` e `Id`; offset pode deslocar itens entre consultas, comportamento aceito para volume moderado |

## Observacao

Offset pagination nao garante snapshot entre paginas quando ha insercoes ou remocoes concorrentes. Essa limitacao foi aceita para o endpoint atual porque o catalogo e moderado e a simplicidade e prioritaria.
