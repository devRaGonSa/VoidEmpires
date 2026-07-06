# SQL Server Baseline Migration Review

Status date: 2026-07-06

## Current Generation Result

`SqlServerInitialBaseline` is now available for review.

`TASK-40D` generated a migration, but that scaffold was rejected and removed because it reused the root PostgreSQL-shaped `VoidEmpiresDbContextModelSnapshot` and produced a provider-transition migration instead of an initial SQL Server schema baseline.

`TASK-40D1` added SQL Server design-time migration history isolation. `TASK-40D2` then regenerated the baseline and accepted these files:

- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/20260706131610_SqlServerInitialBaseline.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/20260706131610_SqlServerInitialBaseline.Designer.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/VoidEmpiresDbContextModelSnapshot.cs`

Initial generation checks:

- `Up` has initial-schema shape with 33 `CreateTable` calls and 67 `CreateIndex` calls.
- `Down` contains the corresponding `DropTable` cleanup path.
- generated metadata uses SQL Server types and annotations, not Npgsql metadata.
- root PostgreSQL migrations and the root PostgreSQL snapshot remain unchanged.
- no automatic migration apply was run.

## Detailed Review Still Required

- persisted entity and Identity table coverage
- construction, research, shipyard, ownership, normalized-name, and catalog-key index coverage
- decimal precision review
- SQL Server string length review
- cascade delete review
- generated-file secret scan

## Required Next Step

Run the generated baseline review retry before generating any idempotent SQL script.

No SQL Server migration was applied automatically, no generated SQL was run, and no real SQL Server connection string or credential was added.
