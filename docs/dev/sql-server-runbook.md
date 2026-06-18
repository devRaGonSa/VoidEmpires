# SQL Server Runbook

Status date: 2026-06-18

This runbook describes the practical, user-managed SQL Server workflow for the future final database target.

It does not make SQL Server the default runtime provider, does not apply migrations automatically, and does not store passwords in the repository.

## Current Position

- The checked-in runtime and design-time provider are still PostgreSQL/Npgsql.
- SQL Server is a documented future target, not the active checked-in provider.
- Normal validation remains:
  - `dotnet build --no-restore`
  - `dotnet test --no-build`
- The only SQL Server-specific validation currently in the repo is the opt-in `SqlServerSmokeTests` connection check.

Use this runbook as a manual operations guide, not as proof that SQL Server cutover is already implemented.

## Safety Rules

- Keep SQL Server credentials outside source control.
- Do not place a real password in any committed file, script, or example.
- Do not apply migrations automatically during app startup or normal test runs.
- Run migration generation, script review, and apply steps only against a disposable or intentionally chosen target.
- Prefer explicit manual apply through SSMS or reviewed SQL scripts for the real user-managed database.

## Safe Connection String Template

Use a placeholder-only template such as:

```text
Server=192.168.178.28,1433;Database=VoidEmpires;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

Replace only `<USER>` and `<PASSWORD>` in local environment variables or secret storage. Do not commit resolved values.

## 1. Create The Database In SSMS

In SQL Server Management Studio:

1. Connect to the target SQL Server instance with your own operator credentials.
2. Create a new database named `VoidEmpires` or your chosen final name.
3. Keep collation, file placement, growth policy, and backup policy aligned with your own infrastructure standards.
4. Create or assign the application login/user outside the repository workflow.
5. Grant only the minimum rights needed for schema apply and application access.

Manual note:

- This repository does not create the SQL Server database for you.
- This repository does not create SQL logins or passwords for you.

## 2. Set Local Environment Variables

For design-time tools and optional smoke validation, prefer environment variables over appsettings edits.

PowerShell example for the current shell:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=192.168.178.28,1433;Database=VoidEmpires;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
$env:VOIDEMPIRES_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
$env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED="true"
$env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
```

Current repository behavior:

- Web startup reads `ConnectionStrings:DefaultConnection`.
- Design-time `VoidEmpiresDbContextFactory` reads:
  - `ConnectionStrings__DefaultConnection`
  - `VOIDEMPIRES_CONNECTION_STRING`
- SQL Server smoke validation reads:
  - `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED`
  - `VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING`

## 3. Run Default Repository Validation First

Always verify the normal provider-independent baseline before any SQL Server-specific step:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

This confirms the ordinary repository regression baseline before any manual SQL Server activity.

## 4. Run The SQL Server Connection Smoke Check

The repository currently has no dedicated SQL Server PowerShell helper script. Use the opt-in smoke test directly:

```powershell
dotnet test --no-build --filter FullyQualifiedName~SqlServerSmokeTests
```

Expected current behavior:

- if the SQL Server gate and connection string are not set, the test reports that smoke coverage was skipped
- if both are set, the test opens a SQL connection and runs read-only `SELECT 1`
- it does not create a database
- it does not apply migrations
- it does not write gameplay data

## 5. Generate Migration Scripts Manually

Important current limitation:

- the checked-in EF design-time factory still uses `UseNpgsql(...)`
- the checked-in migrations are PostgreSQL-shaped
- direct SQL Server script generation is not yet a validated repository path

Because of that, do not treat current migration generation as ready for SQL Server replay on the real target.

Safe current runbook posture:

1. First complete the future provider-selection and SQL Server migration-baseline tasks.
2. After those tasks exist, generate SQL scripts explicitly and review them before apply.
3. Keep generated scripts outside automatic startup and outside default tests.

Planned command shape for that later phase:

```powershell
dotnet ef migrations script --idempotent --output .\artifacts\sql\voidempires-sqlserver.sql
```

Current honest note:

- Do not run the command above yet against the current checked-in provider setup and assume it is SQL Server-safe.

## 6. Review And Apply Scripts Manually

When a SQL Server-specific migration baseline exists in a later task:

1. Open the reviewed SQL script in SSMS.
2. Confirm the target server and database manually.
3. Review schema names, table names, index filters, provider-specific SQL, and any seed inserts.
4. Apply the script manually in SSMS.
5. Record the applied script version in your own operational notes.

Do not:

- hide apply inside app startup
- hide apply inside `dotnet test`
- run apply against a shared production-like server without manual confirmation

## 7. Seed Catalogs And Development Data Carefully

Current repository position:

- static gameplay catalogs are still code-owned, not final relational seed rows
- Development seed profiles are QA scaffolding, not final production initialization

Operational guidance:

- Do not treat `POST /api/dev/seeds/apply` as the final production catalog seed path.
- Use Development seeds only for local QA after the app is intentionally configured to talk to a disposable database.
- Keep final catalog seeding separate from player-owned gameplay state seeding.

If you intentionally run Development seeds against a disposable SQL Server validation database:

1. Start the app only after setting a non-empty `ConnectionStrings__DefaultConnection`.
2. Confirm you are pointing to a disposable validation database, not a shared real environment.
3. Call the existing Development seed endpoint only for local/demo validation.

## 8. Back Up The Database

For a user-managed SQL Server, backup remains a manual infrastructure step.

Minimum runbook expectation:

1. Take a full backup before manually applying schema changes.
2. Store the backup in your approved infrastructure location.
3. Verify restore permissions and restore destination ahead of risky schema work.
4. Label backups clearly with database name, environment, and timestamp.

SSMS path:

- right-click database
- `Tasks`
- `Back Up...`

Preferred operational posture:

- full backup before manual apply
- optional differential or log backup based on your own RPO/RTO requirements
- test restore on a disposable server when the schema batch is high risk

## 9. Restore / Rollback Guidance

Current repository does not provide an automated SQL Server rollback script.

If a manual SQL apply fails:

1. Stop and do not continue applying later batches.
2. Review the exact failed statement in SSMS.
3. If partial apply makes the database unsafe, restore from the pre-change backup.
4. Fix the migration/script issue in a dedicated repository task before retrying.

## 10. Known Current Blockers

- runtime provider selection is still hard-coded to Npgsql
- design-time provider selection is still hard-coded to Npgsql
- existing migration history is PostgreSQL-shaped
- filtered-index SQL and provider-specific migration SQL still need SQL Server audit
- no checked-in SQL Server migration script helper exists yet
- no final relational catalog seed path exists yet

## 11. Recommended Future Task Order

1. provider selection and dependency split
2. SQL Server-compatible migration-baseline strategy
3. SQL Server script-generation helper or documented command flow
4. final relational catalog seed ownership
5. explicit SQL Server validation gate and disposable replay checks
6. final operational backup and restore verification

## Validation

- This runbook is based on the current final DB prep note, SQL Server test strategy, current startup/configuration behavior, and the existing opt-in SQL Server smoke test.
- No real SQL Server connection, migration apply, seed apply, backup, or restore was performed for this documentation task.
