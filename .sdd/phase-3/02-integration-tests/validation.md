# Validation - 02 Integration Tests

Status: concluido.

## Comandos iniciais

| Comando | Resultado |
| --- | --- |
| `git status --short` | Limpo no inicio |
| `git branch --show-current` | `phase/3-quality-and-safety` |
| `git log -3 --oneline` | `d8730d3`, `f35b72a`, `e4be85c` |
| `dotnet build --configuration Release` | Passou com warnings herdados |
| `dotnet test --configuration Release --no-build` | Passou: 36 testes |
| `docker version` | Docker disponivel |
| `docker info` | Docker disponivel via Rancher Desktop/WSL2 |

## Validacao final

| Comando | Resultado |
| --- | --- |
| `dotnet restore` | Passou |
| `dotnet build --configuration Release --no-restore` | Passou sem warnings na ultima execucao incremental |
| `dotnet test --configuration Release --no-build` | Passou: 36 testes existentes + 18 testes de integracao |
| `dotnet test --configuration Release --no-build --filter "Category=Integration"` | Passou: 18 testes de integracao; `Pedidos.Test` sem match para a trait |
| `dotnet test test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --filter "Category=Integration"` | Passou em segunda execucao isolada: 18 testes |
| `docker ps -a` | Nao foram observados containers de teste remanescentes |
| `git diff --check` | Passou; apenas avisos de normalizacao LF/CRLF |

## Observacoes

- Tempo observado da suite completa com containers: cerca de 40 segundos.
- Tempo observado do filtro de integracao: cerca de 31 a 39 segundos no ambiente local.
- Docker estava disponivel; nao houve limitacao de runtime de container.
- Nenhum container de teste remanescente foi observado apos as execucoes.
