# Service Dependencies and Startup Ordering

## depends_on conditions

The `depends_on` key supports three conditions:

| Condition | Meaning |
|---|---|
| `condition: service_started` | Waits only for the container to start (default if no condition specified). |
| `condition: service_healthy` | Waits for the container's health check to pass. |
| `condition: service_completed_successfully` | Waits for the container to run and exit with code 0. Useful for init/migration containers. |

Always use `condition: service_healthy` for infrastructure services (databases, caches, message brokers). The `service_started` condition is insufficient because a container can be running before the process inside is accepting connections.

## Health check patterns for common services

### PostgreSQL

```yaml
healthcheck:
  test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-postgres}"]
  interval: 5s
  timeout: 3s
  retries: 3
  start_period: 10s
```

### MySQL / MariaDB

```yaml
healthcheck:
  test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
  interval: 5s
  timeout: 3s
  retries: 3
  start_period: 20s
```

MySQL can take longer to initialize on first run. Use a `start_period` of 20s or more.

### Redis

```yaml
healthcheck:
  test: ["CMD", "redis-cli", "ping"]
  interval: 5s
  timeout: 3s
  retries: 3
  start_period: 5s
```

### MongoDB

```yaml
healthcheck:
  test: ["CMD", "mongosh", "--eval", "db.adminCommand('ping')"]
  interval: 5s
  timeout: 3s
  retries: 3
  start_period: 10s
```

### RabbitMQ

```yaml
healthcheck:
  test: ["CMD", "rabbitmq-diagnostics", "check_running"]
  interval: 10s
  timeout: 5s
  retries: 3
  start_period: 30s
```

RabbitMQ has a longer startup time. Set `start_period` to at least 30s.

### Elasticsearch

```yaml
healthcheck:
  test: ["CMD-SHELL", "curl -fs http://localhost:9200/_cluster/health || exit 1"]
  interval: 10s
  timeout: 5s
  retries: 5
  start_period: 30s
```

## Init containers pattern

Use `service_completed_successfully` for one-shot tasks like database migrations:

```yaml
services:
  migrate:
    image: myapp:1.2.0
    command: ["./manage.py", "migrate"]
    depends_on:
      db:
        condition: service_healthy

  web:
    image: myapp:1.2.0
    depends_on:
      db:
        condition: service_healthy
      migrate:
        condition: service_completed_successfully
```

This ensures migrations complete before the web service starts.

## Restart behavior and dependencies

`depends_on` only governs initial startup ordering. It does not re-trigger if a dependency restarts. For runtime resilience:

- Configure application-level retry/reconnect logic.
- Use `restart: unless-stopped` so services recover from transient failures.
- Do not chain long dependency trees. Keep the graph shallow — deep chains increase total startup time and fragility.
