# Common Compose Mistakes: Before and After

## 1. Missing health checks on database dependencies

### Bad

```yaml
services:
  web:
    build: .
    depends_on:
      - db
  db:
    image: postgres:17
```

`depends_on` without a condition only waits for the container to start, not for Postgres to accept connections. The web service will crash on startup.

### Good

```yaml
services:
  web:
    build: .
    depends_on:
      db:
        condition: service_healthy
  db:
    image: postgres:17
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      timeout: 3s
      retries: 3
      start_period: 10s
```

---

## 2. Using `latest` tag

### Bad

```yaml
services:
  cache:
    image: redis:latest
```

`latest` is mutable. Builds become non-reproducible and can break without warning.

### Good

```yaml
services:
  cache:
    image: redis:7
```

Pin to a specific major or minor version.

---

## 3. Hardcoded secrets in compose.yaml

### Bad

```yaml
services:
  db:
    image: postgres:17
    environment:
      POSTGRES_PASSWORD: supersecretpassword123
```

Secrets in the Compose file end up in version control.

### Good

```yaml
services:
  db:
    image: postgres:17
    env_file:
      - .env
```

With `.env` containing `POSTGRES_PASSWORD=supersecretpassword123` and `.env` listed in `.gitignore`.

---

## 4. Bind mount for database data

### Bad

```yaml
services:
  db:
    image: postgres:17
    volumes:
      - ./pgdata:/var/lib/postgresql/data
```

Bind mounts for database storage cause permission issues and poor I/O performance on macOS and Windows.

### Good

```yaml
services:
  db:
    image: postgres:17
    volumes:
      - db-data:/var/lib/postgresql/data

volumes:
  db-data:
```

Named volumes are managed by Docker and perform correctly on all platforms.

---

## 5. Legacy filename

### Bad

```
docker-compose.yml
```

### Good

```
compose.yaml
```

`compose.yaml` is the canonical filename. `docker-compose.yml` is legacy.

---

## 6. Development settings in the base Compose file

### Bad

A single `compose.yaml` with bind mounts, debug ports, and development environment variables mixed in with production settings.

### Good

Base `compose.yaml` with production-appropriate defaults. Development-only settings in `compose.override.yaml`, which Compose loads automatically:

```yaml
# compose.override.yaml
services:
  web:
    ports:
      - "9229:9229"
    environment:
      LOG_LEVEL: debug
    develop:
      watch:
        - action: sync
          path: ./src
          target: /app/src
```

---

## 7. No restart policy

### Bad

```yaml
services:
  web:
    image: myapp:1.0.0
```

Without a restart policy, the container stays down after a crash or host reboot.

### Good

```yaml
services:
  web:
    image: myapp:1.0.0
    restart: unless-stopped
```
