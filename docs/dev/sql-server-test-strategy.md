# SQL Server Test Strategy

Status date: 2026-06-18

This repository should validate SQL Server readiness without making ordinary local or CI test runs depend on a user-managed SQL Server instance.

## Goals

- Keep the default `dotnet test --no-build` path provider-independent and fast.
- Keep secrets, passwords, and real SQL Server connection strings out of the repository.
- Allow optional SQL Server-specific smoke coverage only when a caller explicitly opts in.
- Keep migration replay, schema validation, and provider-specific checks manual or disposable by default.

## Default Test Layers

### Fast default layer

Use this layer on every normal local run and in ordinary CI:

- unit tests over domain logic
- service tests over EF Core InMemory or equivalent provider-independent fakes
- endpoint tests through the current `WebApplicationFactory` setup
- script/parser guard checks already documented elsewhere

This layer must remain the meaning of:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

It must not require:

- a real SQL Server instance
- remote infrastructure
- committed secrets
- automatic migration application against a shared database

### Optional SQL Server smoke layer

Use this layer only when a developer or a dedicated validation environment explicitly enables it.

Acceptable scope:

- connection/open smoke
- migration generation or replay against a disposable SQL Server database
- one or two read/write sanity checks for the current provider wiring
- failure-path validation for unavailable or misconfigured SQL Server access

This layer must stay:

- opt-in
- disposable
- isolated from the user's real shared SQL Server
- safe to skip when configuration is absent

## Gating Rules

Optional SQL Server smoke coverage should run only when all required inputs are present, for example:

- an explicit environment flag such as `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=true`
- a placeholder-safe SQL Server connection string provided outside version control
- a disposable/local target database chosen for the smoke run

If any required input is missing, the SQL Server smoke layer should:

- skip cleanly
- report that SQL Server smoke coverage was not enabled
- avoid failing normal provider-independent test runs

## Skip Pattern

Use a clear skip rule for any future SQL Server-specific test fixture:

1. Read the explicit enable flag.
2. Read the external connection string or disposable target configuration.
3. If either is missing, skip with a short reason.
4. If both are present, run only the SQL Server-specific smoke subset.

The repository should prefer skip-or-run behavior over implicit fallback to a real server.

## Migration Safety

Future SQL Server migration validation should follow these rules:

- generate or replay migrations only against disposable/local databases
- never apply migrations automatically during ordinary app startup
- never hide migration execution behind default `dotnet test`
- keep migration commands explicit in docs or dedicated helper scripts

## Current Repository Position

- Normal test runs are currently provider-independent.
- No SQL Server integration tests are configured.
- The current persisted gameplay and endpoint coverage should remain the default regression baseline.
- Any future SQL Server smoke task should add explicit gating rather than changing the meaning of ordinary test runs.

## Recommended Validation Split

Use this split for future SQL Server readiness work:

1. Always run `dotnet build --no-restore`.
2. Always run `dotnet test --no-build`.
3. Only run SQL Server smoke checks when the explicit SQL Server gate is enabled.
4. Record skipped SQL Server smoke coverage honestly when the gate is off.

## Current Honest Status

No SQL Server smoke tests are configured in the repository today.
