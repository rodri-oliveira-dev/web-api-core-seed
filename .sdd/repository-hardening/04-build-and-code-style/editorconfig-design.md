# EditorConfig Design

## Regras portaveis

`.editorconfig` agora declara:

- `root = true`
- `charset = utf-8`
- `end_of_line = lf`
- `insert_final_newline = true`
- `trim_trailing_whitespace = true`
- `indent_style = space`
- `indent_size = 4`

Secoes especificas foram definidas para:

- `*.cs`
- `*.csproj`
- `*.props`
- `*.targets`
- `*.json`
- `*.yml`
- `*.yaml`
- `*.md`
- `*.sh`
- `*.ps1`

## C#

Foram declaradas preferencias explicitas para:

- qualification sem `this` quando desnecessario;
- `var` apenas quando o tipo for aparente;
- expression-bodied members de baixo risco;
- pattern matching;
- null propagation e coalesce;
- using fora do namespace;
- ordem de modificadores;
- namespace file-scoped como sugestao;
- braces como sugestao;
- interfaces com prefixo `I`;
- campos privados `_camelCase`;
- constantes PascalCase;
- parametros camelCase;
- metodos async com sufixo `Async`.

## Severidade

As preferencias subjetivas foram configuradas como `suggestion`, nao como erro. `EnforceCodeStyleInBuild=true` foi habilitado para o contrato existir no build, mas sem transformar gosto local em quebra de build.

## CA2007

`CA2007` permanece `silent` para aplicacao ASP.NET Core. O codigo de request pipeline nao precisa forcar `ConfigureAwait(false)`, e exigir isso aumentaria ruido sem beneficio no contexto da aplicacao.

## CS1591

`CS1591` fica decidido em um unico lugar: `.editorconfig`. A supressao local `NoWarn` da API foi removida.
