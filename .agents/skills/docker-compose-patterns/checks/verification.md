# Verification Runbook for Generated Compose Files

Run these checks against every generated `compose.yaml` before considering it complete.

## 1. Syntax and schema validation

Run the bundled script from the project root:

```bash
bash scripts/verify-compose.sh [--help]
```

Exit status is `0` when the Compose configuration is valid or help is requested, the non-zero status from `docker compose config --quiet` when validation fails, and `2` for invalid arguments.

To run the underlying validation directly:

```bash
docker compose config --quiet
```

This parses and validates the Compose file without printing the resolved configuration. It still resolves variables and reads service `env_file` files. If it exits non-zero, fix the reported errors before proceeding.

Plain `docker compose config` renders interpolated and `env_file` credentials. Do not capture that output in CI logs or agent transcripts. If inspecting the rendered configuration is necessary, use a separate copy with dummy credentials and review it locally. `--quiet` suppresses the configuration dump, not warnings or errors; diagnostics may still contain sensitive details.

## 2. Health check presence

Verify that every database, cache, or message broker service has a `healthcheck` defined. Review the Compose source files, including overrides, and confirm these services include `healthcheck.test`, `healthcheck.interval`, `healthcheck.timeout`, `healthcheck.retries`, and `healthcheck.start_period`. For rendered inspection, follow the dummy-credential precaution above.

Services that must have health checks:

- PostgreSQL, MySQL, MariaDB
- Redis, Memcached
- MongoDB
- RabbitMQ, Kafka
- Elasticsearch

## 3. Dependency ordering

For every service with `depends_on`:

- Confirm `condition: service_healthy` is set for infrastructure dependencies.
- Confirm the referenced service has a matching `healthcheck`.
- Confirm no circular dependencies exist (Compose will reject these, but verify intent).

## 4. Volume definitions

- Every volume referenced in a service's `volumes:` list that uses the `name:/path` format must have a corresponding entry in the top-level `volumes:` key.
- Database services must use named volumes, not bind mounts, for data directories.
- Bind mounts should appear only in development override files.

## 5. Environment variables and secrets

- No plaintext passwords, API keys, or tokens appear directly in `compose.yaml`.
- Sensitive values use `env_file:` or Docker secrets.
- If `.env` is referenced, confirm `.env` is in `.gitignore`.

## 6. Published ports and host mounts

- Published datastore ports bind to loopback unless remote host access is explicitly required.
- Services do not mount the Docker socket from `/var/run/docker.sock` or `/run/docker.sock`.

## 7. Image tags

- No service uses the `latest` tag or omits the tag entirely.
- All image references include an explicit version.

## 8. Runtime verification

After `docker compose up -d`:

```bash
# Check all services are running and healthy
docker compose ps

# Verify health check status specifically
docker inspect --format='{{.State.Health.Status}}' <container_name>

# Check logs for startup errors
docker compose logs --tail=50

# Verify inter-service connectivity
docker compose exec web ping -c 1 db
```

## 9. File naming

- The file is named `compose.yaml`, not `docker-compose.yml` or `docker-compose.yaml`.
- Development overrides are in `compose.override.yaml`.
