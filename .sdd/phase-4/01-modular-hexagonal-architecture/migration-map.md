# Migration Map - Prompt 01

| Caminho atual | Responsabilidade atual | Destino planejado | Acao nesta entrega | Acao adiada | Risco | Teste necessario |
| --- | --- | --- | --- | --- | --- | --- |
| `src/DevIO.Business/Models` | Entidades e validadores | `Restaurant/Domain` | Mover fisicamente preservando namespace | Separar dominio de exemplo no Prompt 2 | Quebra por includes ou namespaces | Build e testes unitarios |
| `src/DevIO.Business/Services` | Casos de uso legados | `Restaurant/Application/UseCases` | Mover fisicamente e ampliar portas de entrada de leitura | Redesenhar aplicacao por caso de uso quando justificar | Controller mudar comportamento | Testes HTTP e arquitetura |
| `src/DevIO.Business/Interfaces/Repository` | Portas de saida temporarias | `Restaurant/Application/Ports/Outbound` | Mover fisicamente preservando namespace | Portas orientadas ao dominio no Prompt 3 | Reforcar repositorio generico | Testes arquiteturais e build |
| `src/DevIO.Business/Interfaces/Service` | Portas de entrada temporarias | `Restaurant/Application/Ports/Inbound` | Mover fisicamente e expor consultas usadas por controllers | Casos de uso mais especificos quando houver necessidade | Interface maior temporaria | Testes unitarios e build |
| `src/DevIO.Data/Context` | EF Core context | `Restaurant/Infrastructure/Persistence/Context` | Mover fisicamente preservando namespace | Unit of Work explicito no Prompt 4 | EF design-time ou testes de integracao | Integration tests |
| `src/DevIO.Data/Mappings` | EF Core mappings | `Restaurant/Infrastructure/Persistence/Mappings` | Mover fisicamente preservando namespace | Revisar ownership de migrations no Prompt 6 | Model drift | Integration tests |
| `src/DevIO.Data/Repository` | Repositorios EF concretos | `Restaurant/Infrastructure/Persistence/Repositories` | Mover fisicamente preservando namespace | Repositorios orientados ao dominio no Prompt 3 | DI quebrada | Build e integration tests |
| `src/DevIO.Api/Controllers/V1/Controllers/PratosController.cs` | Entrada HTTP e leitura por repositorio | API input adapter | Remover injecao direta de repositorio | CancellationToken no Prompt 5 | Mudanca em retorno paginado | Regression HTTP e OpenAPI |
| `src/DevIO.Api/Controllers/V1/Controllers/MesasController.cs` | Entrada HTTP e leitura por repositorio | API input adapter | Remover injecao direta de repositorio | CancellationToken no Prompt 5 | Mudanca em 404/204 | Regression HTTP e OpenAPI |
| `src/DevIO.Api/Migrations` | Migrations Identity | Identity Infrastructure | Nenhuma | Prompt 6 | Migrations continuam na API | Documentacao e testes futuros |
