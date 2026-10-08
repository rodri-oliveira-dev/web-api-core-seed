# Distribuição do template `dotnet new`

[English](template-distribution.md) | [Português (Brasil)](template-distribution.pt-BR.md) | [Voltar ao README](../README.pt-BR.md)

O repositório agora pode ser **empacotado localmente** como template NuGet. **O pacote ainda não foi publicado no NuGet.org nem existe uma GitHub Release v2.0.0**; a publicação está na [#23](https://github.com/rodri-oliveira-dev/web-api-core-seed/issues/23).

## Identidade e conteúdo do pacote

| Propriedade | Valor |
| --- | --- |
| Identidade | `RodriOliveira.WebApiCoreSeed.CSharp` |
| Nome curto | `webapi-seed` |
| ID do pacote | `RodriOliveira.WebApiCoreSeed.Templates` |
| Versão | `2.0.0-preview.1` (preview local) |
| SDK | `global.json` — .NET 10.0.401 |
| Token de origem | `WebApiCoreSeed`, substituído pelo nome passado em `-n` |

O NuGet contém apenas projetos reutilizáveis, testes, gerador/contratos OpenAPI, scripts de Compose/Docker, configurações básicas e READMEs do projeto gerado em EN/PT-BR. **Não inclui** CI, histórico SDD, administração de contribuições, segredos ou saídas de compilação. Cada API gerada recebe um `UserSecretsId` novo, distinto do projeto histórico.

### Criar e instalar localmente

Na raiz do repositório:

```bash
dotnet pack template-pack/WebApiCoreSeed.Templates.csproj -c Release -o ./artifacts/templates
dotnet new install ./artifacts/templates/RodriOliveira.WebApiCoreSeed.Templates.2.0.0-preview.1.nupkg
dotnet new webapi-seed -n SampleApi
cd SampleApi
dotnet restore SampleApi.slnx
dotnet build SampleApi.slnx --configuration Release --no-restore
```

O projeto gerado chama-se `SampleApi`: nomes, namespaces, referências da solução, Dockerfile e scripts recebem o nome novo. O Docker Compose utiliza um nome padrão seguro (`web-api-core-seed`) independente do nome .NET; configure `COMPOSE_PROJECT_NAME` no `.env.local` gerado com um valor único em letras minúsculas, como `sample-api`, para evitar conflitos. Nomes como `Acme.Api` continuam válidos em C# e geram um Compose válido. Os READMEs gerados preservam os links ao repositório original.

### Executar a aplicação gerada

Com Docker ativo, copie `.env.local.example` para `.env.local`, substitua as credenciais e execute:

```bash
docker compose --env-file .env.local up --build -d
docker compose --env-file .env.local --profile tools up seed
curl -i http://localhost:8080/health/live
```

O resultado contém testes unitários/integração e dados de exemplo. Testes de integração utilizam Testcontainers. Veja o README gerado para personalizar banco e migrations.

### Atualizar ou desinstalar

Para atualizar um pacote **local**, desinstale a versão anterior e instale o novo `.nupkg`:

```bash
dotnet new uninstall RodriOliveira.WebApiCoreSeed.Templates
dotnet new install ./artifacts/templates/RodriOliveira.WebApiCoreSeed.Templates.2.0.0-preview.1.nupkg
```

Para remover o template:

```bash
dotnet new uninstall RodriOliveira.WebApiCoreSeed.Templates
```

Para publicação futura em feed, `dotnet new install RodriOliveira.WebApiCoreSeed.Templates` e `dotnet new update` serão aplicáveis após a release; o pacote **não está disponível publicamente** neste momento.

## Parâmetros opcionais avaliados

Autenticação, SQL Server, Redis e OpenTelemetry fazem parte do seed mantido. Não oferecemos `--auth`, `--database`, `--redis` ou `--telemetry` em tempo de geração por enquanto: remover esses componentes envolve DI, dependências, endpoints, migrations, testes e configurações. Alterar apenas `appsettings.json` não é suficiente. Redis e OpenTelemetry podem ser desativados **em runtime**. Flags opcionais só devem ser criadas com testes de todas as combinações relevantes.

## Qualidade

Execute `bash scripts/templates/smoke-test.sh` na raiz do repositório: empacota, inspeciona, instala em ambiente isolado, gera `SampleApi`, verifica nomes e UserSecretsId, restaura, compila, executa testes unitários/integração e verifica o health endpoint da API gerada. O mesmo script roda no GitHub Actions [template-smoke](../.github/workflows/template-smoke.yml). Requer Docker e não publica pacotes.
