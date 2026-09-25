# Layer Caching

Docker builds are incremental. Each instruction creates a layer, and Docker reuses cached layers when the inputs have not changed. Understanding cache invalidation rules is critical for fast builds.

## Cache invalidation rules

1. A layer's cache is invalidated when the instruction itself changes or any of its inputs change.
2. When a layer is invalidated, all subsequent layers are also invalidated.
3. For `COPY` and `ADD`, Docker computes a checksum of the files being copied. If the checksum differs from the cached layer, the cache is invalidated.
4. For `RUN`, the cache key is the command string. The cache does not detect changes to files fetched over the network — use cache mounts or explicit version pinning to manage this.

## Layer ordering strategy

Order instructions from least-frequently-changed to most-frequently-changed:

```
1. Base image (FROM)
2. System package installation
3. Dependency manifest copy (package.json, go.mod, requirements.txt)
4. Dependency installation (npm ci, go mod download, pip install)
5. Application source code copy
6. Application build
7. Runtime configuration (USER, EXPOSE, ENTRYPOINT)
```

The key insight: dependency manifests change far less often than application source code. By copying and installing dependencies before copying the source, you cache the expensive dependency installation step across most builds.

### Bad ordering

```dockerfile
# Invalidates dependency cache on every source change
COPY . .
RUN npm ci
RUN npm run build
```

### Good ordering

```dockerfile
# Dependencies cached until package.json or lock file changes
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build
```

## BuildKit cache mounts

Cache mounts persist package manager caches across builds without embedding them in the image layer. They are the single most impactful optimization for dependency installation speed.

### Syntax

```dockerfile
RUN --mount=type=cache,target=<path> <command>
```

### Common cache mount targets

| Package manager | Cache mount target |
|---|---|
| Go modules | `/go/pkg/mod` |
| Go build cache | `/root/.cache/go-build` |
| npm | `/root/.npm` |
| yarn | `/usr/local/share/.cache/yarn` |
| pnpm | `/root/.local/share/pnpm/store` |
| pip | `/root/.cache/pip` |
| Maven | `/root/.m2` |
| Gradle | `/root/.gradle` |
| apt | `/var/cache/apt` |

### Cache mount with a non-root build user

When the build stage runs as a non-root user, specify `uid` and `gid`:

```dockerfile
RUN --mount=type=cache,target=/home/appuser/.cache/pip,uid=1001,gid=1001 \
    pip install -r requirements.txt
```

### Bind mounts for source

Use bind mounts to avoid copying source files into the build layer when the source is only needed for compilation, not for the final artifact:

```dockerfile
RUN --mount=type=bind,source=.,target=/src \
    --mount=type=cache,target=/go/pkg/mod \
    --mount=type=cache,target=/root/.cache/go-build \
    cd /src && go build -o /app/server ./cmd/server
```

This keeps the build context out of the layer history entirely.

## COPY --link

`COPY --link` creates a layer that is independent of all previous layers. This means:

- Changing a previous layer does not invalidate a `--link` copy.
- It allows Docker to parallelize layer creation.
- It is particularly useful when copying from a build stage into the runtime stage.

Use `COPY --link` when:

- Copying the final artifact from a build stage: `COPY --from=build --link /app/server .`
- Adding static config files that do not depend on prior layer content.

Do not use `COPY --link` when:

- The `COPY` depends on a directory structure created by a prior `RUN` instruction (the `--link` layer cannot see it).

## Reducing layer count

Combine related commands in a single `RUN` to avoid intermediate layers:

```dockerfile
# Good: single layer for system packages
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
      ca-certificates \
      curl && \
    rm -rf /var/lib/apt/lists/*

# Bad: three layers, apt cache persists in first layer
RUN apt-get update
RUN apt-get install -y curl
RUN rm -rf /var/lib/apt/lists/*
```

But do not over-combine. Keep dependency installation and application build in separate `RUN` instructions so that the dependency layer caches independently.

## Debugging cache behavior

Use `docker build --progress=plain` to see which steps are cached (`CACHED`) and which are re-executed. Use `docker history <image>` to inspect layer sizes and identify unexpectedly large layers.
