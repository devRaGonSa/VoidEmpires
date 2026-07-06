# Final DB Security Notes

Status date: 2026-07-06

This note captures the minimum security posture for a future user-managed SQL Server deployment of `VoidEmpires`.

It does not enable SQL Server in the checked-in runtime, does not apply migrations automatically, and does not permit committing credentials into the repository.

## Core Rules

- Keep all SQL Server usernames, passwords, and resolved connection strings outside source control.
- Do not place real credentials in `appsettings*.json`, task notes, scripts, Docker metadata, compose files, or documentation examples.
- Keep ordinary `dotnet build --no-restore` and `dotnet test --no-build` runs independent from the real SQL Server instance.
- Apply schema changes only through explicit, manually reviewed operations against an intentionally chosen target.

## Credential Guidance

- Prefer a dedicated application login for `VoidEmpires` instead of shared operator credentials.
- Avoid using `sa` for the application where feasible; reserve high-privilege accounts for explicit operator actions only.
- Grant the minimum rights needed for the current step:
  - application runtime access should not also imply unrestricted server administration
  - migration or schema-apply rights should be separated from ordinary gameplay runtime access where practical
- Use long, unique passwords generated outside the repository and rotate them through operator-managed secret storage.
- Replace placeholder values such as `<USER>` and `<PASSWORD>` only in local environment variables, user secrets, or operator-managed secret storage.
- Do not paste real SQL Server users, passwords, or full resolved connection strings into chat, committed docs, scripts, terminal logs, tickets, or task notes.

## Connection Security

- Keep SQL Server reachable only from the app host or other explicitly approved administration endpoints.
- Prefer firewall or allowlist rules over broad network exposure.
- Use encrypted connections in the external connection string.
- When the instance uses a private or self-managed certificate path, keep `Encrypt=True;TrustServerCertificate=True;` in the operator-managed connection string until a stricter certificate-validation path is fully in place.
- Do not publish the SQL Server port broadly just because the application runs on a NAS, VM, or container host.

Safe placeholder template:

```text
Server=192.168.178.28,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

Replace only the placeholders in environment variables or secret storage. `VoidEmpires_Dev` is the recommended first controlled validation database name; it remains an example target until an operator creates it manually.

## Controlled Validation Posture

- Create `VoidEmpires_Dev` manually in SSMS after reviewing the SQL.
- Keep the local connection string outside the repository.
- Use the opt-in smoke path only for the read-only `SELECT 1` connection check.
- Keep `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED` and `VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING` scoped to the local shell or operator-managed secret storage, then clear local shell variables after the smoke.
- Generate migration SQL only after the SQL Server baseline exists, then review the script before any manual apply.
- Apply scripts manually only after explicit operator approval.
- Run the app against SQL Server only after externally selecting the `sqlserver` provider.

## Deployment Hygiene

- Inject connection settings through environment variables, secret managers, NAS container settings, or equivalent operator-managed configuration.
- Do not bake credentials into images, compose files, or checked-in deployment manifests.
- If the app is hosted on a NAS or VM platform, keep database secrets scoped to the specific service and avoid reusing the same secret across unrelated workloads.
- Keep SQL Server management access separate from the public application entrypoints.

## Backup And Recovery Hygiene

- Take a tested backup before manual schema changes.
- Store backups in an operator-approved location that is independent from the running application volume where practical.
- Restrict backup access to administrators who actually need it.
- Periodically test restore on a disposable target so backups are not treated as theoretical protection only.
- If a schema apply fails, stop, assess the exact failure, and restore from backup when the database is left in an unsafe partial state.

## Current Repository Scope

- The checked-in runtime and design-time provider remain PostgreSQL/Npgsql today.
- SQL Server guidance in this repository is documentation for a future cutover path, not an active runtime commitment.
- Current SQL Server helpers are non-destructive by default: the create-database helper is manual and guarded, the connection smoke runs `SELECT 1`, the migration script helper stops until the SQL Server baseline exists, and the final catalog helper defaults to dry-run/deferred apply.
- No real SQL Server login, password, migration apply, firewall change, backup action, or restore action was performed for this task.
