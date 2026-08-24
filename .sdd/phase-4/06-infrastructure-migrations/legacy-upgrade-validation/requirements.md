# Requirements - Legacy Upgrade Validation

## Objetivo

Validar, com SQL Server real via Testcontainers, que um banco criado no estado legado do commit `6ce03d7f011c6809fbcbad47aa26d490f53ddf3d` e reconhecido pelas migrations atuais sem reaplicar migrations historicas, preservando dados e aplicando apenas migrations posteriores.

## Escopo

- Criar baseline SQL versionado derivado das migrations legadas do commit informado.
- Registrar as migrations historicas em `dbo.__EFMigrationsHistory`.
- Inserir dados representativos de Identity e SampleRestaurant antes do upgrade.
- Executar `Database.MigrateAsync` nos contextos atuais.
- Confirmar que a migration nova de paginacao `20260801191447_AddPratosPaginationOrderingIndex` foi aplicada.
- Confirmar que os dados legados seguem consultaveis pelo modelo atual.
- Validar tabelas, indices e constraints essenciais apos o upgrade.
- Executar o upgrade duas vezes no mesmo banco para confirmar idempotencia.

## Fora de Escopo

- Recriar ou renumerar migrations historicas.
- Alterar schema produtivo para facilitar teste.
- Usar SQLite, EF InMemory, banco local ou banco compartilhado.
- Gerar baseline a partir das migrations atuais.
- Introduzir seed runtime ou mecanismo de seed de desenvolvimento.
- Fechar a issue antes de aprovacao em CI.

## Criterios de Aceite

- O baseline legado tem origem e hash documentados.
- `dbo.__EFMigrationsHistory` contem os IDs historicos antes do upgrade.
- A migracao atual aplica somente `20260801191447_AddPratosPaginationOrderingIndex`.
- Nenhuma migration historica e reaplicada.
- Dados de usuario, prato, mesa, atendente, pedido e item de pedido sobrevivem ao upgrade.
- O `SampleRestaurantDbContext` atual consulta dados legados e persiste novo dado apos o upgrade.
- O indice `IX_Pratos_Titulo_Id` existe apos o upgrade e permanece apos uma segunda execucao.
- Constraints de FK e indices legados relevantes existem apos o upgrade.
