# Reusable vs Sample Matrix - Prompt 02

| Componente | Classificacao | Justificativa |
| --- | --- | --- |
| `WebApiCoreSeed.Api` hosting/Program | Composition Root | Inicializa o seed e compoe recursos reutilizaveis com o sample. |
| API configuration (`ApiConfig`, `HostingConfig`, `RateLimitConfig`, `OpenTelemetryConfig`, `OpenApiConfig`) | Reusable | Configuracao tecnica comum da API. |
| Problem Details, results e error handlers | Reusable | Tratamento HTTP tecnico sem regra de negocio. |
| Health checks, Redis cache, Serilog, OpenTelemetry settings | Reusable | Infraestrutura configuravel do template. |
| Identity controllers, settings e migrations | Composition Root | Capacidade tecnica/Identity ainda hospedada na API ate prompts futuros. |
| `SampleRestaurant` domain models (`Prato`, `Mesa`, `Pedido`, `Atendente`, `LogginEntity`) | Sample | Entidades do dominio demonstrativo. |
| `SampleRestaurant` validators e enums | Sample | Regras e vocabulário especificos do exemplo. |
| `SampleRestaurant` application services e ports | Sample | Casos de uso e portas temporarias do modulo de exemplo. |
| `SampleRestaurant.Infrastructure` DbContext, mappings, repositories e migrations | Sample | Persistencia especifica do exemplo. |
| Sample controllers `PratosController` e `MesasController` | Sample | Adaptadores HTTP do dominio demonstrativo; rotas preservadas. |
| Sample view models (`PratoViewModel`, `MesaViewModel`, `Pedido*`, `Atendente`) | Sample | DTOs especificos do exemplo. |
| User/auth view models | Composition Root | Contratos de autenticacao hospedados na API. |
| Pagination contracts atuais | Sample | Ainda acoplados aos repositorios/casos de uso do exemplo; Prompt 7 revisara paginacao. |
| `WebApiCoreSeed.Tests` | Reusable e Sample | Projeto de testes compartilhado, com pastas por arquitetura, integracao leve e unitarios do sample. |
| `WebApiCoreSeed.IntegrationTests` | Reusable e Sample | Testa pipeline reutilizavel e persistencia do sample em infraestrutura real. |
| `OpenApiGenerator` | Reusable tooling | Ferramenta de geracao de contrato do seed. |
| `LEGACY.md` e SDD antigo | Obsolete | Registro historico; pode conter nomes legados contextualizados. |
| Shared Kernel | Shared Kernel | Nao ha Shared Kernel produtivo nesta entrega; nenhum tipo do exemplo foi promovido por conveniencia. |
