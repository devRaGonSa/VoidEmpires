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
- `docs/dev/sql-server-user-checklist.md`
- `docs/dev/sql-server-disposable-replay-validation.md`

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
5. Ejecutar el guard estatico del script SQL generado.
6. Hacer backup manual de `VoidEmpires_Dev`.
7. Abrir y revisar el script generado en SSMS.
8. Verificar que SSMS apunta a `VoidEmpires_Dev`.
9. Aplicarlo manualmente solo si el operador lo aprueba.
10. Inspeccionar tablas y consultar `dbo.__EFMigrationsHistory`.
11. Ejecutar el smoke SQL Server opcional contra `VoidEmpires_Dev`.
12. Ejecutar la app contra SQL Server solo despues de seleccionar `sqlserver` mediante configuracion externa.

No conectes este flujo a una base compartida o productiva por comodidad.

## Safe Connection String Template

Use a placeholder-only template such as:

```text
Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
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
$env:ConnectionStrings__DefaultConnection="Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
$env:VOIDEMPIRES_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
$env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED="true"
$env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
```

Reemplaza `<USER>` y `<PASSWORD>` solo en tu entorno local. Si usas notas privadas con nombres como `YOUR_USER` o `YOUR_PASSWORD`, no las pegues en chat, documentos, scripts, logs de terminal, tickets ni commits.

El proyecto `src/VoidEmpires.Web` tiene `UserSecretsId`, asi que tambien puedes guardar la configuracion local con .NET user-secrets:

```powershell
dotnet user-secrets set "VoidEmpires:Persistence:Provider" "SqlServer" --project .\src\VoidEmpires.Web\VoidEmpires.Web.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;" --project .\src\VoidEmpires.Web\VoidEmpires.Web.csproj
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

El smoke real de SQL Server es opcional. `dotnet test --no-build` no requiere SQL Server en el flujo normal; `SqlServerSmokeTests` solo intenta conectarse cuando `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=true` y existe una cadena externa en `VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING`.

Configura la cadena en una variable local antes de ejecutarlo:

```powershell
$env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED="true"
$env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING="Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

The repository includes a dedicated read-only helper:

```powershell
$sqlServerConnectionString=$env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING
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

Limpia las variables de la sesion despues de la prueba:

```powershell
Remove-Item Env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED -ErrorAction SilentlyContinue
Remove-Item Env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING -ErrorAction SilentlyContinue
Remove-Item Env:VOIDEMPIRES_CONNECTION_STRING -ErrorAction SilentlyContinue
```

## 5. Generate Migration Scripts Manually

Current baseline state:

- `SqlServerInitialBaseline` exists under `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`
- the committed review artifact is `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql`
- repository helpers generate and statically review SQL only; they do not apply it

To regenerate the reviewed artifact after a migration change, run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-script-migration.ps1 `
  -OutputPath .\artifacts\sqlserver\VoidEmpires_Dev_SqlServerInitialBaseline.sql
```

Before opening the script in SSMS, run the static safety guard:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-sqlserver-generated-script-safety.ps1
```

The helper and guard do not connect to `VoidEmpires_Dev`, do not run `dotnet ef database update`, and do not apply schema changes. They only produce and inspect a SQL file for manual review.

If the migration baseline changes, repeat the provider-sensitive review in `docs/dev/sql-server-migration-review.md` before using a regenerated script.

## 6. Manual SSMS Apply Runbook

This is the only approved apply path for the reviewed SQL Server baseline script. It is an operator action, not a repository script action.

Before using this path as evidence of repeatable schema replay, run the separate disposable replay checklist in `docs/dev/sql-server-disposable-replay-validation.md` against a disposable database. That checklist is intentionally separate from `VoidEmpires_Dev` and must not target user data.

Pre-apply checks:

1. Confirm `dotnet build --no-restore` and `dotnet test --no-build` pass locally.
2. Confirm the read-only smoke in section 4 passes against `VoidEmpires_Dev`.
3. Confirm the generated SQL guard in section 5 passes.
4. Confirm no resolved usernames, passwords, or full real connection strings were copied into the script or notes.

Backup:

1. In SSMS, right-click `VoidEmpires_Dev`.
2. Select `Tasks` > `Back Up...`.
3. Take a full backup to the operator-approved location.
4. Verify the backup completed successfully and record the backup timestamp in private operational notes.

Manual review and apply:

1. Open `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql` in SSMS.
2. Connect with operator credentials; do not paste repository placeholder strings as real credentials.
3. Verify the target server manually in SSMS.
4. Verify the database dropdown or query context is `VoidEmpires_Dev`.
5. Review the script header, `__EFMigrationsHistory` guards, table creation order, indexes, constraints, and the final history insert.
6. Execute the script manually only after operator approval.
7. Stop immediately on any error; do not continue with later batches until the failure is understood.

Read-only post-apply inspection:

Use `docs/dev/sql-server-post-apply-verification.sql` as the reviewed post-apply verification checklist. Open it in SSMS against `VoidEmpires_Dev`; it lists tables, verifies `__EFMigrationsHistory`, runs empty-safe row counts, checks important indexes, and reports whether catalog-like tables exist.

```sql
SELECT DB_NAME() AS CurrentDatabase;
SELECT [name] FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY [name];
SELECT [MigrationId], [ProductVersion] FROM [dbo].[__EFMigrationsHistory] ORDER BY [MigrationId];
```

Expected inspection result after a successful manual apply:

- `CurrentDatabase` is `VoidEmpires_Dev`
- gameplay and Identity tables are visible
- `dbo.__EFMigrationsHistory` contains `20260706131610_SqlServerInitialBaseline`

Then rerun the read-only smoke from section 4:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-connection-smoke.ps1 `
  -ConnectionString "Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

Repository scripts do not perform the manual apply above. This runbook documents the operator-controlled process only and does not claim the script has been applied.

Do not:

- hide apply inside app startup
- hide apply inside `dotnet test`
- run apply against a shared production-like server without manual confirmation

## 7. Ejecutar la app local contra SQL Server

Solo hagas esto despues de crear `VoidEmpires_Dev`, aplicar manualmente un esquema compatible y revisado, y completar la verificacion post-apply. La app no aplica migraciones al arrancar.

Preconditions:

1. `docs/dev/sql-server-post-apply-verification.sql` has been reviewed and run manually in SSMS.
2. `dbo.__EFMigrationsHistory` contains `20260706131610_SqlServerInitialBaseline`.
3. The read-only SQL Server smoke check passes with the same target database.
4. The connection string is stored only in the local shell, user secrets, or another operator-owned secret store.

PowerShell local con placeholders para la sesion actual:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:VoidEmpires__Persistence__Provider="SqlServer"
$env:ConnectionStrings__DefaultConnection="Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
$env:VOIDEMPIRES_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
dotnet run --project .\src\VoidEmpires.Web\VoidEmpires.Web.csproj
```

The `http` launch profile uses `http://localhost:5142`. In another terminal, verify health:

```powershell
Invoke-RestMethod http://localhost:5142/health | ConvertTo-Json -Depth 5
```

Expected health shape:

- `GET /health` debe responder `status = ok`
- `persistence.configured` debe ser `true`
- `persistence.provider` debe ser `Microsoft.EntityFrameworkCore.SqlServer`
- `auth.configured` debe ser `true`

Health only proves that configuration was read and the SQL Server EF provider was selected. It does not apply migrations, prove table contents, prove catalog seed state, or make the runtime production-ready.

Expected behavior before catalog seed:

- schema-backed endpoints can connect only if the manual baseline apply already succeeded
- gameplay tables can be empty
- catalog/readiness views that depend on seeded or code-owned catalog state can return empty options, blocked options, or validation guidance
- final relational catalog seed apply is still deferred; use the catalog helper in section 8 only as documented

Expected behavior after an approved disposable validation seed:

- Development seed endpoints can populate local QA data only when Development endpoints are enabled and the target database is disposable
- dev UI/readiness endpoints should reflect the seeded scenario rather than an empty universe
- final production catalog readiness still depends on the later final relational catalog seed path; do not infer it from Development seed success
- do not run Development seeds against a shared or production-like database

### Account registration behavior on SQL Server

When the app is explicitly configured for SQL Server and the reviewed baseline schema has been applied manually, account registration uses the same persistence boundary as other runtime account flows:

- ASP.NET Core Identity stores account rows in the Identity tables included in the SQL Server baseline.
- `POST /api/accounts/register` creates the Identity user, then bootstraps one `PlayerProfile`, one `Civilization`, one active `PlanetOwnership`, starting resource stockpile, and starting production profile.
- The bootstrap allocator can create the required account bootstrap galaxy/system/home planet records when no unowned home planet is available, so normal new-account creation does not require a prior Development seed.
- Registration returns the generated `civilizationId`, `homePlanetId`, `homePlanetName`, starting resources, and next planet route for the frontend handoff.

Manual validation checklist:

- Use `docs/dev/sql-server-user-checklist.md` for the prepared registration evidence steps.
- The checklist covers `AspNetUsers`, `PlayerProfile`, `Civilization`, `PlanetOwnership`, starting resource and production rows, second-user coexistence, and verification that no plaintext password is stored.
- Keep all evidence that contains real hostnames, usernames, emails, passwords, or resolved connection strings outside the repository.

Deferred/manual status:

- This documentation does not claim a manual SQL Server registration run was performed.
- Operators must still apply and verify the SQL Server baseline manually before using this path.
- Production hardening for confirmation, recovery, authorization on gameplay reads/mutations, backups, monitoring, and final catalog seed ownership remains outside this runbook step.

Limpia las variables para volver al modo de desarrollo por defecto:

```powershell
Remove-Item Env:ASPNETCORE_ENVIRONMENT -ErrorAction SilentlyContinue
Remove-Item Env:VoidEmpires__Persistence__Provider -ErrorAction SilentlyContinue
Remove-Item Env:ConnectionStrings__DefaultConnection -ErrorAction SilentlyContinue
Remove-Item Env:VOIDEMPIRES_CONNECTION_STRING -ErrorAction SilentlyContinue
```

## 8. Seed Catalogs And Development Data Carefully

Current repository position:

- static gameplay catalogs are still code-owned, not final relational seed rows
- Development seed profiles are QA scaffolding, not final production initialization
- the backend now includes a dry-run-first final catalog seed service, but non-dry-run apply is still intentionally deferred
- post-baseline audit result: the current checked-in catalog sources validate as `15` buildings, `8` research items, `4` orbital assets, `1` defense item, and `7` resources; the SQL Server baseline does not add dedicated final catalog tables, and `DryRun=false` still fails safely with `ApplyDeferred=true`

Post-schema catalog sequence after manual baseline apply:

1. Complete the manual schema apply and post-apply verification in sections 6 and 7.
2. Set the SQL Server connection string only in a local shell, user secret, or operator-managed secret store.
3. Run the final catalog helper without `-Apply` first.
4. Review the dry-run output for each catalog source file and row count.
5. Stop if any catalog source is missing, has invalid shape, or reports unexpected row counts.
6. Do not run a real apply yet; current backend behavior still defers final relational catalog writes.

Before enabling a real final catalog apply, a later implementation task must add or explicitly choose the relational catalog ownership model, add deterministic upsert behavior, map source keys to persisted rows without colliding with player-owned gameplay state, and add disposable-database validation. Until then, Development seed success is not final catalog readiness.

Dry-run command shape after the operator has set an external connection string:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-final-catalog-seed.ps1
```

Expected dry-run output includes `Validated ... rows` lines for the versioned catalog files and `Apply remains deferred by backend design.` The helper does not echo the connection string.

Operational guidance:

- Do not treat `POST /api/dev/seeds/apply` as the final production catalog seed path.
- Use Development seeds only for local QA after the app is intentionally configured to talk to a disposable database.
- Keep final catalog seeding separate from player-owned gameplay state seeding.
- Use `scripts/sqlserver-final-catalog-seed.ps1` only as an operator-invoked guardrail around the current backend seed service.
- El helper de catalogos finales ejecuta dry-run por defecto y valida los JSON versionados sin borrar, truncar, resetear ni sembrar datos de gameplay del usuario.
- `-Apply -ConfirmMutation` requiere confirmacion explicita y actualmente debe detenerse de forma segura mientras el backend mantenga diferido el apply relacional final.

Final catalog helper examples with placeholder-only templates:

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
- it must not delete, truncate, or reset user gameplay data
- it does not call Development seed endpoints
- it does not run migrations or perform automatic SQL Server updates

Future apply gate:

- A future apply mode must first add final relational catalog tables, deterministic upsert logic, reviewable output, and explicit operator confirmation.
- The current `-Apply -ConfirmMutation` shape documents the intended gate but is not a working production seed apply path.
- Do not use Development seed success as proof that final catalog rows were applied.

If you intentionally run Development seeds against a disposable SQL Server validation database:

1. Start the app only after setting a non-empty `ConnectionStrings__DefaultConnection`.
2. Confirm you are pointing to a disposable validation database, not a shared real environment.
3. Call the existing Development seed endpoint only for local/demo validation.

## 9. Back Up The Database

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

## 10. Restore / Rollback Guidance

Current repository does not provide an automated SQL Server rollback script.

If a manual SQL apply fails:

1. Stop and do not continue applying later batches.
2. Review the exact failed statement in SSMS.
3. If partial apply makes the database unsafe, restore from the pre-change backup.
4. Fix the migration/script issue in a dedicated repository task before retrying.

## 11. Known Current Blockers

- runtime and design-time provider selection now support an explicit SQL Server choice, but PostgreSQL remains the checked-in default path
- existing migration history is PostgreSQL-shaped
- manual SSMS apply and post-apply verification remain operator-controlled and have not been performed by repository automation
- final catalog operator scripting now exists only as a guarded dry-run and deferred-apply helper; no final relational catalog seed apply path exists yet

## 12. Recommended Future Task Order

1. complete post-apply verification documentation
2. document app run checks after the SQL Server schema exists
3. finish final relational catalog seed ownership
4. add explicit SQL Server validation gates and disposable replay checks
5. complete final operational backup and restore verification

## Validation

- This runbook is based on the current final DB prep note, SQL Server test strategy, current startup/configuration behavior, and the existing opt-in SQL Server smoke test.
- No real SQL Server connection, migration apply, seed apply, backup, or restore was performed for this documentation task.
