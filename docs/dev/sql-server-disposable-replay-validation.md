# SQL Server Disposable Replay Validation

Status date: 2026-07-06

This checklist defines the repository-side validation gate for replaying the accepted SQL Server baseline script against a disposable database.

It is not a production apply runbook, it is not the `VoidEmpires_Dev` manual apply path, and it was not executed when this document was added.

## Purpose

Use this gate before claiming that `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql` is schema-replay ready.

The current repository validation proves:

- the SQL script exists and passes static safety checks
- default build and tests do not require SQL Server
- the optional smoke path can prove read-only connectivity when an operator supplies a connection string

It does not yet prove that the baseline script has been applied cleanly to a disposable SQL Server database.

## Required Target

Use a disposable SQL Server database only.

Recommended disposable name shape:

```text
VoidEmpires_Replay_<YYYYMMDDHHmm>
```

Do not use the user-managed `VoidEmpires_Dev` database, a shared test database, or any database containing user gameplay data for this replay gate.

## Preconditions

1. `dotnet build --no-restore` passes.
2. `dotnet test --no-build` passes.
3. `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-sqlserver-generated-script-safety.ps1` passes.
4. The operator has created or selected a disposable SQL Server database outside the repository workflow.
5. The connection string is held only in a local shell, user secret, or operator-managed secret store.
6. The operator has confirmed the database can be dropped or discarded after validation.

## Replay Steps

These steps are intentionally manual until an explicit disposable replay automation task exists.

1. Open `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql` in SSMS.
2. Connect to the disposable target with operator credentials.
3. Confirm the active database context is the disposable database, not `VoidEmpires_Dev`.
4. Review the script header, `__EFMigrationsHistory` guard, table creation order, constraints, indexes, and final migration-history insert.
5. Execute the script manually only after confirming the target is disposable.
6. Stop immediately on any SQL error.
7. Run `docs/dev/sql-server-post-apply-verification.sql` against the disposable database.
8. Confirm `dbo.__EFMigrationsHistory` contains `20260706131610_SqlServerInitialBaseline`.
9. Rerun the same SQL script against the same disposable database to verify the idempotent guard is safe.
10. Run `docs/dev/sql-server-post-apply-verification.sql` again and compare the migration-history result.
11. Run the read-only SQL Server smoke helper against the disposable database.
12. Drop or discard the disposable database after recording private operator notes.

## Stop Conditions

Stop and create a repository task before retrying if any of these occur:

- the script targets the wrong database
- the script requires real credentials inside a committed file
- the first apply fails
- the second idempotent replay changes schema unexpectedly or fails
- `dbo.__EFMigrationsHistory` is missing `20260706131610_SqlServerInitialBaseline`
- post-apply verification finds missing expected tables or indexes
- the read-only smoke fails after the schema is present

## Explicit Non-Goals

- Do not run `dotnet ef database update`.
- Do not hide script apply inside app startup.
- Do not hide script apply inside `dotnet test`.
- Do not apply this gate to a database with user data.
- Do not treat Development seed success as final catalog seed readiness.

## Current Status

This gate is documented but not automated. No SQL Server connection, script execution, migration apply, seed apply, backup, restore, or database drop was performed while adding this document.
