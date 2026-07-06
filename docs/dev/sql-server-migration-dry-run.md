# SQL Server Migration Dry Run

Status date: 2026-07-06

This document describes the safest dry-run posture for future SQL Server migration work in `VoidEmpires`.

It is intentionally conservative. The repository does not currently support an end-to-end checked-in SQL Server migration script flow, and no repository path should auto-apply migrations to a real user-managed server.

## Current Reality

- the checked-in runtime provider remains PostgreSQL-first by default
- the checked-in design-time factory now supports explicit SQL Server provider selection through environment variables
- the checked-in migration history is PostgreSQL-shaped
- `docs/dev/sql-server-migration-strategy.md` now records the deferred baseline-generation prerequisites and commands
- `scripts/sqlserver-script-migration.ps1` now exists as a guarded generation-only helper
- current decision for `SqlServerInitialBaseline`: no-go until the SQL Server migration layout, provider-specific model review, and snapshot policy are completed
- `TASK-39G` closed as a documented deferral; no `SqlServerInitialBaseline` files were generated
- `TASK-39H` verified the idempotent SQL output path remains blocked until that baseline exists; no generated SQL output is committed

Because of those constraints, the current dry-run process is a manual readiness checklist, not a completed repository automation path.

## Goal Of The Dry Run

- confirm the normal repository baseline still passes
- review the intended SQL migration batch before any apply
- test the batch only against a disposable or intentionally isolated target
- verify schema outcomes manually
- keep rollback and backup expectations explicit

## 1. Run The Ordinary Repository Baseline

Before any SQL Server-specific activity:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

This confirms the ordinary regression baseline before any manual SQL migration rehearsal.

## 2. Confirm The Dry-Run Preconditions

Do not start a SQL Server dry run unless all of the following are true:

1. the target is disposable or intentionally isolated from the real shared environment
2. the operator understands that SQL Server provider cutover is not complete in the checked-in code
3. a fresh backup exists for any database that matters
4. the no-auto-apply rule remains in effect

If any of those conditions are false, stop.

## 3. Generate Or Assemble The Candidate Script Manually

Current limitation:

- the helper only runs after `SqlServerInitialBaseline` exists under the expected SQL Server migration directory
- `SqlServerInitialBaseline` must not be generated yet from the current repository state

Practical implication:

- do not assume `dotnet ef migrations script` from the current checked-in provider setup is SQL Server-safe
- do not treat the current migration set as directly replayable on SQL Server without the later strategy work

Current safe posture:

1. follow `docs/dev/sql-server-migration-strategy.md` for the deferred baseline-generation prerequisites
2. run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-script-migration.ps1` only after that baseline exists and has been reviewed
3. keep the generated script local to the operator workflow
4. review the script before it touches any database

## 4. Review The Script Before Any Apply

Review the candidate SQL batch manually for:

- expected target database and schema names
- provider-specific syntax that still reflects PostgreSQL assumptions
- filtered indexes or migration SQL branches that may not be SQL Server-safe
- destructive statements, table rebuilds, or data backfills
- any seed data that should remain outside the migration batch

If the script cannot be explained clearly, do not continue with the dry run.

## 5. Apply Only To A Disposable Validation Target

When a reviewed SQL Server batch exists:

1. connect to the disposable validation target in SSMS
2. confirm the database is not the real shared environment
3. apply the script manually
4. record which script version was used and when

Do not:

- hide apply inside app startup
- hide apply inside `dotnet test`
- point the dry run at the real production-like target by convenience

## 6. Verify The Schema Manually

After the disposable apply:

1. confirm expected tables, indexes, and columns exist
2. inspect any filtered indexes or provider-specific objects that were part of the review
3. verify the database remains reachable
4. check for obvious data-shape or constraint failures

This step is manual because the repository does not yet include a validated SQL Server migration verification harness.

## 7. Run Smoke Validation

If the operator has a disposable SQL Server target and external connection settings prepared, run the opt-in smoke coverage:

```powershell
dotnet test --no-build --filter FullyQualifiedName~SqlServerSmokeTests
```

Expected scope:

- the smoke test should only verify connection-level behavior
- it should not be treated as proof that migration replay is complete
- it should not replace manual schema review

## 8. Backup And Rollback Caveats

Before any dry-run apply that matters operationally:

- take a fresh backup
- know where the backup is stored
- know where restore would be rehearsed

If the dry run fails:

1. stop the batch
2. capture the exact failing statement
3. assess whether the disposable target can simply be discarded or must be restored
4. fix the migration or script issue in a dedicated task before retrying

Use `docs/dev/final-db-backup-restore-plan.md` and `docs/dev/final-db-security-notes.md` as the baseline for backup and operational safety.

## No-Auto-Apply Policy

This repository must keep SQL Server migration work manual by default.

That means:

- no automatic apply on web startup
- no automatic apply in tests
- no automatic apply in helper scripts against the real user-managed server
- no committed secrets to simplify operator steps

## Current Honest Conclusion

Today, the dry-run documentation is still a guarded manual procedure because the SQL Server baseline generation and script-helper tasks are not yet completed.

No real SQL Server migration generation, apply, or rollback was performed for this documentation task.
