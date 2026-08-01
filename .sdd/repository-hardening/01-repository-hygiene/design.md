# Design

## `.gitignore`

Usar uma versao menor que o template completo do Visual Studio, mantendo as regras relevantes para:

- Visual Studio, Rider e VS Code local.
- `bin/`, `obj/`, logs, resultados de testes e cobertura.
- caches locais de SDK, NuGet e ferramentas.
- arquivos sensiveis locais como `.env`, certificados e publish profiles.
- excecoes explicitas para `.env.example`, `.env.local.example`, `.globalconfig`, `Directory.Build.*`, `Directory.Packages.props`, `*.http` e `docs/openapi/`.
- permitir `**/Properties/launchSettings.json`.

## `.gitattributes`

Definir LF como padrao, scripts shell e githooks com LF, batch/cmd com CRLF e arquivos binarios como imagens, pacotes e outputs compilados.

## `.dockerignore`

Criar ignore conservador para remover do contexto:

- `.git` e estado local de IDE.
- `bin/`, `obj/`, resultados de teste, cobertura e artefatos.
- caches locais e secrets.
- documentacao SDD e agentes, que nao participam do build containerizado.

Nao excluir source code, `.csproj`, `.props`, `.targets`, solution, `global.json`, docs OpenAPI ou dockerfiles auxiliares.
