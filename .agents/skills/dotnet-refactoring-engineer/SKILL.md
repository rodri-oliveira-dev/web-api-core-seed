---
name: dotnet-refactoring-engineer
description: Use esta skill para revisar, refatorar ou melhorar codigo .NET/C# com foco em qualidade, legibilidade, manutencao, testabilidade, seguranca e performance. Nao use para reescrever codigo por preferencia estetica sem ganho tecnico claro.
---

# Objetivo

Apoiar refatoracoes seguras em C# preservando comportamento observavel e respeitando a etapa da modernizacao em andamento.

# Principios

1. Entenda o comportamento atual antes de alterar.
2. Identifique o problema real.
3. Prefira mudancas pequenas e verificaveis.
4. Nao introduza abstracoes sem necessidade concreta.
5. Preserve contratos publicos, rotas, payloads, codigos HTTP e comportamento externo salvo pedido explicito.
6. Nao misture refatoracao estrutural com mudanca funcional sem criterio registrado.

# Quando usar

- Refatoracao de Controllers, services, validators, repositories ou configuracao.
- Revisao de responsabilidades, acoplamento, coesao e testabilidade.
- Reducao de duplicacao real.
- Preparacao ou revisao de migracao tecnica.
- Revisao de APIs ASP.NET Core, Entity Framework Core, injecao de dependencia e testes automatizados.

# Quando nao usar

- Mudancas apenas documentais.
- Atualizacao de framework ou pacote sem prompt especifico.
- Aplicacao cerimonial de padroes sem beneficio observavel.

# Checklist

- O comportamento atual foi entendido?
- O menor ajuste seguro foi escolhido?
- Contratos externos foram preservados?
- Dependencias e lifetimes continuam coerentes?
- Regras de negocio ficaram fora de Controllers?
- Persistencia e framework nao vazaram para regras de dominio quando evitavel?
- Testes relevantes foram preservados ou ampliados?
- O diff esta limitado ao escopo?

# Validacao

Procure comandos existentes antes de assumir. Quando nao houver instrucao especifica:

```bash
dotnet restore RestauranteAPI.sln
dotnet build RestauranteAPI.sln --no-restore
dotnet test test/Pedidos.Test/Pedidos.Test.csproj --no-build
```

Registre validacoes bloqueadas pelo ambiente sem alterar o projeto para contornar o bloqueio.
