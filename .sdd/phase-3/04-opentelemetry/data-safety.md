# Data Safety - 04 OpenTelemetry

## Prohibited In Spans

- Authorization header values.
- Cookies or session ids.
- JWTs and access tokens.
- Passwords, client secrets and API keys.
- Full connection strings.
- SQL statements and SQL parameter values added by custom enrichment.
- Raw request/response bodies.
- Full query strings.
- Emails and personal data.
- High-cardinality business identifiers.

## Prohibited In Baggage

- Any secret or credential.
- User identifiers.
- Emails.
- Request payload values.
- Authorization or tenant claims.
- Order, payment or entity identifiers.

No baggage is added in this baseline.

## Prohibited In Metric Labels

- Raw URL or query string.
- User, order, entity or request identifiers.
- Email.
- Exception message.
- Token or credential presence/value.
- SQL command text.

## Prohibited In Logs

Rules from prompt 03 remain active:

- No Authorization values.
- No cookies.
- No complete query strings.
- No raw payloads.
- No connection strings.
- No JWTs or access tokens.
- No passwords or client secrets.

Logs may include `TraceId` and `SpanId` for correlation.

## Prohibited In Exception Events

- Sensitive exception messages must not be exposed to clients.
- Stack traces must not be exposed in production responses.
- Exception telemetry must not be enriched with payloads, tokens, credentials or SQL parameters.

`RecordExceptions=true` is enabled so unexpected failures can be correlated. The app does not add sensitive exception attributes beyond the official instrumentation defaults.

## Runtime Controls

- Query redaction for ASP.NET Core and HttpClient OpenTelemetry instrumentation is forced on by setting the experimental disable-redaction environment switches to `false` before instrumentation is registered.
- EF Core instrumentation is registered without custom SQL command enrichment.
- OpenTelemetry logs provider is not enabled, avoiding duplicate log pipelines.
