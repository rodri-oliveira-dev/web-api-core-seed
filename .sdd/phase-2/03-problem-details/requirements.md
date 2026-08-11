# Requirements - 03 Problem Details

## Objetivo

Padronizar respostas de erro HTTP da API ativa com ASP.NET Core Problem Details, usando o pipeline nativo de .NET 10 e removendo mecanismos legados concorrentes.

## Escopo

- Registrar `AddProblemDetails`.
- Registrar handlers com `IExceptionHandler`.
- Centralizar excecoes inesperadas em `UseExceptionHandler()`.
- Mapear erros conhecidos para status, `type`, `title`, `detail`, `instance`, `traceId` e, quando aplicavel, `errors`.
- Migrar respostas de erro de controllers e filtros para `application/problem+json`.
- Manter respostas de sucesso, rotas e autenticacao sem mudanca funcional.
- Manter Domain Notification quando usado como mecanismo de regra de dominio.

## Fora de Escopo

- Nao implementar rate limiting nativo.
- Nao modernizar Swagger/OpenAPI ou API Versioning.
- Nao trocar a estrategia de persistencia.
- Nao remover `CustomResult` das respostas de sucesso.

## Criterios de Aceite

- Erros retornam `application/problem+json`.
- Erros conhecidos possuem status deterministico.
- Erros inesperados sao registrados.
- Stack trace e detalhes sensiveis nao sao expostos fora de Development.
- `traceId` esta presente no payload.
- Nao existem pipelines concorrentes de excecao.
- Testes HTTP cobrem validacao, 404, regra de dominio, excecao inesperada, 401, ausencia de stack trace, `traceId` e content type.
