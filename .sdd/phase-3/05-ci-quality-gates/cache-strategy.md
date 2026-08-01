# Cache Strategy - 05 CI Quality Gates

## NuGet

- Armazenado: `~/.nuget/packages`.
- Chave: `nuget-${{ runner.os }}-${{ hashFiles('global.json', 'Directory.Build.props', '**/*.csproj') }}`.
- Invalidation: alteracoes no SDK, propriedades comuns ou manifests de projeto invalidam a chave principal.
- Fallback: `nuget-${{ runner.os }}-`.
- Risco: sem `packages.lock.json`, a cache melhora tempo de restore mas nao garante lock deterministico de dependencias transitivas.
- Mitigacao: `dotnet restore` sempre roda explicitamente; `bin`, `obj`, Testcontainers e outputs de teste nao sao cacheados.

## Containers

- Armazenado: nenhum cache de imagem/container.
- Chave: nao aplicavel.
- Invalidation: o runner baixa imagens quando necessario.
- Risco: maior tempo de execucao.
- Fallback: Testcontainers emite logs de pull/startup e falha o teste se Docker ou imagem nao estiverem disponiveis.

## Build Outputs

- Armazenado: nenhum `bin`/`obj`.
- Motivo: evitar estado stale em build/test e preservar reprodutibilidade do gate.
