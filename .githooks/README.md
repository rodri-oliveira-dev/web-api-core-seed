# Git Hooks Locais

Este repositorio usa hooks locais opcionais configurados por `core.hooksPath=.githooks`.

## Instalacao

```bash
scripts/setup/configure-git-hooks.sh
```

No PowerShell:

```powershell
scripts/setup/configure-git-hooks.ps1
```

## Verificacao

```bash
scripts/setup/configure-git-hooks.sh --check
git config --local --get core.hooksPath
```

No PowerShell:

```powershell
scripts/setup/configure-git-hooks.ps1 -Check
git config --local --get core.hooksPath
```

## Remocao

```bash
git config --local --unset core.hooksPath
```

## Validacoes

O `pre-push` identifica alteracoes documentais e ignora validacoes .NET quando nao ha impacto tecnico. Quando arquivos .NET ou da solution forem impactados, executa no maximo:

```bash
dotnet restore WebApiCoreSeed.sln
dotnet build WebApiCoreSeed.sln --no-restore
dotnet test WebApiCoreSeed.sln --no-build
```

O hook nao executa containers, cobertura completa nem ferramentas externas pesadas. Gates completos permanecem no CI.

## Excecao

Use `git push --no-verify` somente em situacao excepcional, registrada no contexto da entrega ou Pull Request, e reexecute validacoes assim que possivel.
