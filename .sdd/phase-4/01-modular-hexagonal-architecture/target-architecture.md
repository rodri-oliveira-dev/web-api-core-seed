# Target Architecture - Prompt 01

## Estrutura alvo pragmatica

Nesta entrega, a arquitetura alvo usa um unico modulo inicial de negocio:

```text
src/
├── DevIO.Api/                         # input adapters + composition root
├── DevIO.Business/
│   └── Modules/
│       └── Restaurant/
│           ├── Domain/                # entities, value objects, enums, validations
│           └── Application/           # use cases, input ports, output ports, notifications
└── DevIO.Data/
    └── Modules/
        └── Restaurant/
            └── Infrastructure/        # EF Core context, mappings, repositories
```

Os nomes de assembly e namespaces publicos legados sao preservados para reduzir risco de contrato interno nesta primeira entrega.

## Regras

- Domain nao depende de API, Data, ASP.NET Core, EF Core, Redis, Identity ou logging.
- Application depende de Domain e expoe portas de entrada usadas pelos controllers.
- Infrastructure implementa as portas de saida existentes no nucleo.
- API compoe os adaptadores e expoe os contratos HTTP existentes.
- Controllers de dominio usam services/casos de uso para leitura e escrita.

## Adiado

- Extrair contratos publicos de modulo para assembly dedicado.
- Separar definitivamente dominio de exemplo de componentes reutilizaveis.
- Substituir repositorio generico por portas orientadas ao dominio.
- Tornar Unit of Work explicito.
- Propagar `CancellationToken`.
- Mover migrations de Identity para infraestrutura.
- Corrigir paginacao para ser deterministica e limitada.
