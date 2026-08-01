# Requirements

## Objetivo

Higienizar metadados e arquivos operacionais do repositorio sem alterar codigo produtivo, projetos, packages ou solution.

## Criterios de aceite

- Nenhuma referencia ativa a Sonar, SonarQube, SonarCloud ou SonarLint.
- Nenhum caminho absoluto de maquina.
- `.gitignore` adequado a solution atual.
- `launchSettings.json` nao ignorado globalmente.
- Arquivos locais sensiveis continuam ignorados.
- Finais de linha e binarios classificados em `.gitattributes`.
- `.dockerignore` exclui apenas conteudos que nao devem entrar no contexto de build.
- Arquivos necessarios ao futuro Docker build nao sao ignorados.
- Build e testes continuam passando.

## Fora de escopo

- Codigo C#.
- `.csproj`, solution ou packages.
- Mudancas funcionais.
- Dockerfile da API.
