# Volumes and Networks

## Volume types

### Named volumes

Use named volumes for data that must persist across container recreations:

```yaml
services:
  db:
    image: postgres:17
    volumes:
      - db-data:/var/lib/postgresql/data

volumes:
  db-data:
```

Named volumes are managed by Docker. They survive `docker compose down` (but not `docker compose down -v`, which deletes them and their data irreversibly). Never run `down -v` to work around a startup or connectivity problem — get explicit user confirmation first.

### Bind mounts

Use bind mounts to sync host directories into containers. Appropriate for development-time source code mounting only:

```yaml
services:
  web:
    build: .
    volumes:
      - ./src:/app/src
```

Do not use bind mounts for database data — they cause permission issues and poor performance on macOS and Windows.

### Anonymous volumes

Avoid anonymous volumes (volumes with no name and no host path). They are hard to track and clean up. Always use named volumes.

### tmpfs mounts

Use `tmpfs` for ephemeral scratch data that should not persist:

```yaml
services:
  app:
    image: myapp:1.0.0
    tmpfs:
      - /tmp
      - /app/cache
```

## Volume mount flags

- Use `:ro` to mount volumes as read-only when the container should not write to them.
- Use `:cached` or `:delegated` on macOS only when performance requires it and data consistency tradeoffs are acceptable. Prefer Compose Watch (`develop.watch`) over bind mounts with performance flags.

## Volume patterns

### Excluding node_modules from bind mounts

When bind-mounting a Node.js project, exclude `node_modules` with an anonymous volume to prevent host dependencies from overwriting container dependencies:

```yaml
services:
  web:
    build: .
    volumes:
      - ./:/app
      - /app/node_modules
```

This mounts the project root but keeps the container's own `node_modules` intact.

### Sharing data between services

Use a named volume to share files between services:

```yaml
services:
  generator:
    image: myapp:1.0.0
    volumes:
      - shared-data:/output

  consumer:
    image: nginx:1.27
    volumes:
      - shared-data:/usr/share/nginx/html:ro

volumes:
  shared-data:
```

## Networks

### Default network

Compose creates a default network for each project. All services join it automatically. Services can reach each other by service name as the hostname. For most single-application stacks, the default network is sufficient.

### Custom networks for isolation

Use custom networks when you need to isolate groups of services:

```yaml
services:
  web:
    image: myapp:1.0.0
    networks:
      - frontend
      - backend

  db:
    image: postgres:17
    networks:
      - backend

  proxy:
    image: nginx:1.27
    networks:
      - frontend

networks:
  frontend:
  backend:
```

In this example, `proxy` cannot reach `db` directly because they share no network. `web` bridges both.

### External networks

Use `external: true` to reference a network created outside this Compose file:

```yaml
networks:
  shared:
    external: true
    name: my-shared-network
```

This is useful when multiple Compose projects need to communicate.

### Network aliases

Use aliases to give a service additional hostnames on a specific network:

```yaml
services:
  db:
    image: postgres:17
    networks:
      backend:
        aliases:
          - database
          - postgres
```

Other services on the `backend` network can reach this service as `db`, `database`, or `postgres`.
