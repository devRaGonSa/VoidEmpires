# SQL Server Migration Strategy

Status date: 2026-07-06

This document records the current repository decision for the first SQL Server migration baseline.

Current decision:

- no SQL Server migration files are added in this task
- no SQL Server migration or database update is applied
- decision for `SqlServerInitialBaseline`: generate only through the isolated SQL Server baseline path documented below

## Why No Migration Files Were Added Yet

The repository can now select SQL Server explicitly at runtime and design time, but the checked-in EF Core migration chain is still PostgreSQL-shaped.

## Decision For `SqlServerInitialBaseline`

Decision date: 2026-07-06.

Do not generate `SqlServerInitialBaseline` in the audit task.

Provider readiness is partial:

- runtime DI can select `UseSqlServer(...)` only when configuration explicitly requests `sqlserver`
- design-time factory can select `UseSqlServer(...)` from external environment variables
- tests cover both default PostgreSQL selection and explicit SQL Server provider selection

Baseline generation must use an isolated SQL Server path because:

- existing migrations live directly under `src/VoidEmpires.Infrastructure/Persistence/Migrations`
- the SQL Server baseline target folder is `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`
- the SQL Server baseline migration name is `SqlServerInitialBaseline`
- the target folder does not exist yet, so the next implementation task must create it only by generating the SQL Server migration into that output directory
- the current `VoidEmpiresDbContextModelSnapshot` still includes Npgsql metadata such as `NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(...)`
- the existing migration chain contains PostgreSQL-specific column types such as `uuid` and `timestamp with time zone`
- current snapshot filters still include quoted PostgreSQL-style filter SQL such as `"PlanetId" IS NULL`, while newer runtime configuration uses SQL Server-compatible lowercase filter text in at least some mappings
- mixed convention-driven table names remain visible in the snapshot, so the final SQL Server naming baseline still needs a full model review before committing generated output

Current migration/provider audit:

- `VoidEmpiresDbContext` is the migrations context and applies infrastructure configurations through `ApplyConfigurationsFromAssembly(typeof(VoidEmpiresDbContext).Assembly)`.
- `VoidEmpiresDbContextFactory` is the EF Core design-time factory.
- The design-time factory defaults to Npgsql when no provider environment variable is set.
- The design-time factory selects SQL Server only when `VoidEmpires__Persistence__Provider` or `VOIDEMPIRES_DATABASE_PROVIDER` is set to `sqlserver`.
- When SQL Server is selected without an explicit connection string, the design-time factory uses a passwordless localdb metadata placeholder: `Server=(localdb)\MSSQLLocalDB;Database=VoidEmpires_GenerationOnly;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`.
- SQL Server design-time generation replaces EF Core `IMigrationsAssembly` with `SqlServerDesignTimeMigrationsAssembly`, which only discovers migrations and snapshots under `VoidEmpires.Infrastructure.Persistence.Migrations.SqlServer`.
- Runtime registration uses the same provider expectation: `AddVoidEmpiresPersistence` falls back to Npgsql and uses SQL Server only for an explicit `sqlserver` provider value.
- The Infrastructure project references both `Microsoft.EntityFrameworkCore.SqlServer` and `Npgsql.EntityFrameworkCore.PostgreSQL`.
- The checked-in root migration chain is PostgreSQL-shaped, not provider-neutral. Most migrations and the current snapshot contain Npgsql annotations or PostgreSQL column types; one recent migration includes provider-conditional SQL for PostgreSQL and SQL Server, but that does not make the full chain replayable as a SQL Server baseline.
- `scripts/sqlserver-script-migration.ps1` is intentionally guarded and refuses to script until `*SqlServerInitialBaseline*.cs` exists under `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`.

Required safe generation approach:

1. Keep PostgreSQL/Npgsql as the default provider for normal runtime, tests, and design-time commands.
2. Select SQL Server explicitly for the one generation command through environment variables, not source changes.
3. Generate `SqlServerInitialBaseline` with `--output-dir Persistence/Migrations/SqlServer` so SQL Server artifacts are isolated from the existing root PostgreSQL migration chain.
4. Do not run `dotnet ef database update`.
5. Do not point the command at the operator-managed SQL Server database; use the passwordless design-time fallback or disposable local generation configuration.
6. Review the generated migration and SQL Server model snapshot before committing it, especially provider annotations, table and column names, indexes, filters, identity columns, Guid storage, DateTime storage, and provider-specific SQL.
7. Only after the reviewed baseline exists, use `scripts/sqlserver-script-migration.ps1` to create an idempotent SQL script for manual review.

Actionable blockers before generation:

1. rerun the isolated migration command so EF Core writes the SQL Server snapshot under the isolated folder without reusing or overwriting the root PostgreSQL snapshot
2. complete a provider-sensitive model review for table names, column names, filters, indexes, identity columns, and DateTime/Guid storage
3. confirm the intended baseline is generated from SQL Server provider metadata only
4. run generation only with placeholder external configuration and without connecting to `VoidEmpires_Dev`
5. review the generated migration before committing it

Current blockers:

- the existing migration history and snapshot were produced from the PostgreSQL-first path
- SQL Server baseline generation still needs a full provider-sensitive model review, including naming, filters, and any provider-specific migration SQL
- adding a SQL Server migration into the root migration folder would mix a new provider-specific baseline into the current PostgreSQL-shaped history

Because of that, generating and committing a SQL Server migration in this task would be higher risk than the task budget allows.

## Preconditions For The First SQL Server Baseline

Do not generate the first SQL Server migration until all of the following are true:

1. the remaining SQL Server mapping audit has been completed
2. the repository owner has decided the target migration layout, including the SQL Server output directory and snapshot strategy
3. generation is happening on a disposable local workflow, not against the user-managed target database
4. SQL Server provider selection stays external through environment variables
5. the no-auto-apply rule remains in effect

## Exact Offline Baseline Generation Command

Use the passwordless SQL Server design-time fallback or a placeholder-only SQL Server connection string for design-time generation. Do not commit resolved credentials.

Run the command from the repository root only when the baseline-generation task is active. This command creates migration files for review; it must not be followed by `dotnet ef database update`.

```powershell
$env:VoidEmpires__Persistence__Provider="sqlserver"
Remove-Item Env:ConnectionStrings__DefaultConnection -ErrorAction SilentlyContinue
Remove-Item Env:VOIDEMPIRES_CONNECTION_STRING -ErrorAction SilentlyContinue

dotnet ef migrations add SqlServerInitialBaseline `
  --project src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj `
  --startup-project src/VoidEmpires.Web/VoidEmpires.Web.csproj `
  --context VoidEmpires.Infrastructure.Persistence.VoidEmpiresDbContext `
  --output-dir Persistence/Migrations/SqlServer
```

Command facts:

- provider selector: `VoidEmpires__Persistence__Provider=sqlserver`
- design-time metadata connection: passwordless localdb fallback from `VoidEmpiresDbContextFactory`
- migration name: `SqlServerInitialBaseline`
- target folder: `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`
- EF output argument: `--output-dir Persistence/Migrations/SqlServer`
- context: `VoidEmpires.Infrastructure.Persistence.VoidEmpiresDbContext`
- project: `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj`
- startup project: `src/VoidEmpires.Web/VoidEmpires.Web.csproj`

Do not run this command against `VoidEmpires_Dev`, do not provide a real username or password, and do not follow it with `dotnet ef database update`.

## TASK-39G Deferral Result

`TASK-39G` did not generate migration files because `TASK-39F` recorded a no-go decision. The later manual command remains the approved path only after the listed blockers are resolved.

If the command tries to reuse the PostgreSQL-shaped snapshot or produces provider-conflicted diffs outside the intended SQL Server migration directory, stop and narrow the migration-layout decision in a follow-up task before committing anything.

## TASK-40D Generation Attempt Result

`TASK-40D` ran the exact offline baseline generation command with `VoidEmpires__Persistence__Provider=sqlserver` and with both design-time connection-string environment variables cleared.

The command did not connect to or update a real database, but the generated migration was rejected and removed because EF Core reused the existing root PostgreSQL-shaped migration snapshot. The scaffolded `SqlServerInitialBaseline` was a provider-transition migration with `DropIndex`, `DropPrimaryKey`, `Rename*`, and many `AlterColumn` operations from PostgreSQL column types such as `uuid` and `timestamp with time zone` into SQL Server types. That is not an initial SQL Server schema baseline.

The failed scaffold also changed the root `VoidEmpiresDbContextModelSnapshot`, so the local scaffold was removed and the snapshot was restored before commit. A narrowed follow-up task must isolate SQL Server migration history and snapshot discovery before retrying baseline generation.

## TASK-40D1 Isolation Result

SQL Server design-time migration generation now uses a provider-specific `IMigrationsAssembly` replacement from `VoidEmpiresDbContextFactory`. When `VoidEmpires__Persistence__Provider=sqlserver` is selected, EF Core discovers only migration and snapshot types in the `VoidEmpires.Infrastructure.Persistence.Migrations.SqlServer` namespace. Until the SQL Server baseline exists, that migration assembly intentionally reports no migrations and no model snapshot, so the next baseline-generation retry should scaffold `SqlServerInitialBaseline` as an initial SQL Server schema migration.

The default PostgreSQL design-time path still uses EF Core's normal root migration assembly and root `VoidEmpiresDbContextModelSnapshot`.

## TASK-40D2 Generation Retry Result

`TASK-40D2` reran the exact offline baseline generation command after SQL Server migration history isolation was added.

Accepted generated files:

- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/20260706131610_SqlServerInitialBaseline.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/20260706131610_SqlServerInitialBaseline.Designer.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/VoidEmpiresDbContextModelSnapshot.cs`

The accepted migration has initial-schema shape: 33 `CreateTable` calls and 67 `CreateIndex` calls in `Up`; `DropTable` operations appear only in `Down`. The generated files use SQL Server metadata and types such as `SqlServerModelBuilderExtensions`, `uniqueidentifier`, `datetime2`, `nvarchar`, and `decimal(18,4)`. The root PostgreSQL migration folder and root `VoidEmpiresDbContextModelSnapshot` remain unchanged.

## Deferred Script Generation Command

Only after the SQL Server baseline migration exists and has been reviewed:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\sqlserver-script-migration.ps1
```

El helper genera un script `.sql` idempotente para revision manual. Selecciona `sqlserver`, limpia las variables de cadena de conexion del proceso, usa el fallback design-time sin password real, no ejecuta `database update` y no aplica migraciones. El archivo generado puede alterar esquema si un operador lo ejecuta despues en SSMS, asi que revisalo antes de cualquier apply manual y no lo commitees como salida one-off.

## Safety Rules

- do not commit a real SQL Server password
- do not auto-apply migrations during app startup or test runs
- do not point generation or apply steps at the user-managed target database by convenience
- do not treat the current PostgreSQL-shaped migration chain as directly replayable on SQL Server
- keep generated one-off SQL under local operator control unless a future task explicitly asks for a reviewed template

## Current Honest Result

This task records a no-go decision for generating `SqlServerInitialBaseline` today and keeps the exact deferred-generation commands as the later approved path.

No SQL Server migration files were generated or committed in this task.
