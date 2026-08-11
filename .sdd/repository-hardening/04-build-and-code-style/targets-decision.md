# Directory.Build.targets Decision

## Decisao

Nao criar `Directory.Build.targets` neste prompt.

## Evidencia

A discovery encontrou propriedades comuns e convencoes de analyzers/style, mas nao encontrou necessidade real de target tardio.

Nao houve demanda por:

- validacao de artefato gerado apos build;
- convencao impossivel de expressar via SDK/analyzers;
- integracao pos-build com ferramenta externa;
- verificacao deterministica pequena que precise rodar depois de cada projeto.

## Justificativa

`Directory.Build.props` cobre os defaults de MSBuild. `.editorconfig` e analyzers nativos cobrem estilo e diagnosticos. Criar `Directory.Build.targets` apenas para existir adicionaria superficie de manutencao sem comportamento testavel.
