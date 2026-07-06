# SQL Server Runbook

Status date: 2026-07-06

This runbook describes the practical, user-managed SQL Server workflow for the final database target while the checked-in repository still remains PostgreSQL-first by default today.

Recommended first controlled validation database: `VoidEmpires_Dev`.

It does not make SQL Server the default runtime provider, does not apply migrations automatically, and does not store passwords in the repository.

## Current Position

- The checked-in runtime and design-time default provider are still PostgreSQL/Npgsql.
- SQL Server provider support is now present in the Infrastructure project, but it is selected only when configuration explicitly requests it.
- SQL Server is a documented future target, not the active checked-in provider.
- The repository does not currently ship a `docker-compose` file, SQL Server container definition, or NAS-specific deployment bundle.
- Normal validation remains:
  - `dotnet build --no-restore`
  - `dotnet test --no-build`
- The only SQL Server-specific validation currently in the repo is the opt-in `SqlServerSmokeTests` connection check.

Use this runbook as a manual operations guide, not as proof that SQL Server cutover is already implemented.

Related reference:

- `docs/dev/final-db-security-notes.md`

## Safety Rules

- Keep SQL Server credentials outside source control.
- Do not place a real password in any committed file, script, or example.
- Do not apply migrations automatically during app startup or normal test runs.
- Run migration generation, script review, and apply steps only against a disposable or intentionally chosen target.
- Prefer explicit manual apply through SSMS or reviewed SQL scripts for the real user-managed database.
- Treat `VoidEmpires_Dev` as a safe recommended validation target name, not as proof that the repository has created or modified a real SQL Server database.

## Ruta controlada para `VoidEmpires_Dev`

Para la primera validacion manual use esta secuencia:

1. Crear `VoidEmpires_Dev` manualmente en SSMS despues de revisar el SQL.
2. Configurar la cadena local fuera del repositorio, por ejemplo en variables de entorno o user secrets.
3. Ejecutar la validacion de conexion `SELECT 1` mediante el smoke opt-in.
4. Generar el script de migracion solo cuando exista una baseline SQL Server revisada.
5. Revisar el script generado antes de abrirlo en SSMS.
6. Aplicarlo manualmente solo si el operador lo aprueba.
7. Ejecutar el smoke SQL Server opcional contra `VoidEmpires_Dev`.
8. Ejecutar la app contra SQL Server solo despues de seleccionar `sqlserver` mediante configuracion externa.

No conectes este flujo a una base compartida o productiva por comodidad.

## Safe Connection String Template

Use a placeholder-only template such as:

```text
Server=192.168.178.28,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

Replace only `<USER>` and `<PASSWORD>` in local environment variables or secret storage. Do not commit resolved values.

## 1. Create The Database In SSMS

In SQL Server Management Studio:

1. Open `scripts/sqlserver/create-database.sql` in SSMS.
2. Connect to the target SQL Server instance with your own operator credentials.
3. Confirm the active database context is `master`, then review the script comments and any infrastructure-specific sizing or file-placement adjustments you need.
4. Execute the script manually only after review; it uses `DB_ID(...)` guards and targets `VoidEmpires_Dev` as the recommended first controlled validation database.
5. Create or assign the application login/user outside the repository workflow.
6. Grant only the minimum rights needed for schema apply and application access.

Manual note:

- This repository does not create the SQL Server database for you.
- The checked-in helper is an SSMS-oriented manual script, not an automatic provisioning step.
- This repository does not create SQL logins or passwords for you.
- `VoidEmpires_Dev` is the recommended first validation database name until a later task promotes any different target.
- The helper does not drop, truncate, reset, migrate, or seed data.

## 2. Configurar la conexion local sin commitear secretos

Usa variables de entorno o user secrets. No edites `appsettings*.json` con valores reales.

PowerShell para la sesion actual:

```powershell
$env:VoidEmpires__Persistence__Provider="SqlServer"
$env:ConnectionStrings__DefaultConnection="Server=192.168.178.28,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
$env:VOIDEMPIRES_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
$env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED="true"
$env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
```

Reemplaza `<USER>` y `<PASSWORD>` solo en tu entorno local. Si usas notas privadas con nombres como `YOUR_USER` o `YOUR_PASSWORD`, no las pegues en chat, documentos, scripts, logs de terminal, tickets ni commits.

El proyecto `src/VoidEmpires.Web` tiene `UserSecretsId`, asi que tambien puedes guardar la configuracion local con .NET user-secrets:

```powershell
dotnet user-secrets set "VoidEmpires:Persistence:Provider" "SqlServer" --project .\src\VoidEmpires.Web\VoidEmpires.Web.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=192.168.178.28,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;" --project .\src\VoidEmpires.Web\VoidEmpires.Web.csproj
```

Los user secrets son locales a la maquina de desarrollo y no se commitean. Para el smoke script sigue usando variables de entorno o el parametro `-ConnectionString`, porque ese helper lee configuracion explicita fuera del repositorio.

Current repository behavior:

- Web startup reads `ConnectionStrings:DefaultConnection`.
- Web startup reads `VoidEmpires:Persistence:Provider` and stays on PostgreSQL unless `sqlserver` is selected explicitly.
- Design-time `VoidEmpiresDbContextFactory` reads:
  - `VoidEmpires__Persistence__Provider`
  - `VOIDEMPIRES_DATABASE_PROVIDER`
  - `ConnectionStrings__DefaultConnection`
  - `VOIDEMPIRES_CONNECTION_STRING`
- SQL Server smoke validation reads:
  - `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED`
  - `VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING`

Deployment note:

- Keep these values in the host environment, secret manager, NAS container settings, VM service configuration, or equivalent operator-managed secret storage.
- Do not set `VoidEmpires:Persistence:Provider` to `sqlserver` in checked-in appsettings; keep that selection external just like the real connection string.
- Do not commit a resolved SQL Server connection string into appsettings, Docker metadata, compose files, or checked-in deployment notes.
- If the SQL Server instance uses a privately managed certificate chain, keep `Encrypt=True;TrustServerCertificate=True;` in the external connection string unless your operator-managed certificate validation path is already in place.

## 3. Run Default Repository Validation First

Always verify the normal provider-independent baseline before any SQL Server-specific step:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

This confirms the ordinary repository regression baseline before any manual SQL Server activity.

## 4. Run The SQL Server Connection Smoke Check

The repository now includes a dedicated read-only helper:

```powershell
$sqlServerConnectionString=$env:ConnectionStrings__DefaultConnection
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-connection-smoke.ps1 -ConnectionString $sqlServerConnectionString
```

Optional direct parameter form with placeholders only:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-connection-smoke.ps1 `
  -ConnectionString "Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

Expected current behavior:

- if no connection string is provided, the helper stops with external-configuration guidance
- if a connection string is provided, the helper runs the opt-in `SqlServerSmokeTests` path
- the underlying smoke path opens a SQL connection and runs read-only `SELECT 1`
- it does not create a database
- it does not apply migrations
- it does not inspect schema
- it does not write gameplay data
- it does not echo the provided password
- common failures point to host/port reachability, SQL Authentication/login state, missing `VoidEmpires_Dev`, and encryption/certificate setup

## 5. Generate Migration Scripts Manually

Important current limitation:

- the checked-in migrations are PostgreSQL-shaped
- direct SQL Server script generation is not yet a validated default repository path

Because of that, do not treat current migration generation as ready for SQL Server replay on the real target.

Safe current runbook posture:

1. First follow `docs/dev/sql-server-migration-strategy.md` and complete its documented prerequisites for the first SQL Server baseline.
2. Use the deferred environment-variable setup and `dotnet ef migrations add SqlServerInitialBaseline ...` command from that strategy document instead of improvising a mixed-provider migration.
3. After that baseline exists, prefer `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-script-migration.ps1` to generate the local idempotent SQL script without applying it.
4. Keep generated scripts outside automatic startup and outside default tests.

Planned later script command shape:

```powershell
dotnet ef migrations script 0 SqlServerInitialBaseline --output .\artifacts\sql\voidempires-sqlserver-initial-baseline.sql
```

Current honest note:

- Do not run the command above yet without first following the deferred-generation prerequisites in `docs/dev/sql-server-migration-strategy.md`.
- The helper script is intentionally guarded and will stop if `SqlServerInitialBaseline` does not exist yet.

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
- the backend now includes a dry-run-first final catalog seed service, but non-dry-run apply is still intentionally deferred

Operational guidance:

- Do not treat `POST /api/dev/seeds/apply` as the final production catalog seed path.
- Use Development seeds only for local QA after the app is intentionally configured to talk to a disposable database.
- Keep final catalog seeding separate from player-owned gameplay state seeding.
- Use `scripts/sqlserver-final-catalog-seed.ps1` only as an operator-invoked guardrail around the current backend seed service.

Final catalog helper examples:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-final-catalog-seed.ps1 `
  -ConnectionString "Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

```powershell
$env:VOIDEMPIRES_CONNECTION_STRING="Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-final-catalog-seed.ps1
```

Current expected behavior of the helper:

- it requires explicit connection context from a parameter or environment variable before it runs
- it does not echo the connection string or password
- it defaults to a dry-run path that validates the versioned catalog JSON sources through the backend seed service
- if `-Apply -ConfirmMutation` is requested, the helper still stops safely because the backend service currently defers non-dry-run execution until final relational catalog tables and manual apply wiring exist
- it does not call Development seed endpoints
- it does not run migrations or perform automatic SQL Server updates

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
- if the app runs from a NAS-hosted container or VM, keep backups on storage that is independent from the application runtime volume

## 9. Restore / Rollback Guidance

Current repository does not provide an automated SQL Server rollback script.

If a manual SQL apply fails:

1. Stop and do not continue applying later batches.
2. Review the exact failed statement in SSMS.
3. If partial apply makes the database unsafe, restore from the pre-change backup.
4. Fix the migration/script issue in a dedicated repository task before retrying.

## 10. Known Current Blockers

- runtime and design-time provider selection now support an explicit SQL Server choice, but PostgreSQL remains the checked-in default path
- existing migration history is PostgreSQL-shaped
- filtered-index SQL and provider-specific migration SQL still need SQL Server audit
- no checked-in SQL Server migration script helper exists yet
- final catalog operator scripting now exists only as a guarded dry-run and deferred-apply helper; no final relational catalog seed apply path exists yet

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
