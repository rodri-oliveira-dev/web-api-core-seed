# Compose Design

## Services

- `sqlserver`: `mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`.
- `redis`: `redis:7.4.2-alpine`.
- `migrations`: build target `migrations`.
- `api`: build target `final`.

## Dependencies

- `migrations` waits for `sqlserver` with `condition: service_healthy`.
- `api` waits for healthy `sqlserver`, healthy `redis`, and successful `migrations`.

## Ports

- API: `${API_HTTP_PORT:-8080}:8080`.
- SQL Server: `${SQLSERVER_HOST_PORT:-1433}:1433`.
- Redis: `${REDIS_HOST_PORT:-7001}:6379`.

SQL Server and Redis are published to support host `dotnet run`.

## Volumes

- `sqlserver-data`.
- `redis-data`.

## Secrets

`SQLSERVER_SA_PASSWORD` and `JWT_SECRET` are required by interpolation. No defaults are provided for secrets.

## API Health

Compose controls dependency health for SQL Server and Redis. API HTTP health is validated through external smoke commands because the final image remains minimal.
