# Handoff

## Estado entregue pelo Prompt 1

- `.gitignore` modernizado e ajustado para a solution atual.
- `.gitattributes` criado com normalizacao de texto, finais de linha e classificacao de binarios.
- `.dockerignore` criado para futuro build containerizado sem excluir arquivos de restore/build.
- Artefatos ativos de Sonar removidos de `src/`.
- Caminhos pessoais historicos sanitizados.
- Inventario operacional documentado em `repository-inventory.md`.
- Baseline de validacao registrada em `validation-baseline.md` e `01-repository-hygiene/validation.md`.

## Proximo prompt

Prompt 2 deve tratar layout e namespaces sem reintroduzir arquivos de analise removidos neste prompt. Antes de mover arquivos, validar:

- `git status --short`
- `git ls-files src test tools`
- referencias de namespace e `RootNamespace`
- impactos em `.sln`, `.csproj`, testes e docs SDD

## Riscos para acompanhar

- `src/WebApiCoreSeed.Api/Properties/launchSettings.json` nao existe hoje; se for criado em prompt futuro, nao sera bloqueado pelo `.gitignore`.
- A pasta `tests/` nao existe; o layout atual usa `test/`.
- Dockerfile da API ainda nao existe; `.dockerignore` foi preparado de forma portavel e conservadora.
