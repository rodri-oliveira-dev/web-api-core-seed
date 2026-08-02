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

- [ ] Integracao do scanner
  - Confirmar o projeto no SonarCloud.
  - Confirmar a chave `rodri-oliveira-dev_web-api-core-seed`.
  - Instalar SonarScanner for .NET com versao explicita.
  - Adicionar `SONAR_TOKEN` somente como secret externo.
  - Envolver build e testes entre `sonarscanner begin` e `sonarscanner end`.

- [ ] Geracao de cobertura
  - Ajustar Coverlet Collector para gerar OpenCover.
  - Preservar ou atualizar conscientemente o artefato de cobertura existente.
  - Confirmar que os arquivos `coverage.opencover.xml` sao gerados em `TestResults/**`.

- [ ] Configuracao de exclusoes
  - Aplicar exclusoes estreitas para cobertura e duplicacao.
  - Confirmar que codigo de producao escrito manualmente continua analisado.
  - Registrar qualquer exclusao adicional com justificativa.

- [ ] Validacao local
  - Validar YAML.
  - Executar restore, build, testes unitarios e testes de integracao quando o ambiente suportar.
  - Confirmar geracao de TRX e OpenCover.
  - Revisar `git diff --check`.

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
