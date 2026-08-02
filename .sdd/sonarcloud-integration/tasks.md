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

- [-] Geracao de cobertura
  - [x] Ajustar Coverlet Collector no workflow para gerar OpenCover.
  - [x] Preservar o artefato de cobertura existente com Cobertura.
  - [x] Incluir OpenCover no artefato de cobertura.
  - [!] Confirmar em CI que os arquivos `coverage.opencover.xml` sao gerados em `TestResults/**`.

- [x] Configuracao de exclusoes
  - [x] Aplicar exclusoes estreitas para cobertura e duplicacao.
  - [x] Confirmar por revisao do YAML que codigo de producao escrito manualmente continua analisado.
  - [x] Registrar exclusoes sem adicionar exclusao ampla de codigo-fonte.

- [-] Validacao local
  - [x] Validar YAML com parser local disponivel.
  - [!] Executar `actionlint .github/workflows/ci.yml` quando a ferramenta estiver disponivel.
  - [x] Executar `git diff --check`.
  - [x] Executar `dotnet --info`.
  - [x] Executar `dotnet restore WebApiCoreSeed.slnx`.
  - [x] Executar `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`.
  - [!] Confirmar geracao de TRX e OpenCover em CI com `SONAR_TOKEN` configurado.

- [ ] Documentacao externa
  - Documentar importacao do projeto no SonarCloud.
  - Documentar desativacao da analise automatica do SonarCloud.
  - Documentar criacao do secret `SONAR_TOKEN`.
  - Documentar configuracao de New Code e Quality Gate.

- [ ] Protecao da branch
  - Executar o workflow ao menos uma vez.
  - Configurar status checks obrigatorios em `main`.
  - Incluir o status do Quality Gate quando disponivel.

- [ ] Validacao final
  - Confirmar analise visivel no SonarCloud.
  - Confirmar PR analysis.
  - Confirmar falha do workflow quando o Quality Gate reprovar.
  - Registrar pendencias e riscos remanescentes.
