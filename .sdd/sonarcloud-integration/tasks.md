# Tasks - SonarCloud Integration

## Status Legend

```text
[ ] Pendente
[-] Em andamento
[x] Concluido
[!] Bloqueado
```

## Execution Plan

- [x] Especificacao
  - Inventariar estado atual do projeto, testes, cobertura e CI.
  - Criar a pasta `.sdd/sonarcloud-integration/`.
  - Registrar contexto, requisitos, design, decisoes, validacao e log de execucao.
  - Registrar a branch atual `phase/4-architecture-modernization`.

- [-] Integracao do scanner
  - [!] Confirmar o projeto no SonarCloud.
  - [!] Confirmar a chave `rodri-oliveira-dev_web-api-core-seed` no SonarCloud.
  - [x] Instalar SonarScanner for .NET com versao explicita `11.2.1` no workflow.
  - [!] Adicionar `SONAR_TOKEN` somente como secret externo no GitHub.
  - [x] Referenciar apenas `secrets.SONAR_TOKEN`, sem versionar ou imprimir o valor.
  - [x] Envolver build e testes entre `sonarscanner begin` e `sonarscanner end`.

- [x] Geracao de cobertura
  - [x] Ajustar Coverlet Collector no workflow para gerar OpenCover.
  - [x] Preservar o artefato de cobertura existente com Cobertura.
  - [x] Incluir OpenCover no artefato de cobertura.
  - [x] Filtrar assemblies de teste no Coverlet Collector para evitar cobertura artificial de projetos de teste.
  - [x] Excluir arquivos `*.generated.cs` na coleta de cobertura para remover codigo gerado pelo source generator.
  - [x] Usar globs suite-scoped para OpenCover e TRX no SonarCloud.
  - [x] Confirmar localmente que os arquivos `coverage.opencover.xml` sao gerados em `TestResults/Unit/*/` e `TestResults/Integration/*/`.
  - [!] Confirmar em CI que os arquivos `coverage.opencover.xml` sao gerados nos mesmos caminhos com `SONAR_TOKEN` configurado.

- [x] Configuracao de exclusoes
  - [x] Aplicar exclusoes estreitas para cobertura e duplicacao.
  - [x] Confirmar por revisao do YAML que codigo de producao escrito manualmente continua analisado.
  - [x] Registrar exclusoes sem adicionar exclusao ampla de codigo-fonte.

- [x] Validacao local
  - [x] Validar YAML com parser local disponivel.
  - [!] Executar `actionlint .github/workflows/ci.yml` quando a ferramenta estiver disponivel.
  - [x] Executar `git diff --check`.
  - [x] Executar `dotnet --info`.
  - [x] Executar `dotnet restore WebApiCoreSeed.slnx`.
  - [x] Executar `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`.
  - [x] Executar testes unitarios com TRX e OpenCover.
  - [x] Executar testes de integracao com TRX e OpenCover.
  - [x] Validar XMLs OpenCover quanto a tamanho, XML valido, modulos, classes, linhas cobertas e nao cobertas.
  - [x] Executar geracao de OpenAPI local.
  - [x] Validar JSON dos contratos OpenAPI.
  - [x] Verificar sincronizacao dos contratos OpenAPI com `git diff --exit-code`.
  - [x] Executar auditoria de pacotes vulneraveis.
  - [x] Executar relatorio de pacotes obsoletos.
  - [x] Confirmar que `TestResults/` esta ignorado pelo `.gitignore`.
  - [!] Confirmar geracao de TRX e OpenCover em CI com `SONAR_TOKEN` configurado.

- [x] Documentacao externa
  - [x] Documentar importacao do projeto no SonarCloud.
  - [x] Documentar desativacao da analise automatica do SonarCloud.
  - [x] Documentar criacao do secret `SONAR_TOKEN`.
  - [x] Documentar configuracao de New Code e Quality Gate.
  - [x] Documentar protecao de branch e selecao de checks apos primeira execucao.
  - [x] Documentar limitacoes de branch analysis, pull requests e forks.
  - [x] Documentar troubleshooting e execucao local.

- [!] Protecao da branch
  - [!] Executar o workflow ao menos uma vez.
  - [!] Configurar status checks obrigatorios em `main`.
  - [!] Incluir o status do Quality Gate quando disponivel.

- [-] Validacao final
  - [x] Registrar validacoes locais executadas.
  - [x] Registrar pendencias e riscos remanescentes.
  - [!] Confirmar analise visivel no SonarCloud.
  - [!] Confirmar PR analysis.
  - [!] Confirmar falha do workflow quando o Quality Gate reprovar.
