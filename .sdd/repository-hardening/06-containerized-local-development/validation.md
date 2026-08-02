# Validation

## Commands

```bash
dotnet restore WebApiCoreSeed.slnx
dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore
dotnet test WebApiCoreSeed.slnx --configuration Release --no-build
docker build --pull --tag web-api-core-seed:local .
docker compose --project-name web-api-core-seed-validation --env-file .env.local config
docker compose --project-name web-api-core-seed-validation --env-file .env.local build
docker compose --project-name web-api-core-seed-validation --env-file .env.local up -d
```

## Results

- `dotnet restore WebApiCoreSeed.slnx`: passed.
- `dotnet build WebApiCoreSeed.slnx --configuration Release --no-restore`: passed.
- `dotnet test WebApiCoreSeed.slnx --configuration Release --no-build`: passed, 53 unit/light tests and 42 integration/container tests.
- OpenAPI generator: passed and regenerated v1/v2 documents.
- `docker compose --env-file .env.local config`: passed; output was not recorded because it renders secrets.
- `docker build --pull --tag web-api-core-seed:local .`: passed.
- Image inspection: final user `app`, exposed port `8080/tcp`, entrypoint `dotnet WebApiCoreSeed.Api.dll`, size about 113 MB.
- SDK absence: `dotnet --list-sdks` in final image returned no SDKs.
- Source and local secret absence: `/src`, `/app/.git`, `/app/.env.local` and `/root/.microsoft/usersecrets` were absent.
- Docker history grep for password/JWT/secret/connection string patterns returned no matches.
- Compose build with project `web-api-core-seed-validation`: passed.
- First Compose up reached healthy SQL Server, healthy Redis and successful migrations, then API failed because host port `8080` was already allocated.
- Validation `.env.local` was changed to `API_HTTP_PORT=18080`; Compose up then passed.
- Migrations container exit code: `0`.
- HTTP smoke: `/health/live`, `/health/ready`, `/hc`, `/openapi/v1.json` and `/api/v1/Pratos?pageNumber=1&pageSize=10` returned `200`.
- Problem Details smoke: invalid pagination returned `400`; protected endpoint without token returned `401`.
- Persistence: SQL probe row and Redis probe key survived API restart and dependency restarts.
- Log grep for password/JWT/secret/connection string validation values returned no matches.
- User Secrets validation: isolated `APPDATA` confirmed the preserved `UserSecretsId` and both required keys.
- Cleanup: validation containers, network and volumes were removed.

## Limitations

- Host port `8080` was occupied in this environment; validation used `API_HTTP_PORT=18080`.
- API has no Dockerfile health check because the runtime image does not include an HTTP client and no extra client was installed.
