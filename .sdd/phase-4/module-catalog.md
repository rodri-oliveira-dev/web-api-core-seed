# Module Catalog - Phase 4

## SampleRestaurant

| Campo | Conteudo |
| --- | --- |
| Capacidade de negocio | Operacao de restaurante de exemplo. |
| Responsabilidade | Gerenciar pratos, mesas, pedidos, itens de pedido, atendentes e logs legados associados ao exemplo. |
| Entidades principais | `Prato`, `Mesa`, `Pedido`, `PedidoPrato`, `Atendente`, `LogginEntity`. |
| Casos de uso | Criar, atualizar, remover e consultar pratos e mesas; manter operacoes legadas de pedidos, atendentes e logs. |
| Dados pertencentes | Tabelas `Pratos`, `Mesas`, `Pedidos`, `PedidoPrato`, `Atendentes`, `Loggin`. |
| Contratos publicos | Endpoints HTTP versionados atuais em `/api/v{version}`; view models do sample permanecem na API nesta entrega. |
| Dependencias permitidas | Domain sem infraestrutura; Application depende de Domain; Infrastructure depende do nucleo do modulo; API depende de Application e Infrastructure para composicao. |
| Dependencias proibidas | Domain nao pode depender de API, EF Core, ASP.NET Core, Redis, Identity ou logging; Application nao pode depender de API, EF Core, ASP.NET Core ou Infrastructure concreta; Infrastructure nao deve ser consumida diretamente por outro modulo de negocio. |
| Maturidade do limite | Inicial. O limite fisico e nominal deixa claro que este e o dominio demonstrativo do seed; separacoes mais finas estao planejadas para os prompts seguintes. |

## Identity

| Campo | Conteudo |
| --- | --- |
| Capacidade de negocio | Registro, login e emissao de JWT para acesso aos endpoints. |
| Responsabilidade | Autenticacao e autorizacao por ASP.NET Core Identity, claims e JWT. |
| Entidades principais | `IdentityUser`, `IdentityRole` e tabelas `AspNet*`. |
| Casos de uso | Registrar usuario, autenticar usuario, gerar token JWT. |
| Dados pertencentes | Tabelas `AspNetUsers`, `AspNetRoles`, `AspNetUserClaims`, `AspNetUserRoles`, `AspNetUserLogins`, `AspNetUserTokens`, `AspNetRoleClaims`. |
| Contratos publicos | `POST /api/v1/nova-conta`, `POST /api/v1/entrar`, `POST /api/v2/entrar`. |
| Dependencias permitidas | API e infraestrutura de Identity do ASP.NET Core nesta etapa. |
| Dependencias proibidas | Nao deve acessar detalhes internos do modulo `SampleRestaurant`; nao deve depender dos repositorios de restaurante. |
| Maturidade do limite | Imaturo. Permanece acoplado ao adaptador API por pragmatismo e deve ser reavaliado apos a separacao do dominio de exemplo. |
