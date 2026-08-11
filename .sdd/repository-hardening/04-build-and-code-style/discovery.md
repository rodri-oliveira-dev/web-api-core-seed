# Discovery

## Contexto lido

- `.sdd/repository-hardening/README.md`
- `.sdd/repository-hardening/status.md`
- `.sdd/repository-hardening/decisions.md`
- `.sdd/repository-hardening/handoff.md`
- `.editorconfig`
- `Directory.Build.props`
- projetos `.csproj` ativos

## Estado inicial observado

- `.editorconfig` tinha apenas secoes C# minimas e nao declarava `root = true`.
- `Directory.Build.props` declarava `Nullable=disable`, `ImplicitUsings=enable` e `AnalysisLevel=latest-recommended`.
- `TargetFramework=net10.0` estava repetido em todos os projetos ativos.
- `GenerateDocumentationFile=true` estava repetido nos projetos de `src`, com duplicacao adicional no projeto da API.
- A API tambem suprimia `CS1591` via `NoWarn`, enquanto `.editorconfig` ja tratava `CS1591`.
- Projetos de testes e ferramenta tinham `Nullable`/`ImplicitUsings` locais em parte dos projetos.

## Build baseline

Comando:

```bash
dotnet build WebApiCoreSeed.sln --configuration Release --no-incremental
```

Resultado:

- Sucesso.
- 31 warnings, todos `CA*`.
- 0 warnings `CS*`.

## Simulacao nullable global

Comando:

```bash
dotnet build WebApiCoreSeed.sln --configuration Release --no-incremental -p:Nullable=enable
```

Resultado deduplicado:

- 31 warnings `CA*` ja existentes.
- 101 warnings nullable `CS*` adicionais.
- Conclusao: habilitar nullable global sem correcoes criaria explosao artificial de warnings.
