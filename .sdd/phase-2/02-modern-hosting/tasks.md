# Tasks - 02 Modern Hosting

## Specification

- [x] Confirm branch, clean working tree and Prompt 1 commit.
- [x] Read Phase 2 shared context and Prompt 1 handoff.
- [x] Define acceptance criteria and non-goals.

## Discovery

- [x] Map service registrations.
- [x] Map middleware order.
- [x] Identify duplicated MVC/controller registration.
- [x] Identify static configuration access.
- [x] Confirm tests and HTTP collection state.

## Design

- [x] Design one modern `Program.cs` composition root.
- [x] Design cohesive API extensions.
- [x] Record middleware order changes.

## Development

- [x] Replace legacy host setup with `WebApplication.CreateBuilder`.
- [x] Consolidate service registration under `AddApiServices`.
- [x] Consolidate pipeline registration under `UseApiPipeline`.
- [x] Move Serilog setup to host configuration.
- [x] Remove static configuration helper.
- [x] Remove duplicated MVC registration.
- [x] Remove the legacy startup class file.

## Validation

- [x] Restore.
- [x] Release build.
- [x] Release tests.
- [x] Host-cleanup searches.
- [x] Smoke startup, Swagger, endpoint, authentication challenge, CORS, health check and shutdown.
- [x] Diff review.

## Delivery

- [x] Update Phase 2 shared context.
- [x] Commit `refactor: adopt modern ASP.NET Core hosting`.
- [x] Confirm clean working tree after commit.
