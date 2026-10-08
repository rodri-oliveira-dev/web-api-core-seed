# Architecture Decision Records (ADRs)

[Back to architecture](../architecture.md) | [Back to README](../../README.md)

ADRs document significant **current** architectural choices and their trade-offs. These are concise records reflecting implemented code, not proposed changes.

| ADR | Decision | Status |
| --- | --- | --- |
| [0001](0001-explicit-ports-and-modules.md) | Explicit ports and SampleRestaurant module boundaries | Accepted (implemented) |
| [0002](0002-separate-dbcontexts-and-explicit-seed.md) | Per-module migrations and explicit non-production seed | Accepted (implemented) |
| [0003](0003-response-cache-and-observability.md) | Restricted shared response caching and opt-in telemetry export | Accepted (implemented) |

A change that reverses a decision should add a new ADR describing its motivation and migration strategy; do not silently rewrite historical rationale.