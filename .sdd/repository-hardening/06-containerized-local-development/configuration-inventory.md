# Configuration Inventory

| Key | Current file | Type | Sensitive | Versioned value before | Host source | Compose source | Required | Missing behavior |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | Connection string | Yes | SQL Server local string with password placeholder | User Secrets | `.env.local` rendered to env var | Yes | API throws clear configuration error |
| `AppSettings:Secret` | `appsettings.json` | JWT signing secret | Yes | JWT local placeholder | User Secrets | `.env.local` rendered to env var | Yes | API throws clear configuration error |
| `AppSettings:ExpiracaoHoras` | `appsettings.json` | Number | No | `2` | `appsettings.json` | Compose env value | Yes | JWT expiration may be invalid/default |
| `AppSettings:Emissor` | `appsettings.json` | String | No | `WebApiCoreSeed` | `appsettings.json` | Compose env value | Yes | Token validation fails |
| `AppSettings:ValidoEm` | `appsettings.json` | URL/audience | No | `https://localhost` | `appsettings.json` or User Secrets | Compose env value | Yes | Token validation fails |
| `RedisCacheSettings:Enabled` | `appsettings.json` | Boolean | No | `true` | `appsettings.json` | Compose env value | Yes | Redis cache disabled when false |
| `RedisCacheSettings:ConnectionString` | `appsettings.json` | Endpoint | No, unless password is added | `localhost:7001` | `appsettings.json` | Compose env value `redis:6379` | Required when Redis enabled | Redis registration or health fails |
| `OpenTelemetry:Enabled` | `appsettings.json` | Boolean | No | `true` | `appsettings.json` | `appsettings.json` | No | Telemetry disabled when false |
| `OpenTelemetry:Otlp:Endpoint` | `appsettings.json` | URL | No | Empty | appsettings/env | appsettings/env | No | No OTLP export |
| `SeqSettings:Enabled` | `appsettings.json` | Boolean | No | `false` | appsettings/env | Compose env value `false` | No | Seq sink disabled |
| `SeqSettings:Url` | `appsettings.json` | URL | No | `http://localhost:5341` | appsettings/env | appsettings/env | No | Seq health/sink unused when disabled |
| `Cors:AllowedOrigins` | `appsettings.Development.json` | String array | No | localhost origins | appsettings/env | appsettings/env | No | CORS denies all origins |

## Notes

- Host mode relies on User Secrets for the full SQL connection string because SQL passwords must not be assembled from tracked JSON.
- Compose mode uses service DNS names and interpolates secrets from `.env.local` or host environment variables.
