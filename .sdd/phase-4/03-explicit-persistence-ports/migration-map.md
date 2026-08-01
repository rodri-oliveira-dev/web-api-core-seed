# Migration Map - Prompt 03

| Fatia | Antes | Depois | Observacao |
| --- | --- | --- | --- |
| Prato | Porta e implementacao herdavam o repositorio generico legado | `IPratoRepository` explicito + `PratoRepository` direto em `SampleRestaurantDbContext` | Mantem consulta por id, pagina e contagem |
| Mesa | Porta e implementacao herdavam o repositorio generico legado | `IMesaRepository` explicito + `MesaRepository` direto em `SampleRestaurantDbContext` | Mantem consulta por id |
| Atendente | Porta e implementacao herdavam o repositorio generico legado | `IAtendenteRepository` explicito + `AtendenteRepository` direto em `SampleRestaurantDbContext` | Mantem comandos usados pelo service |
| Pedido | Porta e implementacao herdavam o repositorio generico legado | `IPedidoRepository` explicito + `PedidoRepository` direto em `SampleRestaurantDbContext` | Remove `ObterPedidoItens` sem consumidor |
| PedidoPrato | Porta e implementacao herdavam o repositorio generico legado | `IPedidoPratoRepository` explicito + `PedidoPratoRepository` direto em `SampleRestaurantDbContext` | Mantem comandos usados pelo service |
| Loggin | Porta e implementacao herdavam o repositorio generico legado | `ILogginRepository` explicito + `LogginRepository` direto em `SampleRestaurantDbContext` | Mantem inclusao de log |
| Testes | Fakes herdavam o repositorio generico legado | Fakes implementam portas explicitas | Mantem injecao de falhas de persistencia |
