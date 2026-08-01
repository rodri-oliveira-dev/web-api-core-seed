# Version Conflicts

## Conflitos resolvidos

| Package | Projeto | Versao inicial | Versao final | Tipo de mudanca | Justificativa |
| --- | --- | --- | --- | --- | --- |
| `coverlet.collector` | `WebApiCoreSeed.UnitTests` | 10.0.1 | 10.0.1 | Nenhuma | Ja estava na versao mais recente reportada. |
| `coverlet.collector` | `WebApiCoreSeed.IntegrationTests` | 6.0.4 | 10.0.1 | Major de tooling de teste | Resolve divergencia antiga e outdated; validado com restore, build e testes. |
| `Microsoft.NET.Test.Sdk` | `WebApiCoreSeed.UnitTests` | 18.8.1 | 18.8.1 | Nenhuma | Ja estava na versao mais recente reportada. |
| `Microsoft.NET.Test.Sdk` | `WebApiCoreSeed.IntegrationTests` | 17.14.1 | 18.8.1 | Major de tooling de teste | Resolve divergencia antiga e outdated; validado com restore, build e testes. |
| `xunit.runner.visualstudio` | `WebApiCoreSeed.UnitTests` | 3.1.5 | 3.1.5 | Nenhuma | Ja estava na versao mais recente reportada. |
| `xunit.runner.visualstudio` | `WebApiCoreSeed.IntegrationTests` | 3.1.4 | 3.1.5 | Patch | Resolve divergencia antiga e outdated. |

## Divergencias mantidas

| Package | Versao | Motivo |
| --- | --- | --- |
| `OpenTelemetry.Instrumentation.EntityFrameworkCore` | 1.17.0-beta.1 | Pacote ja estava beta; `dotnet list --outdated` reportou `Nao encontrado nas fontes`. Mantido sem mudanca funcional. |
| `xunit` | 2.9.3 | `dotnet list --deprecated` reporta `Legacy` com alternativa `xunit.v3`; a migracao para xUnit v3 e major e ficou fora do escopo. |

## Politicas nao usadas

- `VersionOverride`: nao utilizado.
- Versoes flutuantes ou wildcards: nao utilizados.
- `CentralPackageTransitivePinningEnabled`: avaliado e nao habilitado, porque nao havia necessidade comprovada de pinning transitivo.
