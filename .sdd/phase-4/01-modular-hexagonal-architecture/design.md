# Design - Prompt 01

## Escolha

Adotar um desenho modular pragmatico sem trocar nomes de assemblies publicos:

- `Restaurante.IO.Business` continua sendo o nucleo compilado, mas ganha estrutura fisica de `Modules/Restaurant/Domain` e `Modules/Restaurant/Application`.
- `Restaurante.IO.Data` continua sendo infraestrutura compilada, mas ganha estrutura fisica de `Modules/Restaurant/Infrastructure`.
- `Restaurante.IO.Api` continua sendo API, adaptador de entrada e composition root.

## Mudancas de codigo planejadas

- Mover arquivos do nucleo de negocio para a estrutura fisica do modulo `Restaurant`.
- Mover context, mappings e repositories de dominio para a estrutura fisica de infraestrutura do modulo `Restaurant`.
- Substituir `Microsoft.Extensions.Logging.LogLevel` em `LogginEntity` por um enum de dominio com os mesmos valores numericos.
- Remover `Microsoft.Extensions.Logging.Abstractions` do projeto Business se nao houver outro uso.
- Adicionar metodos de leitura nas portas de entrada `IPratoService` e `IMesaService`.
- Ajustar `PratosController` e `MesasController` para dependerem apenas dos services/casos de uso do modulo para consultas de dominio.
- Adicionar testes arquiteturais simples por reflexao no projeto `Pedidos.Test`.

## Preservacao de comportamento

- Rotas HTTP permanecem iguais.
- View models permanecem na API.
- Repositorios e EF Core preservam assinaturas e comportamento.
- Migrations nao sao alteradas.
- Paginacao atual e calculo de total permanecem iguais.
- Autenticacao, autorizacao, rate limiting, cache, health checks e Problem Details permanecem iguais.
