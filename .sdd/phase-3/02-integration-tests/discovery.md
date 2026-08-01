# Discovery - 02 Integration Tests

## Solution e projetos

- Solution: `RestauranteAPI.sln`.
- API: `src/DevIO.Api/Restaurante.IO.Api.csproj`.
- Business: `src/DevIO.Business/Restaurante.IO.Business.csproj`.
- Data: `src/DevIO.Data/Restaurante.IO.Data.csproj`.
- Testes existentes: `test/Pedidos.Test/Pedidos.Test.csproj`.
- Novo projeto: `test/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj`.

## DbContext e migrations

- `ApplicationDbContext`: Identity, migration `20200817223121_InitialCreate`, tabelas `AspNet*`.
- `MeuDbContext`: dominio legado, migration `20200817223231_InitialCreate`, tabelas `Atendentes`, `Mesas`, `Pratos`, `Pedidos`, `PedidoPrato`, `Loggin`.
- `Program` ja e `public class Program`; `public partial class Program` nao foi necessario.

## Configuracao e infraestrutura

- `DefaultConnection` em `appsettings.json` aponta para `localhost,1433`; os testes sobrescrevem por variaveis de ambiente e configuracao in-memory antes do host montar servicos.
- Redis e configurado por `RedisCacheSettings`; os testes mantem `Enabled=true` e apontam para container.
- `DatasulSeqSettings.Enabled=false` nos testes para remover health check externo de Seq.
- Health check ativo: `/hc`.
- OpenAPI ativo: `/openapi/v1.json` e `/openapi/v2.json`.
- Rate limiting nativo ativo com politicas `public`, `authenticated` e `authentication-sensitive`.

## Autenticacao e autorizacao

- JWT configurado em `IdentityConfig`.
- Testes geram token HS384 com issuer/audience do ambiente `Testing`.
- Claims customizadas sao validadas por `ClaimsAuthorizeAttribute` e `RequisitoClaimFilter`.

## Achado de integracao

Com SQL Server real, `MeuDbContext` falhava ao montar o modelo porque `Atendente.Telefone` era descoberto como entidade sem chave. A migration legada de `Atendentes` tambem nao contem `Email` nem `Telefone`. O mapping de `Atendente` foi ajustado para ignorar essas propriedades e preservar o schema versionado atual.
