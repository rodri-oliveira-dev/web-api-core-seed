# Validation - Development Seed

## Plano

- `dotnet restore WebApiCoreSeed.slnx --locked-mode`
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`
- `dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build`
- `dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build`
- Seed isolado em banco descartavel.
- Segunda execucao do seed no mesmo banco descartavel.
- Login do usuario de desenvolvimento.
- Chamada autenticada a endpoint protegido.
- Bloqueio em `Production`.
- Geracao/validacao de OpenAPI.
- `dotnet list WebApiCoreSeed.slnx package --vulnerable`
- `dotnet list WebApiCoreSeed.slnx package --deprecated`
- `git diff --check`
- Revisao manual para ausencia de secrets.

## Resultados

| Validacao | Resultado |
| --- | --- |
| `dotnet restore WebApiCoreSeed.slnx --locked-mode` | Passou apos atualizar lock files de `WebApiCoreSeed.UnitTests` e `OpenApiGenerator`, que estavam inconsistentes com versoes centrais ja definidas. |
| `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore` | Passou com 0 warnings e 0 erros na execucao final isolada. |
| Unit tests | Passou: 113 testes. |
| Integration tests | Passou: 53 testes com SQL Server e Redis por Testcontainers. |
| Seed isolado em SQL Server descartavel | Passou. Primeira execucao: 5 mudancas Identity e 11 mudancas SampleRestaurant. |
| Segunda execucao no mesmo banco | Passou. Segunda execucao: 0 mudancas Identity e 0 mudancas SampleRestaurant. |
| Contagens apos segunda execucao | `Users=1`, `Claims=4`, `Pratos=4`, `Mesas=3`, `Atendentes=1`, `Pedidos=1`, `PedidoPrato=2`. |
| Login e endpoint protegido | Passou em teste de integracao: login em `/api/v1/entrar` e chamada autenticada a `/api/v1/Mesas/{id}`. |
| Bloqueio em `Production` | Passou. Seed isolado retornou exit code 1 antes de gravar dados. |
| OpenAPI | Gerado por `tools/OpenApiGenerator`; `docs/openapi/openapi-v1.json` e `docs/openapi/openapi-v2.json` ficaram sem diff. |
| `dotnet list WebApiCoreSeed.slnx package --vulnerable` | Passou; nenhum pacote vulneravel nas fontes atuais. |
| `dotnet list WebApiCoreSeed.slnx package --deprecated` | Passou com achado preexistente: `xunit 2.9.3` deprecated nos projetos de teste. |
| `git diff --check` | Passou; Git emitiu apenas aviso de line ending nos dois lock files atualizados. |
| Busca de secrets no diff | Sem segredo real encontrado. Apenas placeholders documentados e literal de teste `NotASecret_ForTests_2026!`. |
