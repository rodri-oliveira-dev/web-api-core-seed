# Requirements - Prompt 02

## Objetivo

Separar de forma inequivoca o dominio demonstrativo de restaurante dos componentes reutilizaveis do seed `web-api-core-seed`, preservando contratos HTTP e a arquitetura modular introduzida no Prompt 01.

## Escopo

- Renomear solution, projetos, assemblies e namespaces ativos que ainda usam nomes legados ou de negocio sem contexto.
- Tornar o modulo de exemplo explicitamente `SampleRestaurant`.
- Renomear o DbContext de dominio de `SampleRestaurantDbContext` para um nome tecnico e contextualizado.
- Atualizar composition root, testes, ferramenta de OpenAPI, workspace, workflows e documentacao operacional afetada.
- Manter migrations antigas no local atual, aplicando somente ajustes de namespace/tipo inevitaveis.
- Documentar o mapa reusable vs sample e o inventario de nomes.

## Fora de escopo

- Empacotar como `dotnet new`.
- Criar parametros de template.
- Publicar pacote.
- Substituir repositorios.
- Alterar Unit of Work.
- Mover migrations para outra infraestrutura.
- Reescrever a documentacao publica completa.

## Criterios de aceite

- Componentes reutilizaveis usam nomes neutros baseados em `WebApiCoreSeed`.
- Componentes especificos do exemplo usam `SampleRestaurant`.
- `Restaurante`, `Datasul` e `MeuDbContext` nao aparecem em codigo ativo, configuracao ativa, testes ativos, workflows ou tooling ativo.
- Ocorrencias historicas permanecem apenas em `LEGACY.md` e SDD antigo quando claramente contextuais.
- Rotas e payloads HTTP permanecem preservados.
- Testes arquiteturais continuam ativos e cobrem a separacao.
- Restore, build, testes, smoke/regressao e geracao OpenAPI sao validados.
