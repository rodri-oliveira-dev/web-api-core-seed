# Requirements

## Objetivo

Padronizar convencoes globais de estilo e build para a solution .NET 10 sem gerar churn amplo nem esconder warnings por supressoes largas.

## Escopo

- Modernizar `.editorconfig` com `root = true`, charset, EOL, whitespace, secoes por extensao e regras C# explicitas.
- Consolidar defaults comuns em `Directory.Build.props`.
- Remover propriedades repetidas dos `.csproj`.
- Habilitar nullable de forma controlada.
- Avaliar `Directory.Build.targets` somente com evidencia de target tardio necessario.
- Registrar baseline de warnings e validacao.

## Fora de escopo

- Corrigir toda a divida historica de formatacao.
- Criar pacote NuGet ou alterar empacotamento global.
- Migrar para SLNX.
- Adicionar analyzers externos.
- Criar migration apenas por efeito colateral de nullable.
