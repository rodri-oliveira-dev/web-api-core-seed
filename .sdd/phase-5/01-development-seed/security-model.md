# Security Model - Development Seed

## Regras

- O seed e um comando explicito e nunca roda implicitamente no startup HTTP normal.
- O seed e bloqueado em `Production` antes de aplicar migrations ou gravar dados.
- A senha do usuario de desenvolvimento e obrigatoria e deve vir de variavel de ambiente, User Secrets ou configuracao equivalente.
- `.env.local.example` pode conter somente placeholders claramente ilustrativos.
- A senha nunca deve ser escrita em log, SDD, testes, commit, PR ou OpenAPI.
- O seed nao emite JWT.
- O seed nao reduz politicas de senha do Identity.
- O seed nao usa `EnsureCreated`.
- O seed nao usa `HasData` para usuario, senha ou hash.

## Configuracao Planejada

- `DevelopmentSeed:User:Email`: email do usuario seedado.
- `DevelopmentSeed:User:Password`: senha local obrigatoria.
- `DevelopmentSeed:User:UserName`: opcional; default igual ao email.

Variaveis equivalentes:

- `DevelopmentSeed__User__Email`.
- `DevelopmentSeed__User__Password`.
- `DevelopmentSeed__User__UserName`.

## Logs

- Logs podem informar inicio, fim e contagens/acoes por tipo de dado.
- Logs nao devem incluir senha.
- Logs nao devem incluir token porque nenhum token e gerado.

## Limites Transacionais

- Identity e SampleRestaurant usam DbContexts separados.
- O seed aplica migrations e persiste Identity e SampleRestaurant em etapas separadas.
- Nao ha Unit of Work distribuida entre os dois contextos; falha no segundo contexto pode deixar Identity ja atualizado.
- No `SampleRestaurantDbContext`, a fronteira atomica do seed e um unico `SaveChangesAsync`, usando a transacao implicita do EF Core.
