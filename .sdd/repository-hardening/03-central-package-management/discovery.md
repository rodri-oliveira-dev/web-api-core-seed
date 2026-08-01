# Discovery

## Comandos executados

```bash
dotnet list WebApiCoreSeed.sln package
dotnet list WebApiCoreSeed.sln package --outdated
dotnet list WebApiCoreSeed.sln package --deprecated
dotnet list WebApiCoreSeed.sln package --vulnerable
```

A CLI atual aceitou a sintaxe `dotnet list ... package`; nao foi necessario usar `dotnet package list`.

## Estado inicial

- Todos os projetos ativos miravam `net10.0`.
- Nao havia `Directory.Packages.props`.
- Nao havia `packages.lock.json`.
- Todos os `PackageReference` diretos continham `Version`.
- Pacotes Microsoft de plataforma estavam majoritariamente alinhados em `10.0.10`.
- Havia divergencias nos pacotes de teste:
  - `Microsoft.NET.Test.Sdk`: `18.8.1` em unitarios e `17.14.1` em integracao.
  - `coverlet.collector`: `10.0.1` em unitarios e `6.0.4` em integracao.
  - `xunit.runner.visualstudio`: `3.1.5` em unitarios e `3.1.4` em integracao.

## Outdated inicial

- `WebApiCoreSeed.IntegrationTests`:
  - `coverlet.collector` `6.0.4` -> `10.0.1`.
  - `Microsoft.NET.Test.Sdk` `17.14.1` -> `18.8.1`.
  - `xunit.runner.visualstudio` `3.1.4` -> `3.1.5`.
- `WebApiCoreSeed.Api`:
  - `OpenTelemetry.Instrumentation.EntityFrameworkCore` `1.17.0-beta.1` foi reportado como `Nao encontrado nas fontes`.

## Deprecated inicial

- `xunit` `2.9.3` foi reportado como `Legacy` em:
  - `WebApiCoreSeed.UnitTests`;
  - `WebApiCoreSeed.IntegrationTests`.
- Alternativa indicada pela fonte NuGet: `xunit.v3`.

## Vulnerable inicial

Nenhum pacote vulneravel foi reportado para os projetos ativos, de acordo com `https://api.nuget.org/v3/index.json`.

## Observacoes

- `Microsoft.AspNetCore.Mvc.Testing` e `Microsoft.EntityFrameworkCore.InMemory` sao usados por `tests/` e `tools/`, entao pertencem ao arquivo raiz.
- `StackExchange.Redis` e referencia direta apenas em testes de integracao; o uso produtivo e indireto via pacotes de cache/health checks.
- `OpenTelemetry.Instrumentation.EntityFrameworkCore` permanece beta e foi mantido sem upgrade silencioso.
