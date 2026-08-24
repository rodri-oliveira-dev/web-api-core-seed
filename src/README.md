# Source Layout

Esta pasta contém a aplicação ativa do `web-api-core-seed`, modernizada para .NET 10.

## Projetos

- `WebApiCoreSeed.Api`: adaptador HTTP e composition root.
- `Modules/Identity/WebApiCoreSeed.Identity.Infrastructure`: persistência do Identity.
- `Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant`: domínio e aplicação do exemplo de restaurante.
- `Modules/SampleRestaurant/WebApiCoreSeed.SampleRestaurant.Infrastructure`: persistência EF Core do exemplo.

## Tecnologias Ativas

- .NET 10
- ASP.NET Core Web API com JWT Bearer Authentication
- ASP.NET Identity Core
- Entity Framework Core 10
- AutoMapper
- FluentValidation
- OpenAPI com Scalar UI e suporte a JWT
- Health checks
- Redis opcional
- Rate limiting nativo do ASP.NET Core
- Serilog

## Compatibilidade

O histórico legado permanece preservado fora desta pasta ativa. Não renomeie tabelas, colunas ou migrations legadas apenas para corrigir nomenclatura.
