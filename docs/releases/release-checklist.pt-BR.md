# Operação da release - v2.0.1

[Notas da versão](v2.0.1.md) | [README](../../README.pt-BR.md) | [English](release-checklist.md)

## Fluxo de publicação

O workflow existente `.github/workflows/release-v2.yml` executa em pushes para a `main` e permite reexecução manual a partir da `main`. Não publica a partir de pull requests. A versão **2.0.1** será distribuída como asset de uma **GitHub Release**; o NuGet.org não está configurado.

1. Incorporar o PR após aprovação de `ci`, `codeql` e `template-smoke`.
2. Aguardar os mesmos três checks, com sucesso no SHA exato que chegou à `main`. Falhas, checks ignorados ou obsoletos bloqueiam a publicação.
3. Verificar se não há tag `v2.0.1` conflitante nem release em draft/inconsistente. Preservar `v2.0.0` sem alterações.
4. Empacotar o template `2.0.1`, conferir a versão e validar no `.nupkg` o `global.json` (`10.0.403`) e o `Dockerfile` (`sdk:10.0.403`, `aspnet:10.0.14`).
5. Instalar e executar o smoke test do **mesmo arquivo `.nupkg`** que será publicado, incluindo build, testes e a rota de saúde da API gerada.
6. Publicar a release `v2.0.1` com `docs/releases/v2.0.1.md`, o pacote verificado e `SHA256SUMS.txt`.
7. Baixar os assets, conferir o SHA-256 e verificar se a tag aponta para o commit efetivamente testado.

**Idempotência:** uma release `v2.0.1` já publicada apenas é verificada; drafts, estados inconsistentes e tags conflitantes causam falha explícita. A publicação corretiva não executa mais o fechamento da issue histórica #23 nem precisa de permissão de escrita em issues.

## Conferência após publicação

```bash
gh release view v2.0.1 --repo rodri-oliveira-dev/web-api-core-seed
gh release download v2.0.1 --repo rodri-oliveira-dev/web-api-core-seed --dir ./release-check
cd release-check
sha256sum --check SHA256SUMS.txt
dotnet new install ./RodriOliveira.WebApiCoreSeed.Templates.2.0.1.nupkg
dotnet new webapi-seed -n SampleApi
```

Essa publicação atualiza a **configuração distribuída para projetos novos**, mas não corrige automaticamente aplicações existentes. Os ambientes precisam do runtime .NET atualizado e os contêineres devem ser reconstruídos e implantados novamente. Consulte o [comunicado da Microsoft](https://devblogs.microsoft.com/dotnet/dotnet-october-2026-oob-security-updates/).

## Distribuição e licenciamento

Os assets da GitHub Release continuam sendo o único canal configurado. Ainda não existe um arquivo `LICENSE`; a distribuição pública não concede automaticamente direitos de reutilização. O responsável deve revisar a licença e a procedência do código antes de anunciá-lo como open source licenciado.

Na seção About do repositório, utilize o link `https://github.com/rodri-oliveira-dev/web-api-core-seed/releases/tag/v2.0.1` após a publicação. Se o workflow falhar, investigue a causa sem mover tags; publique correções subsequentes em uma nova versão.
