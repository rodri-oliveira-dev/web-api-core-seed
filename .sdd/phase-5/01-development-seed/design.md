# Design - Development Seed

## Interface

Interface escolhida:

```bash
dotnet run --project src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj -- --seed
```

Justificativa:

- Usa o projeto da API como composition root.
- Reaproveita configuracao, DI, Identity e DbContexts reais.
- Mantem o comando explicito e facil de documentar.
- Evita controller, endpoint administrativo ou seed automatico.

O comando deve:

1. Construir o host de servicos.
2. Bloquear `Production`.
3. Validar configuracao obrigatoria.
4. Aplicar migrations de `ApplicationDbContext`.
5. Aplicar migrations de `SampleRestaurantDbContext`.
6. Executar seed de Identity.
7. Executar seed de SampleRestaurant.
8. Encerrar o processo sem iniciar listener HTTP.

## Separacao

- `Program.cs`: somente roteia `--seed` para um runner dedicado.
- Definicao dos dados: classes imutaveis/records em namespace de seed.
- Orquestracao: servico `DevelopmentSeedRunner`.
- Persistencia Identity: servico usando `UserManager<IdentityUser>`.
- Persistencia SampleRestaurant: servico usando `SampleRestaurantDbContext` e um unico `SaveChangesAsync` como fronteira atomica local.

## Idempotencia

- Identity usa email normalizado pelo `UserManager` como chave natural.
- Claims sao comparadas por tipo e valor.
- SampleRestaurant usa GUIDs deterministicas por entidade de seed.
- Reexecucao atualiza campos gerenciados pelo seed quando o ID/chave gerenciada ja existe.
- O seed nao remove dados fora do inventario versionado.

## Cancelamento

- `CancellationToken` vem de `Console.CancelKeyPress`/host lifetime quando disponivel.
- Operacoes EF Core recebem token.
- Operacoes do `UserManager` nao aceitam token diretamente; o runner checa cancelamento antes e depois das chamadas relevantes.

## Testes

- Unitarios/leves: definicao de opcoes, bloqueio de producao, credencial ausente e idempotencia de definicoes puras quando viavel.
- Integracao: SQL Server real via Testcontainers, migrations, primeira execucao, segunda execucao, dados parciais, atualizacao segura, preservacao de dados do usuario, login e endpoint protegido.

## Docker Compose

Adicionar um servico one-shot opcional `seed` que executa o mesmo comando no container publicado, dependente de `sqlserver` healthy e de `migrations` concluido. O comando tambem aplica migrations para suportar host mode; no Compose, isso e idempotente.
