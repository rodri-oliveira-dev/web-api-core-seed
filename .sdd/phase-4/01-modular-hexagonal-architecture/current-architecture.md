# Current Architecture - Prompt 01

## Visao

A arquitetura ativa antes desta entrega era a arquitetura em camadas herdada:

```text
Restaurante.IO.Api -> Restaurante.IO.Data -> Restaurante.IO.Business
Restaurante.IO.Api -> Restaurante.IO.Business
```

## Responsabilidades misturadas

- `Restaurante.IO.Business` mistura entidades, validadores, interfaces de repositorio, interfaces de servico, notificacoes e services.
- `Restaurante.IO.Data` contem persistencia do dominio de restaurante, mas a API registra diretamente seus repositorios concretos.
- `Restaurante.IO.Api` contem entrada HTTP e composicao, mas tambem contem `ApplicationDbContext` e migrations de Identity.
- Controllers de `Pratos` e `Mesas` usam repositories para consultas e services para comandos.

## Riscos

- Regressao facil para controllers acoplados a persistencia.
- Dependencia de logging vazando para uma entidade de negocio.
- Limites de modulo dependem de convencao e documentacao, sem teste arquitetural.
- Migrations de Identity ainda nao pertencem a um adaptador de infraestrutura separado.
