# Operação da release — v2.0.0

[Notas da versão](v2.0.0.md) | [README](../../README.pt-BR.md) | [English](release-checklist.md)

## Processo de publicação

O workflow `release-v2.yml` inicia ao fazer merge para a **`main`** (com `workflow_dispatch` para reexecuções).

1. Confere se já existem a GitHub Release e a tag `v2.0.0`; **nunca** substitui ou move referências históricas.
2. Exige `ci`, `codeql` e `template-smoke` concluídos com sucesso no mesmo commit. O `ci` também avalia o SonarCloud Quality Gate em contextos confiáveis.
3. Empacota `RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg`.
4. Executa `TEMPLATE_PACKAGE=<caminho do .nupkg> bash scripts/templates/smoke-test.sh`, validando **o mesmo arquivo** da publicação.
5. Calcula SHA-256, publica release com notas e anexa o `.nupkg` e `SHA256SUMS.txt`.
6. Baixa os artefatos publicados, confere checksum e verifica a tag.

**Rascunhos e idempotência:** o workflow identifica releases ausentes, publicadas, em rascunho ou inconsistentes. Uma release **draft** gera falha explícita mesmo quando o pacote e o checksum já tiverem sido enviados: finalize ou remova o rascunho manualmente antes de executar novamente. A etapa de conferência exige `draft=false`, `prerelease=false` e `published_at` preenchido **antes de baixar os artefatos ou encerrar a #23**. A tag nunca é movida. A classificação possui testes de regressão em `scripts/releases/test-release-state.sh`, executados no CI. O PR **não fecha automaticamente** a #23; somente o workflow a encerra após verificar uma release realmente publicada.

## Conferência da publicação

```bash
gh release view v2.0.0 --repo rodri-oliveira-dev/web-api-core-seed
gh release download v2.0.0 --repo rodri-oliveira-dev/web-api-core-seed --dir ./release-check
cd release-check
sha256sum --check SHA256SUMS.txt
dotnet new install ./RodriOliveira.WebApiCoreSeed.Templates.2.0.0.nupkg
dotnet new webapi-seed -n SampleApi
```

## Informações do repositório (acesso de administrador)

O token padrão de GitHub Actions não consegue modificar metadados que exigem `Administration: write`. Atualize a seção **About** do GitHub após a publicação:

- **Descrição:** `Reusable .NET 10 ASP.NET Core Web API seed and dotnet new template — modular architecture, Identity, EF Core, SQL Server, Redis and OpenTelemetry.`
- **Website:** `https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.0`
- **Tópicos:** `dotnet`, `dotnet-10`, `aspnetcore`, `webapi`, `dotnet-template`, `clean-architecture`, `hexagonal-architecture`, `ef-core`, `redis`, `opentelemetry`, `docker`.
- Remova tópicos obsoletos como `netcore31`, `iprate` e `datasul`.

## Licença e procedência

O repositório ainda não possui arquivo `LICENSE`. Uma GitHub Release pública **não** concede automaticamente direitos de reutilização de código. O proprietário deve escolher a licença, verificar direitos sobre dependências/código de terceiros e só então anunciar como projeto open source licenciado. Não atribua licença sem essa decisão.

## Divulgação e próximos passos

Após confirmar publicação e checksum, divulgue [v2.0.0](https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.0) com links ao `README`, `CONTRIBUTING.md` e `SECURITY.md`. O NuGet.org não faz parte deste lançamento: o pacote está nos **assets da GitHub Release**. Aplique proteção de tags ou imutabilidade nas configurações do repositório, se disponível. Para correções posteriores, faça uma versão nova, sem editar a tag publicada.

## Pós-lançamento

Se algum teste ou artefato falhar, publique uma versão corretiva (por exemplo, `v2.0.1`), em vez de modificar a tag `v2.0.0`. Revise também a proteção de tags/imutabilidade de releases nas configurações do GitHub, quando disponível. A issue #23 só será encerrada pelo workflow **após** a publicação e a verificação dos artefatos.
