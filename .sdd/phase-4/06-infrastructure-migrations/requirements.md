# Requirements - Prompt 06

## Objetivo

Migrations EF Core pertencem ao adaptador de persistencia que implementa o schema. A API permanece como composition root e adaptador HTTP, sem armazenar classes de migration.

## Escopo

- Mover migrations de Identity para uma infraestrutura propria.
- Manter migrations do `SampleRestaurantDbContext` no projeto `WebApiCoreSeed.SampleRestaurant.Infrastructure`.
- Configurar `MigrationsAssembly` explicitamente para todos os DbContexts relacionais ativos.
- Adicionar factories design-time para comandos `dotnet ef` reproduziveis.
- Documentar comandos reais por contexto.
- Validar aplicacao de migrations em SQL Server descartavel via Testcontainers.

## Fora de Escopo

- Recriar, consolidar ou reordenar migrations antigas.
- Alterar schema sem requisito funcional.
- Unificar `ApplicationDbContext` e `SampleRestaurantDbContext`.
- Introduzir ferramenta externa de migration.
- Aplicar migrations em banco local do usuario.
- Criar seed runtime novo.

## Criterios de Aceite

- API nao contem pasta nem classes de migrations.
- `ApplicationDbContext` pertence a uma infraestrutura de Identity.
- `SampleRestaurantDbContext` continua pertencendo a infraestrutura do sample.
- IDs, timestamps, operacoes e snapshots das migrations antigas sao preservados.
- `dotnet ef dbcontext list` e `dotnet ef migrations list` funcionam com comandos documentados.
- Banco SQL Server vazio e atualizado por migrations nos testes de integracao.
- Nao ha seed produtivo automatico; ausencia de seed e documentada.
