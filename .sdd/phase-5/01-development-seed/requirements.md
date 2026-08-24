# Requirements - Development Seed

## Objetivo

Adicionar um mecanismo explicito, deterministico, idempotente e seguro para popular ambientes locais de desenvolvimento com dados representativos de Identity e do modulo `SampleRestaurant`.

## Escopo

- Comando explicito para aplicar migrations e executar seed em um checkout limpo.
- Usuario de desenvolvimento autenticavel pelo fluxo real de login.
- Claims minimas para exercitar endpoints protegidos representativos.
- Dados representativos de `SampleRestaurant`: pratos, mesas e um conjunto minimo relacionado para demonstracao.
- Idempotencia por chaves deterministicas e chaves naturais controladas pelo seed.
- Configuracao externa para credencial de desenvolvimento.
- Bloqueio completo em ambiente `Production`.
- Testes unitarios/leves e testes de integracao com SQL Server por Testcontainers.
- Documentacao operacional minima em README, `.env.local.example` e docs de desenvolvimento local.

## Fora de Escopo

- Seed automatico no startup HTTP normal.
- Seed produtivo, admin produtivo ou credenciais versionadas.
- Mudancas de politica de senha do Identity.
- `EnsureCreated`, `HasData` para senha/hash Identity ou scripts SQL de insert.
- Unificar `ApplicationDbContext` e `SampleRestaurantDbContext`.
- Refatoracoes de nomenclatura, xUnit ou arquitetura fora do seed.

## Criterios de Aceite

- `dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed` aplica migrations e executa o seed em ambiente nao produtivo.
- O comando falha de modo seguro quando `DevelopmentSeed:User:Password` estiver ausente.
- O comando falha antes de alterar dados quando `ASPNETCORE_ENVIRONMENT`/`DOTNET_ENVIRONMENT` for `Production`.
- A primeira e segunda execucao produzem o mesmo conjunto logico de dados sem duplicacao.
- Dados criados pelo usuario fora das chaves do seed sao preservados.
- Um dado conhecido de seed pode ser atualizado com seguranca em nova execucao.
- O usuario seedado consegue fazer login pelo endpoint existente.
- O token obtido pelo login acessa um endpoint protegido representativo.
- Migrations dos dois DbContexts sao aplicadas antes da persistencia de dados.
- Nao ha senha, token ou segredo real no diff.
