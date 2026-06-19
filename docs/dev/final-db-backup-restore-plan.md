# Final DB Backup Restore Plan

Status date: 2026-06-19

This document records the expected backup and restore posture for a future user-managed SQL Server deployment of `VoidEmpires`.

It is documentation only. It does not automate backups, does not automate restores, and does not authorize applying changes to a real SQL Server instance from the repository.

## Goals

- protect the final database before manual schema work
- define a repeatable backup cadence for normal operations
- require restore validation instead of assuming backups are usable
- keep all operations under explicit operator control

## Scope

- manual SSMS backups
- operator-scheduled backup recommendations
- restore testing on a disposable target
- pre-migration backup expectations

Out of scope:

- automatic repository-driven SQL Server backup jobs
- automatic restore scripts against the real target
- changes to runtime provider wiring

## Baseline Rules

- Take a fresh backup before any manual schema apply, migration replay, or risky data correction.
- Keep backups outside the application repository and outside the running app container or service volume where practical.
- Restrict backup and restore permissions to trusted operators.
- Treat restore testing as mandatory operational validation, not optional documentation.

## Manual SSMS Backup Flow

Use SQL Server Management Studio when performing a manual backup:

1. Connect to the intended SQL Server instance with operator credentials.
2. Confirm the exact target database name before starting.
3. Right-click the database and open `Tasks` -> `Back Up...`.
4. Prefer a full backup before schema changes.
5. Choose an operator-approved destination with enough free space and retention coverage.
6. Label the backup clearly with database name, environment, and timestamp.
7. Record why the backup was taken, for example pre-migration, pre-seed validation, or pre-maintenance.

Minimum expectation:

- do not begin manual schema work until the backup completes successfully
- do not overwrite the only recent known-good backup

## Scheduled Backup Recommendations

Recommended baseline for a user-managed SQL Server:

- daily full backups for small or early-stage deployments when operational simplicity matters most
- additional differential or log backups when the service grows and tighter recovery objectives are required
- retention policies that keep multiple restore points instead of a single rolling file
- backup storage on infrastructure independent from the app runtime where practical, especially for NAS-hosted or VM-hosted deployments

Operator notes:

- align cadence with your own RPO and RTO requirements
- verify storage capacity and cleanup policy before relying on scheduled jobs
- protect backup destinations with access controls equivalent to or stricter than the live database

## Restore Testing Expectations

Backups are not considered trustworthy until restore has been tested.

Recommended restore drill:

1. Choose a disposable SQL Server target or isolated validation database.
2. Restore the latest full backup.
3. Confirm the restore completes without corruption or missing files.
4. Run minimal sanity checks such as database open, table presence, and expected row visibility.
5. Record the restore date, source backup, target location, and outcome in operator notes.

Do not:

- run restore drills against the live production target
- treat an untested backup chain as sufficient protection

## Pre-Migration Safeguard

Before any future manual SQL Server migration apply:

1. Run the ordinary repository validation baseline:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
2. Confirm the script or change batch has been reviewed manually.
3. Take a fresh full backup of the target database.
4. Verify where the backup is stored and who can restore it.
5. Confirm the restore target for rollback testing is available or can be provisioned quickly.
6. Only then begin manual apply in SSMS or another explicit operator workflow.

If any of the steps above are missing, stop and do not continue with the schema change.

## Failure And Recovery Posture

If a manual apply fails:

1. Stop the operation and do not continue with later batches.
2. Capture the exact failing statement and error.
3. Decide whether the database remains safe to inspect in place or whether restore is required immediately.
4. If the state is unsafe or partially applied, restore from the pre-change backup on the chosen recovery target.
5. Fix the script or migration issue in a dedicated repository task before retrying.

## Relationship To Other Docs

- `docs/dev/sql-server-runbook.md` describes the broader manual SQL Server workflow.
- `docs/dev/final-db-security-notes.md` covers credential, encryption, and least-privilege expectations for the same deployment model.

## Current Repository Scope

- The repository still uses PostgreSQL/Npgsql in checked-in runtime and design-time paths today.
- This plan does not create SQL Agent jobs, PowerShell backup scripts, or automated restore tooling.
- No real SQL Server backup or restore action was performed for this documentation task.
