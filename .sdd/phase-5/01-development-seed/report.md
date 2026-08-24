# Report - Development Seed

Closes #33

## Status

Development e Validation concluidos.

## Issue

- Issue criada porque o prompt trouxe `ISSUE_URL` como placeholder e nenhuma issue existente tinha o titulo esperado.
- Link: https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/33

## Implementacao

- Comando explicito: `dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed`.
- O comando aplica migrations dos dois contextos e encerra sem iniciar listener HTTP.
- O seed e bloqueado em `Production`.
- A senha vem de `DevelopmentSeed:User:Password` ou `DevelopmentSeed__User__Password`.
- Identity usa `UserManager<IdentityUser>`.
- SampleRestaurant usa GUIDs deterministicas e um unico `SaveChangesAsync` como fronteira atomica local.
- Identity e SampleRestaurant continuam sem Unit of Work distribuida.

## Dados Seedados

- Usuario Identity default: `developer@example.local`.
- Claims: `Mesas=ObterPorId`, `Mesas=Adicionar`, `Pratos=ObterPorId`, `Pratos=Adicionar`.
- SampleRestaurant: 4 pratos, 3 mesas, 1 atendente, 1 pedido e 2 itens de pedido.

## Evidencia de Idempotencia

- Primeira execucao em banco descartavel: 5 mudancas Identity e 11 mudancas SampleRestaurant.
- Segunda execucao no mesmo banco: 0 mudancas Identity e 0 mudancas SampleRestaurant.
- Contagens finais: `Users=1`, `Claims=4`, `Pratos=4`, `Mesas=3`, `Atendentes=1`, `Pedidos=1`, `PedidoPrato=2`.

## Validacao

Consulte `validation.md`.

## Criterios de Aceite e Evidencias

| Criterio | Evidencia |
| --- | --- |
| Comando explicito e documentado | `dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed`; documentado em README e docs de desenvolvimento local. |
| Migrations aplicadas antes do seed | `DevelopmentSeedRunner` chama `MigrateAsync` nos dois DbContexts; validacao isolada criou schema e dados em banco descartavel. |
| Usuario de desenvolvimento configuravel | `DevelopmentSeed:User:Email`, `DevelopmentSeed:User:Password` e `DevelopmentSeed:User:UserName`; senha obrigatoria fora do repo. |
| Claims minimas para endpoints protegidos | Claims `Mesas` e `Pratos`; teste de integracao faz login e acessa `GET /api/v1/Mesas/{id}`. |
| Dados representativos do sample | 4 pratos, 3 mesas, 1 atendente, 1 pedido e 2 itens. |
| Idempotencia | Primeira execucao: 5/11 mudancas; segunda: 0/0; contagens finais estaveis. |
| Dados parcialmente existentes | Coberto por `DevelopmentSeedIntegrationTests.SeedQuandoDadosParciaisExistemDeveCompletarSemDuplicar`. |
| Atualizacao segura de dado conhecido | Coberto por `SeedQuandoDadoConhecidoFoiAlteradoDeveRestaurarDefinicao`. |
| Preservacao de dados do usuario | Coberto por `SeedQuandoExistemDadosDoUsuarioDevePreservaLos`. |
| Credencial ausente e placeholder | Coberto por testes unitarios de `DevelopmentSeedConfiguration`. |
| Bloqueio em Production | Teste unitario e validacao isolada com exit code 1. |
| Cancelamento | Coberto por `SeedQuandoCanceladoDeveFalharSemGravarDados`. |
| Sem secrets reais | Diff revisado; somente placeholders e literal de teste `NotASecret_ForTests_2026!`. |

## Comandos de Reproducao

```bash
dotnet user-secrets set "DevelopmentSeed:User:Password" "<local development password>" --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj
docker compose --env-file .env.local up -d sqlserver redis
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed
```

Compose:

```bash
docker compose --env-file .env.local --profile tools up seed
```

## Delivery

- Commit: `feat: add idempotent development seed`.
- Branch: `feat/idempotent-development-seed`.
- Pull Request: https://github.com/rodri-oliveira-dev/web-api-core-seed/pull/34.
- Checks remotos: Build/test, CodeQL, Dependency Review e SonarCloud Quality Gate passaram antes deste registro final.
