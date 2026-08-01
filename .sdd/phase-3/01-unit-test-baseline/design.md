# Design - 01 Unit Test Baseline

## Convencoes

- Usar nomes de metodo no formato `MetodoQuandoCondicaoDeveResultado`.
- Manter `DisplayName` em portugues, descrevendo comportamento observavel.
- Preservar AAA (`Arrange`, `Act`, `Assert`) quando ja existir.
- Builders locais so devem existir quando reduzem duplicacao real no proprio arquivo de teste.

## Mocks

- Mockar repositorios e portas externas nos testes de service.
- Nao mockar entidades, Value Objects ou validators.
- Usar `Notificador` real quando o comportamento esperado for notificacao.
- Verificar chamadas de repositorio somente quando isso representar efeito observavel do service.
- Verificar ausencia de chamada de persistencia quando a validacao falha.

## Dados de teste

- Usar dados fixos e intencionais.
- Nao usar `DateTime.Now` para preencher campos sem relacao com tempo.
- Evitar GUID aleatorio nos testes unitarios quando um GUID fixo torna o caso mais diagnostico.
- Valores extremos devem ter nome claro, como `new string('a', 1001)`.

## Validators

- Testar propriedades diretamente com `FluentValidation.TestHelper`.
- Separar cenario de campo obrigatorio de cenario de limite quando uma propriedade independente estiver sendo avaliada.
- Para validacao positiva, verificar ausencia de erro nos campos protegidos, nao apenas `IsValid`.

## Services

- Testar retorno booleano e efeito colateral de repositorio.
- Testar que validacao invalida adiciona notificacoes reais e nao chama repositorio.
- Testar `Adicionar` e `Atualizar` separadamente.
- Testar `Remover` quando o service expuser apenas delegacao ao repositorio.

## Excecoes

- Nenhum teste unitario atual cobre excecoes.
- Nao adicionar testes de excecao sem regra produtiva clara nesta entrega.

## Categorias

- `Unitarios/Validators`: testes unitarios puros.
- `Unitarios/Services`: testes unitarios com mocks de repositorio.
- `Unitarios/Notificacoes`: testes unitarios de componente de dominio/infra leve.
- `Integracao`: testes HTTP existentes da Fase 2; auditados, mas nao reescritos neste prompt.

## Mudanca produtiva permitida

Se confirmado por teste, corrigir somente a regra pequena de `PedidoPratoValidation.Observacao` para permitir texto opcional ate 1000 caracteres e rejeitar texto acima do limite. Registrar a decisao e cobrir o comportamento com teste unitario.
