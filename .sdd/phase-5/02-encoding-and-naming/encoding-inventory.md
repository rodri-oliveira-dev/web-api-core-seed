# Encoding Inventory

| Occurrence | Location | Classification | Decision | Correction classification |
| --- | --- | --- | --- | --- |
| `UsuÃ¡rio temporariamente bloqueado por tentativas invÃ¡lidas` | `src/WebApiCoreSeed.Api/Controllers/AuthControllerBase.cs` | Contrato HTTP publico / texto retornado pela API | Correct to `Usuário temporariamente bloqueado por tentativas inválidas`. | Correção de texto público |
| `UsuÃ¡rio ou Senha incorretos` | `src/WebApiCoreSeed.Api/Controllers/AuthControllerBase.cs` | Contrato HTTP publico / texto retornado pela API | Correct to `Usuário ou Senha incorretos`. | Correção de texto público |
| `Requisicao invalida.` | `src/WebApiCoreSeed.Api`, `docs/openapi/openapi-v1.json` | Contrato HTTP público / contrato OpenAPI | Correct to `Requisição inválida.` in active API/OpenAPI text. | Correção de texto público |
| `Autenticacao necessaria.` | `src/WebApiCoreSeed.Api`, `docs/openapi/openapi-v1.json` | Contrato HTTP público / contrato OpenAPI | Correct to `Autenticação necessária.` in active API/OpenAPI text. | Correção de texto público |
| Unaccented public Problem Details text such as `Validacao`, `requisicao`, `operacao`, `nao`, `permissao`, `acao` | `src/WebApiCoreSeed.Api` | Contrato HTTP público | Correct active user-facing Portuguese text while preserving status codes and JSON shape. | Correção de texto público |
| `src/README.md` mojibake observed through direct file read | `src/README.md` | Documento ativo | Replace with coherent current UTF-8 documentation. | Interna e não quebradora |
| Mojibake in `.sdd/phase-4/*` | `.sdd/phase-4/status.md`, `.sdd/phase-4/handoff.md` | Documento histórico | Preserve; it records previous phase state and is not active code. | Identificador/texto histórico preservado |

## Notes

- Active OpenAPI did not initially contain `Ã`, `Â`, or `�`, but it did contain unaccented public Portuguese descriptions generated from active code.
- Historical docs may retain legacy wording when needed for compatibility context.
