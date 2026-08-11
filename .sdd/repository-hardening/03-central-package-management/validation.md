# Validation

## Comandos

| Comando | Resultado |
| --- | --- |
| `dotnet nuget locals all --list` | Sucesso. Caches listados em `http-cache`, `global-packages`, `temp` e `plugins-cache`. |
| `dotnet restore WebApiCoreSeed.sln --force-evaluate` | Sucesso. Todos os projetos restaurados; nenhum `NU1008`. |
| `dotnet build WebApiCoreSeed.sln --configuration Release --no-restore` | Sucesso. 31 warnings de analisadores ja existentes; 0 erros. |
| `dotnet test WebApiCoreSeed.sln --configuration Release --no-build` | Sucesso. 53 unitarios e 42 integracao aprovados. |
| `dotnet list WebApiCoreSeed.sln package --include-transitive` | Sucesso. Conflitos de teste aparecem resolvidos no estado final. |
| `git grep -n '<PackageReference.*Version=' -- '*.csproj'` | Nenhuma ocorrencia. |
| `git grep -n '<PackageVersion' -- '*Directory.Packages.props'` | Sucesso. Versoes aparecem somente nos arquivos CPM. |

## Saude de pacotes apos CPM

| Comando | Resultado |
| --- | --- |
| `dotnet list WebApiCoreSeed.sln package --outdated` | Sucesso. Sem outdated em testes apos alinhamento; `OpenTelemetry.Instrumentation.EntityFrameworkCore` `1.17.0-beta.1` segue como `Nao encontrado nas fontes`. |
| `dotnet list WebApiCoreSeed.sln package --deprecated` | Sucesso. `xunit` `2.9.3` reportado como `Legacy` em unitarios e integracao. |
| `dotnet list WebApiCoreSeed.sln package --vulnerable` | Sucesso. Nenhum pacote vulneravel reportado. |

## MSBuild preprocessado

Foram gerados preprocessados temporarios em `obj` e removidos apos a verificacao:

- `src/WebApiCoreSeed.Api/obj/api.pp.xml`;
- `tests/WebApiCoreSeed.IntegrationTests/obj/integration.pp.xml`;
- `tools/OpenApiGenerator/obj/tool.pp.xml`.

Confirmacoes:

- projeto em `src` importa `src/Directory.Packages.props` e depois `Directory.Packages.props` raiz;
- projeto em `tests` importa `tests/Directory.Packages.props` e depois `Directory.Packages.props` raiz;
- projeto em `tools` importa `tools/Directory.Packages.props` e depois `Directory.Packages.props` raiz;
- `ManagePackageVersionsCentrally` avalia como `true`.

## Observacoes

- Os XML preprocessados nao foram versionados.
- `packages.lock.json` foi versionado para preservar restore reproduzivel.
- `RestoreLockedMode` nao foi habilitado neste prompt.
