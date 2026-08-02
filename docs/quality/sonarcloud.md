# SonarCloud Quality Gate

## Visao geral

A integracao com SonarCloud complementa o workflow `ci` com analise estatica, importacao de cobertura, importacao de resultados de testes e avaliacao sincronizada do Quality Gate.

O fluxo atual roda em GitHub Actions:

1. checkout com historico completo;
2. setup do SDK definido em `global.json`;
3. caches separados para NuGet e SonarCloud;
4. instalacao pinada do `dotnet-sonarscanner`;
5. restore;
6. `sonarscanner begin`;
7. build;
8. testes unitarios e de integracao com TRX, Cobertura e OpenCover;
9. `sonarscanner end` aguardando o Quality Gate;
10. geracao e verificacao de OpenAPI;
11. auditorias de pacotes vulneraveis e obsoletos;
12. upload de artefatos com `if: always()`.

Os dados enviados ao SonarCloud incluem metricas de codigo C#, problemas de confiabilidade, seguranca e manutenibilidade, security hotspots, relatorios TRX e cobertura OpenCover. Cobertura continua sendo publicado como artefato do workflow, mas o SonarCloud usa OpenCover por meio de `sonar.cs.opencover.reportsPaths`.

O Quality Gate e avaliado no SonarCloud. Como o workflow usa `sonar.qualitygate.wait=true`, o passo `End SonarCloud analysis and enforce Quality Gate` aguarda o resultado ate o timeout configurado. Se o gate reprovar ou o timeout expirar, o job falha.

## Pre-requisitos

- Acesso a organizacao SonarCloud `rodri-oliveira-dev`.
- Permissao administrativa no repositorio GitHub `rodri-oliveira-dev/web-api-core-seed`.
- Projeto importado ou vinculado no SonarCloud.
- GitHub Actions habilitado no repositorio.
- Secret `SONAR_TOKEN` configurado no GitHub antes da primeira analise bem-sucedida.

## Importacao do projeto

1. Acesse o SonarCloud com uma conta que tenha acesso a organizacao `rodri-oliveira-dev`.
2. Entre na organizacao `rodri-oliveira-dev`.
3. Importe o repositorio GitHub `web-api-core-seed`.
4. Confirme a chave do projeto:

   ```text
   rodri-oliveira-dev_web-api-core-seed2
   ```

5. Mantenha `main` como branch principal do projeto.
6. Use analise baseada em CI, porque o projeto precisa de restore, build, testes e relatorios gerados no runner.
7. Desative a analise automatica do SonarCloud para evitar analises duplicadas e resultados sem TRX/OpenCover.

A branch `phase/4-architecture-modernization` tambem esta configurada para `push` no workflow. A analise dessa branch pode depender do plano e da configuracao do SonarCloud. Quando branch analysis nao estiver disponivel, use um pull request para `main` como caminho de validacao.

## Token

Crie um token no SonarCloud para a analise CI e armazene-o somente como secret do GitHub.

Nome obrigatorio do secret:

```text
SONAR_TOKEN
```

Local no GitHub:

```text
Settings
-> Secrets and variables
-> Actions
-> Repository secrets
```

Nunca registre o valor do token no repositorio, em arquivos versionados, em exemplos de documentacao, em logs ou no historico do shell. O workflow deve referenciar apenas `secrets.SONAR_TOKEN`; o valor real pertence a configuracao externa.

Para rotacionar o token:

1. gere um novo token no SonarCloud;
2. atualize o secret `SONAR_TOKEN` no GitHub;
3. execute o workflow;
4. confirme que a analise foi aceita;
5. revogue o token antigo no SonarCloud.

## Quality Gate

Use inicialmente o `Sonar way` para estabelecer a integracao com baixo atrito. Depois da primeira execucao, avalie se o projeto precisa de um Quality Gate especifico.

Para este repositorio, prefira condicoes sobre New Code. Isso permite bloquear regressao sem exigir que toda a divida historica seja resolvida no mesmo passo de modernizacao.

Uma politica sugerida, sujeita a maturidade real do projeto, e:

```text
Coverage on New Code >= 80%
Duplicated Lines on New Code <= 3%
Reliability Rating on New Code = A
Security Rating on New Code = A
Maintainability Rating on New Code = A
Security Hotspots Reviewed on New Code = 100%
```

Revise esses valores apos a primeira analise real. Projetos em modernizacao podem precisar de um gate inicial mais incremental, desde que ele ainda proteja New Code contra regressao.

O workflow define:

```text
sonar.qualitygate.wait=true
sonar.qualitygate.timeout=300
```

Com isso, o job aguarda o Quality Gate por ate 300 segundos. Gate vermelho ou timeout retornam falha para o passo do scanner.

## Protecao da branch

Configure a protecao da `main` depois que o workflow tiver executado ao menos uma vez, pois o GitHub so permite selecionar checks que ja existem.

Caminho no GitHub:

```text
Settings
-> Rules
-> Rulesets
```

Configuracao recomendada:

- proteger a branch `main`;
- exigir pull request antes de merge;
- exigir status checks obrigatorios;
- exigir o workflow de build, testes e qualidade;
- exigir o check do SonarCloud quando ele estiver disponivel apos a primeira execucao;
- exigir branch atualizada antes do merge;
- bloquear merge quando o Quality Gate falhar.

Nao fixe nesta documentacao um nome exato para o check do SonarCloud antes da primeira execucao. Selecione o check exibido pelo GitHub depois que SonarCloud publicar o primeiro status.

## Branches e pull requests

Comportamento atual do workflow:

- `push` em `main`: roda CI e analise SonarCloud;
- `push` em `phase/4-architecture-modernization`: roda CI e tenta analise SonarCloud quando branch analysis estiver disponivel;
- `pull_request` para `main`: roda CI e tenta analise de pull request;
- `workflow_dispatch`: permite execucao manual.

A disponibilidade de analise de branch e alguns recursos de pull request podem depender do plano SonarCloud e da vinculacao do projeto com GitHub.

Pull requests vindos de forks podem nao receber secrets do repositorio. Nesses casos, o passo do SonarCloud pode nao ter `SONAR_TOKEN`. Trate esse resultado como uma limitacao segura do GitHub Actions e valide a mudanca por um branch interno ou por um maintainer rerun quando a politica do repositorio permitir. Nunca exponha o token para contornar essa limitacao.

## Troubleshooting

`SONAR_TOKEN` ausente: confirme que o secret existe em `Settings -> Secrets and variables -> Actions` e que o nome esta exatamente `SONAR_TOKEN`.

Chave do projeto incorreta: confirme no SonarCloud se a chave e `rodri-oliveira-dev_web-api-core-seed2` e ajuste o workflow apenas se a chave real for diferente.

Organizacao incorreta: confirme se o projeto esta na organizacao `rodri-oliveira-dev`.

Relatorio OpenCover nao encontrado: confirme se os testes geraram `TestResults/Unit/*/coverage.opencover.xml` e `TestResults/Integration/*/coverage.opencover.xml`.

Arquivo TRX nao encontrado: confirme se existem `TestResults/Unit/unit-tests.trx` e `TestResults/Integration/integration-tests.trx`.

Cobertura exibida como zero: confirme que o formato `opencover` foi gerado, que os caminhos do scanner apontam para os arquivos diretos de cada suite e que as exclusoes nao removeram codigo de produto.

Shallow clone: confirme que `actions/checkout` usa `fetch-depth: 0`.

Quality Gate timeout: confirme se o projeto aparece no SonarCloud. Se houver evidencia de processamento normal acima de 300 segundos, aumente o timeout em um commit separado.

Analise automatica duplicada: desative a analise automatica no SonarCloud quando usar analise baseada em CI.

Scanner fora do ciclo de build: o build precisa ocorrer depois de `sonarscanner begin` e antes de `sonarscanner end`.

Falha do `XPlat Code Coverage`: confirme a presenca de `coverlet.collector` nos projetos de teste e mantenha os argumentos de runsettings apos `--`.

Incompatibilidade de versao do scanner: o workflow instala `dotnet-sonarscanner` com versao explicita. Atualize a versao deliberadamente e valide restore, build, testes e analise.

## Execucao local

A reproducao local cobre restore, build, testes, TRX, OpenCover, OpenAPI e auditorias de pacotes. A analise completa depende de token e do servico SonarCloud, portanto nao deve ser marcada como concluida localmente sem uma execucao real no GitHub Actions/SonarCloud.

Comandos locais equivalentes aos gates reproduziveis:

```bash
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test tests/WebApiCoreSeed.UnitTests/WebApiCoreSeed.UnitTests.csproj --configuration Release --no-build --logger "trx;LogFileName=unit-tests.trx" --results-directory TestResults/Unit --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
dotnet test tests/WebApiCoreSeed.IntegrationTests/WebApiCoreSeed.IntegrationTests.csproj --configuration Release --no-build --logger "trx;LogFileName=integration-tests.trx" --results-directory TestResults/Integration --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.Test]*,[*.Tests]*,[*.UnitTests]*,[*.IntegrationTests]*" "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile=**/*.generated.cs"
dotnet run --project tools/OpenApiGenerator/OpenApiGenerator.csproj --configuration Release --no-build
git diff --exit-code -- docs/openapi/openapi-v1.json docs/openapi/openapi-v2.json
dotnet list WebApiCoreSeed.slnx package --vulnerable
dotnet list WebApiCoreSeed.slnx package --deprecated
```

Nao coloque o token em arquivos versionados nem em comandos que fiquem gravados no historico do shell. Para validar a analise completa, prefira o workflow do GitHub Actions com o secret configurado.
