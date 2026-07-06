# SQL Server Migration Strategy

Status date: 2026-07-06

This document records the current repository decision for the first SQL Server migration baseline.

Current decision:

- no SQL Server migration files are added in this task
- no SQL Server migration or database update is applied
- the repository now documents the exact deferred-generation commands and prerequisites for the later baseline task

## Why No Migration Files Were Added Yet

The repository can now select SQL Server explicitly at runtime and design time, but the checked-in EF Core migration chain is still PostgreSQL-shaped.

Current blockers:

- the existing migration history and snapshot were produced from the PostgreSQL-first path
- SQL Server baseline generation still needs a full provider-sensitive model review, including naming, filters, and any provider-specific migration SQL
- adding a SQL Server migration immediately would mix a new provider-specific baseline into the current history without a documented isolated migration-directory policy

Because of that, generating and committing a SQL Server migration in this task would be higher risk than the task budget allows.

## Preconditions For The First SQL Server Baseline

Do not generate the first SQL Server migration until all of the following are true:

1. the remaining SQL Server mapping audit has been completed
2. the repository owner has decided the target migration layout, including the SQL Server output directory and snapshot strategy
3. generation is happening on a disposable local workflow, not against the user-managed target database
4. SQL Server provider selection stays external through environment variables
5. the no-auto-apply rule remains in effect

## Deferred Offline Generation Commands

Use a placeholder-only SQL Server connection string for design-time generation. Do not commit resolved credentials.

PowerShell setup:

```powershell
$env:VoidEmpires__Persistence__Provider="sqlserver"
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=VoidEmpires_GenerationOnly;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
$env:VOIDEMPIRES_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
```

Deferred baseline generation command:

```powershell
dotnet ef migrations add SqlServerInitialBaseline `
  --project src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj `
  --startup-project src/VoidEmpires.Web/VoidEmpires.Web.csproj `
  --context VoidEmpires.Infrastructure.Persistence.VoidEmpiresDbContext `
  --output-dir Persistence/Migrations/SqlServer
```

If the command tries to reuse the PostgreSQL-shaped snapshot or produces provider-conflicted diffs outside the intended SQL Server migration directory, stop and narrow the migration-layout decision in a follow-up task before committing anything.

## Deferred Script Generation Command

Only after the SQL Server baseline migration exists and has been reviewed:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-script-migration.ps1
```

El helper genera un script `.sql` idempotente para revision manual. Usa una cadena placeholder de generacion, no requiere password real, no ejecuta `database update` y no aplica migraciones. El archivo generado puede alterar esquema si un operador lo ejecuta despues en SSMS, asi que revisalo antes de cualquier apply manual y no lo commitees como salida one-off.

## Safety Rules

- do not commit a real SQL Server password
- do not auto-apply migrations during app startup or test runs
- do not point generation or apply steps at the user-managed target database by convenience
- do not treat the current PostgreSQL-shaped migration chain as directly replayable on SQL Server
- keep generated one-off SQL under local operator control unless a future task explicitly asks for a reviewed template

## Current Honest Result

This task prepares the initial SQL Server migration path by documenting exact deferred-generation commands and prerequisites only.

No SQL Server migration files were generated or committed in this task.
